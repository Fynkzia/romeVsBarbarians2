using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapControlManager : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster raycaster;

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


    [SerializeField] public ArmyController armyController;
    [SerializeField] public CityController cityController;
    [SerializeField] private CityToDoController toDoController;

    [SerializeField] public MapBattleController battleController;
    [SerializeField] private Vector3 battleControllerOffset;

    [SerializeField] public UIManager uiManager;
    

    [SerializeField] private float infoUpdateTime = 3f;
    private float currentInfoUpdateTime = 0f;

    [SerializeField] private float unzoomDistance = 10f;
    private float currentUnzoomDistance = 0f;

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
       
    }


    void Update() {
        if (Input.touchCount < 2)
        {
            HandleSquadTouch();
        }

        if (tapBattle && battleController != null)
        {
            currentInfoUpdateTime += Time.deltaTime;

            if (currentInfoUpdateTime > infoUpdateTime)
            {
                BattleUpdate();
                currentInfoUpdateTime = 0;
            }
        }
    }

    private void HandleSquadTouch() {
        if (Input.GetMouseButtonDown(0)) {



            EventSystem eventSystem = EventSystem.current;
           

            PointerEventData eventData = new PointerEventData(eventSystem)
            {
                position = Input.mousePosition
            };
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.layer == 5) // Игнорируем элементы с этим тегом
                    return;


            }


            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 3000f, mapLayer))
                {

                if (!tapBattle)
                {

                    if (hit.collider.gameObject.GetComponent<MapBattleController>() != null)
                    {
                        battleController = hit.collider.gameObject.GetComponent<MapBattleController>();

                        if (battleController.isSelect)
                        {
                            battleController.Select(false);

                            uiManager.UIReset();

                            tapBattle = true;

                        }
                        else
                        {

                            battleController.Select(true);

                            uiManager.BattlePanelActivation(battleController);

                            tapBattle = true;

                            cameraMapMovement.CamToPoint(hit.collider.transform.position + battleControllerOffset,null);
                            cameraMapMovement.Zoom(true);
                        }

                      
                    }

                }
                else
                {
                    if (hit.collider.gameObject.GetComponent<MapBattleController>() != null)
                    {
                        battleController = hit.collider.gameObject.GetComponent<MapBattleController>();

                        if (battleController.isSelect)
                        {

                            battleController.Select(false);


                            uiManager.UIReset();


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

                        if (tapArmy)
                        {
                            if(armyController != null)
                            {
                                armyController.SetMoving(false);
                                armyController.SetFormation(armyController.moveDirection);
                            }
                        }
                    }
                    else
                    {
/// ресет всего
                        if (tapCity)
                        {
                            tapCity = false;

                            toDoController.CityDeselect();
                            if (armyController != null)
                            {
                                armyController.ArmyDeselect();
                            }
                           
                            toDoController = null;
                            cityController = null;

                            
                        }


                        if (tapArmy)
                        {
                            tapArmy = false;

                            armyController = null;
                           


                        }

                        if (tapBattle)
                        {
                            tapBattle = false;

                           
                        }

                         uiManager.UIReset();
/// ресет всего                          








// нажали на армию
                        if (hit.collider.transform.parent.GetComponent<ArmyController>() != null)
                        {
                            armyController = hit.collider.transform.parent.GetComponent<ArmyController>();

                            

                          
                            armyController.ArmySelected();

                            armyController.lineRenderer.gameObject.SetActive(true);

                            tapArmy = true;
                            cameraCentred = false;
                            cameraMapMovement.CamToPoint(hit.collider.transform.position, hit.collider.gameObject);

                            cameraMapMovement.mapFollowObject = armyController.gameObject;

                            selectedCollider = hit.collider;

                            cameraMapMovement.Zoom(true);

                            uiManager.ArmyUIActivation(armyController);

                        }
                        else if (hit.collider.gameObject.GetComponent<CityController>() != null )
                        {
// нажали на город
                            cityController = hit.collider.transform.gameObject.GetComponent<CityController>();

                            if (!cityController.isSmallCity)
                            {
                                uiManager.CityUIActivation(cityController);



                                cityController.CitySelected();

                                if (cityController.armyInCity != null)
                                {
                                    armyController = cityController.armyInCity;
                                    armyController.SelectInCity();
                                    //uiManager.ArmyUIActivation(armyController);


                                    // armyController.ArmySelected();
                                    // tapArmy = true;
                                }
                                else
                                {


                                }

                                toDoController = hit.collider.transform.gameObject.GetComponent<CityToDoController>();



                                toDoController.CitySelected();
                                tapCity = true;
                                cameraCentred = false;
                                cameraMapMovement.CamToPoint(hit.collider.transform.position, null);

                                selectedCollider = hit.collider;

                                cameraMapMovement.Zoom(true);

                                Debug.Log("CITY TAP");
                            }
                            else
                            {
                                if (cityController.armyInCity != null)
                                {
                                    armyController = cityController.armyInCity;




                                    armyController.ArmySelected();

                                    armyController.lineRenderer.gameObject.SetActive(true);

                                    tapArmy = true;
                                    cameraCentred = false;
                                    cameraMapMovement.CamToPoint(hit.collider.transform.position, hit.collider.gameObject);

                                    cameraMapMovement.mapFollowObject = armyController.gameObject;

                                    selectedCollider = hit.collider;

                                    cameraMapMovement.Zoom(true);

                                    uiManager.ArmyUIActivation(armyController);
                                }
            
                            }
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
                Ray ray = cam.ScreenPointToRay(Input.mousePosition + new Vector3(0f,25f,0f));
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 3000f, terrainLayer))
                {

                    if (armyController != null)
                    {

                        if (armyController.targetObject != null && !armyController.inBattle)
                        {
                            if (!tapCity)
                            {

                                if (armyController.lineRenderer.positionCount > 2)
                                {
                                    armyController.lineRenderer.positionCount = 2;
                                }
                                armyController.targetObject.gameObject.SetActive(true);
                                armyController.lineRenderer.SetPosition(0, armyController.transform.position);
                                armyController.lineRenderer.SetPosition(1, armyController.targetObject.transform.position);
                                armyController.targetObject.transform.position = hit.point;


                                currentUnzoomDistance = Vector3.Distance(armyController.targetObject.transform.position, armyController.transform.position);

                                if (currentUnzoomDistance >= unzoomDistance)
                                {
                                    //cameraMapMovement.Zoom(false);


                                    //cameraMapMovement.CamToPoint(armyController.targetObject.transform.position, null);
                                }
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
                //armyController.ExitFromCity();

                
            }

            
               
            

            if (tapArmy && tapToSelected)
            {
                float targetDistance = Vector3.Distance(armyController.targetObject.transform.position, armyController.transform.position);

                float minimumDisnace = 5;
                if (targetDistance > minimumDisnace)
                {
                    armyController.SetMoving(true);

                    if (drawPath)
                    {
                        cameraCentred = false;
                        cameraMapMovement.CamToPoint(armyController.gameObject.transform.position, armyController.gameObject);





                        cameraMapMovement.Zoom(true);
                    }



                    drawPath = false;
                    tapToSelected = false;
                    cameraMapMovement.ignoreMovement = false;

                    //Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    //RaycastHit hit;
                    //if (Physics.Raycast(ray, out hit, 3000f, mapLayer))
                    //{
                    //    if (hit.collider.gameObject.tag == PLAYER_TAG && hit.collider != selectedCollider && hit.collider.gameObject.layer == 18)
                    //    {
                    //        armyController.jointArmy = hit.collider.transform.gameObject.GetComponent<ArmyController>();
                    //        armyController.goToJoint = true;
                    //    }
                    //    else if (hit.collider.gameObject.layer == 19)
                    //    {


                    //        armyController.goToCity = true;
                    //    }

                    //}
                }
                else
                {
                    drawPath = false;
                    tapToSelected = false;
                    cameraMapMovement.ignoreMovement = false;

                    armyController.SetMoving(false);
                }
            }
            else 
            {
                
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
        uiManager.UIReset();

        if (tapArmy)
        {
            if (armyController != null)
            {
                armyController.ArmyDeselect();
            }
        }

        if (tapCity)
        {
            cityController.CityDeselect();

            if(cityController.armyInCity != null)
            {
                cityController.armyInCity.DeselectInCity();
            }
        }


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

    public void BattleUpdate()
    {


        uiManager.BattlePanelUpdate(battleController);

       


    }

    public void SelectBattle(MapBattleController info)
    {
        
            CencelSelection();

            tapBattle = true;


            battleController = info;

            cameraMapMovement.CamToPoint(info.transform.position + battleControllerOffset, null);

            battleController.Select(true);

        uiManager.BattlePanelActivation(battleController);
          

            cameraMapMovement.Zoom(true);
        
    }

    public void SelectCity(CityController info)
    {

        CencelSelection();

      


        cityController = info;

       

        uiManager.CityUIActivation(cityController);



        cityController.CitySelected();

        //if (cityController.armyInCity != null)
        //{
        //    armyController = cityController.armyInCity;
        //    uiManager.ArmyUIActivation(armyController);


        //    armyController.ArmySelected();
        //    tapArmy = true;
        //}
        //else
        //{


        //}

        if (!cityController.isSmallCity)
        {
            toDoController = cityController.gameObject.GetComponent<CityToDoController>();



            toDoController.CitySelected();
            tapCity = true;
            cameraCentred = false;
            cameraMapMovement.CamToPoint(cityController.gameObject.transform.position, null);

            selectedCollider = cityController.GetComponent<Collider>();

            cameraMapMovement.Zoom(true);

            Debug.Log("CITY Select");
        }
        else
        {
            
        }

    }






}
