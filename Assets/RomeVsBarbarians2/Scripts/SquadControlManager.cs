using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadControlManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject drawingPrefab;

    public static SquadControlManager Instance { get; private set; }

    private bool hitSquad = false;
    private LineRenderer lineRenderer;
    private Vector3 mousePos;
    private Vector3 mousePrevPos = Vector3.zero;
    private const string SQUAD_TAG = "Squad";

    private void Awake() {
        Instance = this;
    }

    private void Update() {
        HandleSquadTouch();
    }

    private void HandleSquadTouch() {
        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Began) {
            Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                if (hit.collider.gameObject.tag == SQUAD_TAG) {
                    hitSquad = true;
                    GameObject drawing = Instantiate(drawingPrefab);
                    lineRenderer = drawing.GetComponent<LineRenderer>();
                }
            }
        }

        if (Input.touchCount > 0 && (Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary)) {
            DrawLine();
        }

        if (Input.touchCount > 0 && Input.touches[0].phase == TouchPhase.Ended) {
            hitSquad = false;
        }
    }

    private void DrawLine() {
        
        Ray ray = cam.ScreenPointToRay(Input.touches[0].position);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider != null) {
                mousePos = new Vector3(hit.point.x, hit.point.y, hit.point.z);
                if(mousePrevPos != mousePos) {
                   
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1,mousePos);
                    mousePrevPos = mousePos;
                }
            }
        }
    }

    public bool HasHitSquad() {
        return hitSquad;
    }
}
