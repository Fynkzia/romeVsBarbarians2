using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadControlManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject drawingPrefab;
    [SerializeField] private float offset;
    [SerializeField] private Color attackColor;

    [SerializeField]private int maxRoundIndex = 3;

    [SerializeField] private Transform pointDebug;
    [SerializeField] private LayerMask terrainLayer ;
    [SerializeField] private LayerMask unitLayer ;
 

    public static SquadControlManager Instance { get; private set; }

    [SerializeField]private bool hitSquad = false;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private SquadController squadController;
    private Vector3 mousePos;
     private Vector3 mousePrevPos = Vector3.zero;
    private Vector3 mousePosSum = Vector3.zero;
    private int roundIndex = 0;

    private float currentLineLength = 0;

    private const string SQUAD_TAG = "Squad";
    private const string TERRAIN_TAG = "Terrain";
    private const string ENEMY_TAG = "Enemy";

    private void Awake() {
        Instance = this;
    }

    void Update() {
        HandleSquadTouch();
    }

    private void HandleSquadTouch() {
        if (Input.GetMouseButtonDown(0)) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,1000f, unitLayer)) {
                pointDebug.position = hit.point;

                
                if (hit.collider.gameObject.tag == SQUAD_TAG) {
                    
                    squadController = hit.collider.transform.parent.gameObject.GetComponent<SquadController>();

                   

                    if (!hitSquad && !squadController.isMoved) {//squad don't hitted before and squad don't moving


                        squadController.lineRenderer = GetLineRenderer(squadController.transform.position);
                       

                    }
                    if (squadController.isMoved) {
                        squadController.tapCount++;
                    }

                    hitSquad = true;
                }
            }
        }

        if (Input.GetMouseButton(0) && hitSquad && !squadController.isMoved) {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, terrainLayer)) {
                
                DrawLine();
            }
        }

        if (Input.GetMouseButtonUp(0) && hitSquad) {
            if (!squadController.isMoved && currentLineLength > 0) { 
                currentLineLength = 0;
                squadController.SetMoving(true);
                squadController.SetBattle(false);
                TryToShortcutLine();
                TryChangeColor();
            }
            hitSquad = false;

            mousePrevPos = Vector3.zero;
            mousePosSum = Vector3.zero;
            roundIndex = 0;

        }
    }

    private void DrawLine() {

        if (lineRenderer != null)
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000f, terrainLayer))
            {
                if (hit.collider.gameObject.tag == TERRAIN_TAG)
                {
                    mousePos = new Vector3(hit.point.x, hit.point.y + offset, hit.point.z);

                    if (mousePrevPos != mousePos)
                    {
                        mousePosSum += mousePos;
                        roundIndex++;

                        if (roundIndex == maxRoundIndex)
                        {





                            if (lineRenderer.positionCount > 0)
                            {
                                currentLineLength += Vector3.Distance(lineRenderer.GetPosition(lineRenderer.positionCount - 1), mousePosSum / roundIndex);
                            }
                            if (currentLineLength < GameOptions.maxLineLength)
                            {
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
        else
        {
            
            squadController.lineRenderer = GetLineRenderer(squadController.transform.position);
        }

    }

    private void TryChangeColor() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit,1000f,unitLayer)) {
            if (hit.collider.transform.parent.gameObject.tag == ENEMY_TAG) {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1,hit.collider.gameObject.transform.position);
                lineRenderer.startColor = attackColor;
                lineRenderer.endColor = attackColor;
                squadController.predictEnemy = hit.collider;
                squadController.isGoingToEnemy = true;
            }
            else
            {
                squadController.predictEnemy = null;
                squadController.isGoingToEnemy = false;
            }
        }
        else
        {
            squadController.predictEnemy = null;
            squadController.isGoingToEnemy = false;
        }
    }

    private void TryToShortcutLine()
    {
        if(lineRenderer.positionCount > 3)
        {
            for (int i = 0; i < lineRenderer.positionCount; i++)
            {
                if(Vector3.Distance(squadController.transform.position, lineRenderer.GetPosition(i)) < squadController.aroundRadius)
                lineRenderer.SetPosition(i, squadController.transform.position);
            }

                
           
        }
    }
    public bool HasHitSquad() {
        return hitSquad;
    }

    public LineRenderer GetLineRenderer(Vector3 position)
    {
        GameObject drawing = Instantiate(drawingPrefab);
        lineRenderer = drawing.GetComponent<LineRenderer>();


        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, position);

       
        return lineRenderer;
    }

    public void SquadWayToPoint(SquadController Squad, Vector3 point)
    {
        GameObject drawing = Instantiate(drawingPrefab);
        LineRenderer lineRenderer = drawing.GetComponent<LineRenderer>();
        Squad.lineRenderer = lineRenderer;

        Vector3 norm = Vector3.Normalize(point - Squad.transform.position);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, Squad.transform.position);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, Squad.transform.position + (norm));

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);

        // lineRenderer.positionCount++;
        //lineRenderer.SetPosition(lineRenderer.positionCount - 1, Squad.transform.position + (norm*30));


        // Squad.SetBattle(false);
        Squad.SetMoving(true);
       

    }

}
