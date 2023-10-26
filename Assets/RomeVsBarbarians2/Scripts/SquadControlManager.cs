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
    [SerializeField] private LayerMask terrainLayer ;
    [SerializeField] private LayerMask unitLayer ;

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
    private float maxTimeWait = 0.5f;
    private float startTapTime = 0;
    private bool firstTap = false;

    private const string SQUAD_TAG = "Squad";
    private const string TERRAIN_TAG = "Terrain";
    private const string ENEMY_TAG = "Enemy";

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        HandleSquadTouch();
        if (firstTap) {
            startTapTime += Time.deltaTime;

            if (startTapTime > maxTimeWait) {
                firstTap = false;
                startTapTime = 0;
                hitSquad = false;
            }
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
                        squadController = hit.collider.transform.GetComponent<SquadController>();
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
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (!Physics.Raycast(ray, out hit, 1000f, unitLayer)) {
                DrawLine();
            }
        }

        if (Input.GetMouseButtonUp(0) && hitSquad && !isSquadMoving) {
            currentLineLength = 0;
            squadController.SetMoving(true);
            squadController.SetBattle(false);
            TryChangeColorToRed();
            hitSquad = false;
            firstTap = false;
        }
    }

    private void DrawLine() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit,1000f,terrainLayer)) {
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
                            lineRenderer.SetPosition(lineRenderer.positionCount - 1, mousePosSum / roundIndex);
                        }
                        roundIndex = 0;
                        mousePosSum = Vector3.zero;
                    }
                    mousePrevPos = mousePos;
                }
            }
        }
        
    }

    private void TryChangeColorToRed() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit,1000f,unitLayer)) {
            if (hit.collider.gameObject.tag == ENEMY_TAG) {
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
            }
        }
    }
    public bool HasHitSquad() {
        return hitSquad;
    }

}
