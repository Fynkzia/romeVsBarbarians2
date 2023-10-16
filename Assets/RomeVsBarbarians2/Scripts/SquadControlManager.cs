using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadControlManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject drawingPrefab;
    [SerializeField] private float offset;

    [SerializeField]private int maxRoundIndex = 3;

    [SerializeField] private Transform pointDebug;

    public static SquadControlManager Instance { get; private set; }

    private bool hitSquad = false;
    private LineRenderer lineRenderer;
    private SquadController squadController;
    private Vector3 mousePos;
    private Vector3 mousePrevPos = Vector3.zero;
    private Vector3 mousePosSum = Vector3.zero;
    private int roundIndex = 0;
    public bool isSquadMoving = false;

    private float currentLineLength = 0;
    private float maxTimeWait = 1;
    [SerializeField]private float startTapTime = 0;
    private bool firstTap = false;

    private const string SQUAD_TAG = "Squad";
    private const string TERRAIN_TAG = "Terrain";

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        HandleSquadTouch();
        if (firstTap) {
            startTapTime += Time.deltaTime;
        }
    }

    private void HandleSquadTouch() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                pointDebug.position = hit.point;
                if (hit.collider.gameObject.tag == SQUAD_TAG) {
                    if (!hitSquad && !isSquadMoving) {//squad don't hitted before and squad don't moving
                        hitSquad = true;
                        GameObject drawing = Instantiate(drawingPrefab);
                        lineRenderer = drawing.GetComponent<LineRenderer>();
                        squadController = hit.collider.transform.parent.GetComponent<SquadController>();
                        squadController.lineRenderer = lineRenderer;
                    }
                    if(hitSquad && isSquadMoving && startTapTime> 0 && startTapTime<maxTimeWait) {
                        firstTap = false;
                        startTapTime = 0;
                        squadController.CancelMovement();
                        currentLineLength = 0;
                        hitSquad = false;
                    }
                    if (isSquadMoving) {
                        firstTap = true;
                        hitSquad = true;
                    }
                }
            }
        }

        if (Input.GetMouseButton(0) && hitSquad && !isSquadMoving) {
            firstTap = false;
            DrawLine();
        }

        if (Input.GetMouseButtonUp(0) && hitSquad && !isSquadMoving) {
            currentLineLength = 0;
            squadController.SetMoving(true);
            hitSquad = false;
            firstTap = false;
        }
    }

    private void DrawLine() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.gameObject.tag == TERRAIN_TAG) {
                mousePos = new Vector3(hit.point.x, hit.point.y+offset, hit.point.z);

                if(mousePrevPos != mousePos) {
                    mousePosSum += mousePos;
                    roundIndex++;

                    if (roundIndex == maxRoundIndex) {
                        if (lineRenderer.positionCount > 0) {
                            currentLineLength += Vector3.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), mousePosSum / roundIndex);
                        }
                        if (currentLineLength < GameOptions.Instance.maxLineLength) {
                            lineRenderer.positionCount++;
                            lineRenderer.SetPosition(lineRenderer.positionCount - 1,mousePosSum/maxRoundIndex);
                        }
                        roundIndex = 0;
                        mousePosSum = Vector3.zero;
                    }
                    mousePrevPos = mousePos;
                }
            }
        }
    }

    public bool HasHitSquad() {
        return hitSquad;
    }

}
