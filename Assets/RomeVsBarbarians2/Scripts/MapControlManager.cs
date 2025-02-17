using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapControlManager : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private GameObject drawingPrefab;
    [SerializeField] private float offset;
    [SerializeField] private Color attackColor;

    [SerializeField] private CameraMapMovement cameraMapMovement;




    [SerializeField] private LayerMask terrainLayer ;
    [SerializeField] private LayerMask mapLayer ;

    [SerializeField] private List<Vector3> pointsList = new List<Vector3>();

    [SerializeField] public bool cameraCentred = false;
    [SerializeField]public bool tapArmy = false;
    [SerializeField] public bool tapCity = false;
    [SerializeField] public bool tapBattle = false;
    [SerializeField] public bool tapToSelected = false;
    [SerializeField] public bool drawPath = false;
    [SerializeField] private Collider selectedCollider;


    [SerializeField] private ArmyController armyController;
    [SerializeField] private CityController cityController;
    [SerializeField] private HireController hireController;

    [SerializeField] private BattleInfo battleController;
    [SerializeField] private Vector3 battleControllerOffset;


    [SerializeField] public GameObject armyUIPanel;
    [SerializeField] public GameObject cityUIPanel;
    [SerializeField] public GameObject hireUIPanel;


    private Vector3 mousePos;
     private Vector3 mousePrevPos = Vector3.zero;
    private Vector3 mousePosSum = Vector3.zero;
    private int roundIndex = 0;

    private float currentLineLength = 0;

    private const string PLAYER_TAG = "Player";
    private const string TERRAIN_TAG = "Terrain";
    private const string ENEMY_TAG = "Enemy";


    void Start()
    {
        armyUIPanel.SetActive(false);
        cityUIPanel.SetActive(false);
        hireUIPanel.SetActive(false);
    }


    void Update() {
        if (Input.touchCount < 2)
        {
            HandleSquadTouch();
        }
    }

    private void HandleSquadTouch() {
        if (Input.GetMouseButtonDown(0)) {

            
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 3000f, mapLayer))
                {

                if (!tapBattle)
                {

                    if (hit.collider.gameObject.GetComponent<BattleInfo>() != null)
                    {
                        battleController = hit.collider.gameObject.GetComponent<BattleInfo>();

                        if (battleController.isSelect)
                        {
                            battleController.Select(false);

                            tapBattle = true;

                        }
                        else
                        {
                            battleController.Select(true);

                            tapBattle = true;
                            cameraMapMovement.CamToPoint(hit.collider.transform.position + battleControllerOffset,null);

                            cameraMapMovement.Zoom(true);
                        }

                      
                    }

                }
                else
                {
                    if (hit.collider.gameObject.GetComponent<BattleInfo>() != null)
                    {
                        battleController = hit.collider.gameObject.GetComponent<BattleInfo>();

                        if (battleController.isSelect)
                        {

                            battleController.Select(false);
                            tapBattle = false;
                            battleController = null;

                            cameraMapMovement.Zoom(false);
                        }
                    }
                }

                if (hit.collider.gameObject.tag == PLAYER_TAG)
                {

                    //if (!tapArmy && !tapCity)
                    //{

                    if (hit.collider == selectedCollider)
                    {
                        tapToSelected = true;

                        if (tapCity && armyController != null)
                        {
                            armyController.gameObject.SetActive(true);
                        }
                    }
                    else
                    {

                        if (tapCity)
                        {
                            tapCity = false;

                            hireController.CityDeselect();
                            if (armyController != null)
                            {
                                armyController.ArmyDeselect();
                            }
                            armyController = null;
                            hireController = null;
                            cityController = null;

                            armyUIPanel.SetActive(false);
                            cityUIPanel.SetActive(false);
                            hireUIPanel.SetActive(false);
                        }

                        if (hit.collider.gameObject.GetComponent<ArmyController>() != null)
                        {
                            armyController = hit.collider.transform.gameObject.GetComponent<ArmyController>();

                            

                            armyUIPanel.SetActive(true);
                            armyController.ArmySelected();

                            tapArmy = true;
                            cameraCentred = false;
                            cameraMapMovement.CamToPoint(hit.collider.transform.position, hit.collider.gameObject);

                            cameraMapMovement.mapFollowObject = armyController.gameObject;

                            selectedCollider = hit.collider;

                            cameraMapMovement.Zoom(true);

                        }
                        else if (hit.collider.gameObject.GetComponent<CityController>() != null)
                        {

                            cityController = hit.collider.transform.gameObject.GetComponent<CityController>();
                            cityUIPanel.SetActive(true);

                            if (cityController.armyInCity != null)
                            {
                                armyController = cityController.armyInCity;
                                armyUIPanel.SetActive(true);

                                armyController.ArmySelected();
                                tapArmy = true;
                            }
                            else
                            {

                                armyUIPanel.SetActive(false);
                            }

                            hireController = hit.collider.transform.gameObject.GetComponent<HireController>();

                            hireUIPanel.SetActive(true);

                            hireController.CitySelected();
                            tapCity = true;
                            cameraCentred = false;
                            cameraMapMovement.CamToPoint(hit.collider.transform.position, null);

                            selectedCollider = hit.collider;

                            cameraMapMovement.Zoom(true);
                        }


                        //}
                        //else if (tapArmy || tapCity)
                        //{

                        //}
                    }
                }


                }
            
           
        }

        if (Input.GetMouseButton(0) ) {


            if (tapArmy && tapToSelected && cameraCentred)
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 3000f, terrainLayer))
                {

                    if (armyController != null)
                    {

                        if (armyController.targetObject != null && !armyController.inBattle)
                        {
                            if (!tapCity)
                            {
                                armyController.targetObject.transform.position = hit.point;
                            }
                            else
                            {
                                
                                armyController.targetObject.transform.position = hit.point;
                                
                            }
                            drawPath = true;
                            cameraMapMovement.ignoreMovement = true;
                        }
                    }






                }
            }
        }

        if (Input.GetMouseButtonUp(0)) {
            

            if (tapCity && tapArmy && drawPath)
            {
                armyController.transform.parent = null;
                armyController.gameObject.SetActive(true);

                cityController.armyInCity = null;

                
            }

            if (tapArmy && tapToSelected)
            {

                armyController.SetMoving(true);

                
                drawPath = false;
                tapToSelected = false;
                cameraMapMovement.ignoreMovement = false;

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 3000f, mapLayer))
                {
                    if (hit.collider.gameObject.tag == PLAYER_TAG && hit.collider != selectedCollider && hit.collider.gameObject.layer == 18)
                    {
                        armyController.jointArmy = hit.collider.transform.gameObject.GetComponent<ArmyController>();
                        armyController.goToJoint = true;
                    }

                    }
              }

            if (tapBattle)
            {
                //battleController = null;
                //tapBattle = false;
            }
            

            mousePrevPos = Vector3.zero;
            mousePosSum = Vector3.zero;
            roundIndex = 0;

        }
    }



    public void CencelSelection()
    {
        armyUIPanel.SetActive(false);
        cityUIPanel.SetActive(false);
        hireUIPanel.SetActive(false);

        drawPath = false;
        tapToSelected = false;
        cameraCentred = false;

        tapArmy = false;
        tapCity = false;

        tapBattle = false;
        if (battleController != null)
        {
            battleController.Select(false);
            battleController = null;
        }

        selectedCollider = null;

        cameraMapMovement.Zoom(false);
    }

    public void SelectBattle(BattleInfo info)
    {

        CencelSelection();

        tapBattle = true;
        
            
            battleController = info;

        cameraMapMovement.CamToPoint(info.transform.position + battleControllerOffset,null);

        battleController.Select(true);


        cameraMapMovement.Zoom(true);
    }






}
