using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class ArmyController : MonoBehaviour
{
    public bool isPlayer = true;
    public bool isCampainTarget = true;

    public string armyName;
    public int armyUnits;
    public int armySqaudCount;
    public int armyUnitsMax;

    public int armyPower;
    public float armyMorale;
    public float armyMoraleMax;

    public bool isSelected;
    public GameObject selectedObject;
    public GameObject baseObject;

    public bool inCity;
    public CityController city;
    public CityController predictCity;

    public bool retreat;
    public float retreatTime;
    public float retreatCurrentTime;

    [Header("Moving")]
    [Space(10)]
    public bool isMoved;
    public bool goToJoint;
    public bool goToBattle;
    public bool goToCity;

    public float armySpeed;
    public float armyTimeToCaptureGrid;
     float currentTimeToCaptureGrid;

    public float armySpeedMin;
    public float armySpeedMax;

    public Transform targetObjectPrefab;
    public Transform targetObject;


    public Vector3 moveDirection;

    [SerializeField] public NavMeshAgent agent;

    [Header("Morale")]
    [Space(10)]

    public float moraleUpdateTime;
    public float currentMoraleUpdateTime;


    [Header("Battle")]
    [Space(10)]
    public bool inBattle;

    public ArmyController enemyArmy;

    public ArmyController jointArmy;
    public int armyDefence;

   

    [Header("Formation")]
    [Space(10)]
    public float formationTime;
    float currentFormationTime;
    public bool isFormated;
    public int formationIndex;
    public Transform[] formationPoints;
    public Transform formationObject;

    public GameObject[] SquadMapPrefabs;


    [Header("Formation movement")]
    public float offset = 1.0f;    // Расстояние между сегментами

    private Vector3[] history;     // История позиций родителя
    private int historyLength;

    [SerializeField] float minDistance = 2f; // минимальное расстояние между отрядами
    [SerializeField] bool[] isActive;

    [Header("Ai Settings")]
    [Space(10)]
    public int ai_currentState;

    [Header("Other")]
    [Space(10)]


    public List<SquadController> squadStartList;

    public List<SquadController> squadList;

    public List<SquadController> selectedSquadList;

    public GameObject armyInfoPanel;
    [SerializeField] public TextMeshPro armyNameText;
    
    [SerializeField] public TextMeshPro armyUnitsCountText;
    [SerializeField] public TextMeshPro armyPowerCountText;

    [SerializeField] public BarUI moraleBarImage;
    


    [SerializeField] public LineRenderer lineRendererPrefab;
    [SerializeField] public LineRenderer lineRenderer;

    

    public List<Transform> squadsOnMap;  
                                         
    Quaternion unitRotation = Quaternion.Euler(0,-90,0);

    Vector3 unitSize = new Vector3(1.3f, 1.3f, 1.3f);
    Vector3 infoLocalPosition;

    Vector3 lastPositionToOffset;

    private Queue<Vector3> leaderPositions = new Queue<Vector3>();

    [SerializeField] MapControlManager mapControlManager;
    [SerializeField] GridManager grid;
    [SerializeField] public MapSceneManager mapSceneManager;
    public BoxCollider collider;
    public LayerMask checkMask;

    public event Action<ArmyController> OnArmyDestroyed;



    // Start is called before the first frame update
    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position);
        lineRenderer = Instantiate(lineRendererPrefab);

        mapSceneManager = GameObject.Find("MapSceneManager").GetComponent<MapSceneManager>();
        mapControlManager = mapSceneManager.controlController;
        collider = GetComponent<BoxCollider>();

        if (targetObject == null)
        {
            Transform newTarget = Instantiate(targetObjectPrefab, transform.position, transform.rotation);
            targetObject = newTarget;
            targetObject.gameObject.SetActive(false);
        }

        for (int i = 0; i < squadStartList.Count; i++)
        {
            SquadController newSquadController = Instantiate(squadStartList[i], transform.position, transform.rotation);
            newSquadController.gameObject.SetActive(false);
            newSquadController.transform.parent = transform;
            squadList.Add(newSquadController);

            newSquadController.InitOnMap();



            
        }
        CreateArmyOnMap();

        SetFormation(Vector3.zero);
        UpdateArmyInfo();

        SpeedCalculation();


        historyLength = squadsOnMap.Count *50; // Чем больше - тем плавнее шаг
        history = new Vector3[historyLength];

        // Заполняем стартовыми значениями
        for (int i = 0; i < historyLength; i++)
        {
            history[i] = transform.position;
        }

        grid = mapSceneManager.gridManager;

        infoLocalPosition = armyInfoPanel.transform.localPosition;
    }

    public void CreateArmyOnMap()
    {
        for (int i = 0; i < squadList.Count; i++)
        {
            GameObject newSquad = Instantiate(squadList[i].unitPrefab, transform.position, unitRotation);
            newSquad.transform.localScale = unitSize;
            newSquad.transform.GetChild(1).gameObject.SetActive(true);

            squadsOnMap.Add(newSquad.transform);
        }

     }

   



        public void UpdateArmyInfo()
    {
        ArmyMoraleUpdate();

        armyUnitsCountText.text = "" + UnitsCountUpdate();
        armyPowerCountText.text = "" + ArmyPowerUpdate();

        moraleBarImage.ChangeProgress(armyMorale,armyMoraleMax);

       
    }

    

    

   

    

   public int UnitsCountUpdate()
    {
        int count = 0;
        int max = 0;
        for (int i = 0; i < squadList.Count; i++)
        {
            count += (int)squadList[i].currentAmountUnits;
            max += (int)squadList[i].amountUnits;
        }

        armySqaudCount = squadList.Count;
        armyUnits = count;
        armyUnitsMax = max;

        return count;
    }

    public int ArmyPowerUpdate()
    {
        int power = 0;
        for (int i = 0; i < squadList.Count; i++)
        {
            power += (int)GameOptions.PowerCalculate(squadList[i]); 
        }

        armyPower = power;

        return power;
    }

    public void ArmyMoraleUpdate()
    {
        float moraleMax = 0;
        float moraleCur = 0;

        for (int i = 0; i < squadList.Count; i++)
        {
            moraleMax += squadList[i].maxMorale;
            moraleCur += squadList[i].currentMorale;
        }

        armyMorale = moraleCur;

        armyMoraleMax = moraleMax;

       
    }

    public void TargetPointCheck()
    {
        Collider[] nearColliders = Physics.OverlapSphere(targetObject.position, 3f, checkMask);

        for (int i = 0; i < nearColliders.Length; i++)
        {
            if(nearColliders[i] == collider || nearColliders[i].gameObject.tag != tag) { return; }

            if (nearColliders[i].gameObject.layer == 19 && gameObject.tag == nearColliders[i].gameObject.tag)
            {
                goToCity = true;
                predictCity = nearColliders[i].GetComponent<CityController>();
            }
            else 
            if (nearColliders[i].gameObject.layer == 18 && gameObject.tag == nearColliders[i].gameObject.tag)
            {
                goToJoint = true;
                jointArmy = nearColliders[i].GetComponent<ArmyController>();
            }
        }
     }




        public void SqaudsUpdate()
    {
        ArmyPowerUpdate();
        UnitsCountUpdate();

        UpdateArmyInfo();


        for (int i = 0; i < squadList.Count; i++)
        {
            SquadController squad = squadList[i];

            if (squad.squadDie){

                if (!squad.playerSquad)
                {


                }
                RemoveSquad(squad);

                Destroy(squad.gameObject);
            }
        }

    }


    public void AddNewSquad(SquadController squad)
    {
        SquadController newSquadController = Instantiate(squad, transform.position, transform.rotation);
        newSquadController.gameObject.SetActive(false);
        newSquadController.transform.parent = transform;
        squadList.Add(newSquadController);

        newSquadController.InitOnMap();

        GameObject newSquad = Instantiate(squad.unitPrefab, transform.position,  unitRotation);
        newSquad.transform.localScale = unitSize;
        newSquad.transform.GetChild(1).gameObject.SetActive(true);

        squadsOnMap.Add(newSquad.transform);

        if (mapControlManager == null)
        {
            if(mapSceneManager == null)
            {
                mapSceneManager = mapSceneManager = GameObject.Find("MapSceneManager").GetComponent<MapSceneManager>();
            }
            mapControlManager = mapSceneManager.controlController;
        }

        if (isSelected)
        {
            
                mapControlManager.uiManager.ArmyUIActivation(this);
            

            
        }

        SpeedCalculation();


    }

    public void AddSquadFromArmy(SquadController squad)
    {
        SquadController newSquadController = squad;
        newSquadController.gameObject.SetActive(false);
        newSquadController.transform.parent = transform;
        squadList.Add(newSquadController);

        GameObject newSquad = Instantiate(squad.unitPrefab, transform.position, unitRotation);
        newSquad.transform.localScale = unitSize;
        newSquad.transform.GetChild(1).gameObject.SetActive(true);
        squadsOnMap.Add(newSquad.transform);
        



        if (isSelected)
        {
            mapControlManager.uiManager.ArmyUIListUpdate(this);
        }

        SpeedCalculation();

        UpdateArmyInfo();

    }

    public void RemoveSquad(SquadController squad)
    {
        int indexToRemove = 0;
        for (int i = 0; i < squadList.Count; i++)
        {
            if(squad == squadList[i])
            {
                indexToRemove = i;
            }
        }


        squadList.Remove(squad);

            
        Destroy(squadsOnMap[indexToRemove].gameObject);
        squadsOnMap.RemoveAt(indexToRemove);




        if (!inBattle) // баттл сам разбереться шо делать с армией/
        {
            if (squadList.Count == 0)
            {
                ArmyDestroy();
                return;
            }
        }
        SpeedCalculation();
        UpdateArmyInfo();
    }


    public void SquadMoraleChange(float moraleChange)
    {
        for (int i = 0; i < squadList.Count; i++)
        {
            squadList[i].MoraleChange(moraleChange);
        }
    }


    public void SquadSelect(int index)
    {
       

        if (!selectedSquadList.Contains(squadList[index]))
        {
            selectedSquadList.Add(squadList[index]);
        }
        else
        {
            selectedSquadList.Remove(squadList[index]);
        }
    }

    

   

    public void ArmyDestroy() // отсылает мап баттл котроллер
    {
        
            OnArmyDestroyed?.Invoke(this); // ивент идет на компайн манагер
        

        isSelected = false;
        if (targetObject != null)
        {
            Destroy(targetObject.gameObject);
        }

        for (int i = 0; i < squadsOnMap.Count; i++)
        {
            Destroy(squadsOnMap[i].gameObject);
        }

        if (lineRenderer != null)
        {
            Destroy(lineRenderer.gameObject);
        }
     
        

        Destroy(gameObject);
     }

    public void ArmySelected()
    {
        isSelected = true;

        selectedObject.SetActive(true);

        if (targetObject == null)
        {
            Transform newTarget = Instantiate(targetObjectPrefab, transform.position, transform.rotation);
            targetObject = newTarget;
            targetObject.gameObject.SetActive(false);
        }

        targetObject.gameObject.SetActive(true);

    }

    public void ArmyDeselect()
    {
        isSelected = false;

        if (selectedObject != null)
        {
            selectedObject.SetActive(false);
            targetObject.gameObject.SetActive(false);
        }
    }




    private void OnTriggerEnter(Collider other)
    {
        if (!retreat)
        {
            if ((tag == "Player" && other.gameObject.tag == "Enemy"))//|| (tag == "Enemy" && other.gameObject.tag == "Player")
            {
                ArmyController enemy = null;

                if (other.GetComponent<ArmyController>() != null)
                {
                    enemy = other.GetComponent<ArmyController>();

                }
                else if (other.GetComponent<CityController>() != null)
                {
                    enemy = other.GetComponent<CityController>().armyInCity;
                }


                if (enemy != null)
                {
                    enemyArmy = enemy;
                    SceneLoader.Instance.CreateBattleInfo(this, enemy);


                    RedyToBattle();

                    enemy.enemyArmy = this;
                    enemy.RedyToBattle();
                    // enemy.collider.enabled = false;

                    // enemy.armyInfoPanel.SetActive(false);
                    // enemy.targetObject.gameObject.SetActive(false);

                }
            }
        }

        if (goToCity)
        {
            if (other.tag == tag && other.gameObject.layer == 19)
            {
                CityController newCity = other.GetComponent<CityController>();

                if (predictCity = newCity)
                {

                    EnterToCity(newCity);
                }
            }

        }

        Debug.Log("OnTriggerEnter");
        if (goToJoint)
        {
           
            if (other.tag == tag)
            {
                if(other.GetComponent<BoxCollider>() == jointArmy.collider)
                {
                    SetMoving(false);
                    Debug.Log("goToJoint") ;

                    if (isSelected)
                    {

                        mapControlManager.uiManager.UIReset();



                    }
                    int countToDelete = 0;
                    for (int i = 0; i < squadList.Count; i++)
                    {
                        if (jointArmy.squadList.Count < 20)
                        {
                            jointArmy.AddSquadFromArmy(squadList[i]);
                            countToDelete++;
                        }
                        else
                        {
                           
                        }

                    }
                   
                    for (int i = 0; i < countToDelete; i++)
                    {
                        RemoveSquad(squadList[0]);
                    }

                     goToJoint = false;
                    return;



                }
            }
        }

        if (isMoved)
        {
            if (other.gameObject.layer == 20) // in battle map
            {

                MapBattleController mapControlManager = other.GetComponent<MapBattleController>();

                if (mapControlManager.playerArmy.squadList.Count < 20)
                {
                    SetMoving(false);

                    mapControlManager.AddSquadsToArmy(isPlayer, this);
                }
               

            }

            if (other.tag != tag && other.gameObject.layer == 19) //завахт пустого горада
            {
                CityController newCity = other.GetComponent<CityController>();
                if (newCity.armyInCity == null)
                {

                    newCity.CityCaptured(isPlayer);
                    goToCity = true;
                    EnterToCity(newCity);
                }
                else
                {
                    if (isPlayer)
                    {
                       
                        SceneLoader.Instance.CreateBattleInfo(this, newCity.armyInCity);
                    }
                    else
                    {
                        SceneLoader.Instance.CreateBattleInfo(newCity.armyInCity, this);
                    }




                    newCity.armyInCity.enemyArmy = this;
                    enemyArmy = newCity.armyInCity;

                    RedyToBattle();
                    newCity.armyInCity.RedyToBattle();



                }
            }
        }


    }

    //city
    public void EnterToCity(CityController newCity)
    {
        if (goToCity)
        {

            
                goToCity = false;

                SetMoving(false);
            SetFormation(Vector3.zero);

            transform.rotation = Quaternion.Euler(0, 0, 0);

            targetObject.position = transform.position;
                targetObject.gameObject.SetActive(false);

            Vector3 offset = new Vector3(0,2,0);

            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, newCity.armyPoint.position );
            lineRenderer.SetPosition(1, newCity.transform.position + offset);
            lineRenderer.gameObject.SetActive(true);

            //armyInfoPanel.transform.parent = newCity.transform;

           // armyInfoPanel.transform.position = newCity.armyInfoPoint.transform.position;

            //newCity.armyInfoObject.SetActive(true);

            //collider.enabled = false;

            if (newCity.armyInCity == null)
            {
                city = newCity;

                inCity = true;
                city.armyInCity = this;


                //agent.nextPosition = city.armyPoint.position;

                agent.transform.position = city.armyPoint.position;
                //transform.position = city.armyPoint.position;
                //transform.parent = city.transform;

              


                if (isSelected)
                {

                    mapControlManager.SelectCity(city);


                }

                for (int i = 0; i < squadsOnMap.Count; i++)
                {
                    //squadsOnMap[i].gameObject.SetActive(false);
                }
            }
            else
            {


            }
        }

    }

    public void ExitFromCity()
    {
        // transform.parent = null;
        if (inCity)
        {
            inCity = false;

            Vector3 dir = transform.position - city.transform.position;

           // Debug.Log("dir = " + dir.normalized);
            //targetObject.position = targetObject.position + (dir.normalized * 5f);
            //SetMoving(true);

            //targetObject.gameObject.SetActive(true);
            collider.enabled = true;

            armyInfoPanel.transform.parent = transform;

            armyInfoPanel.transform.localPosition = infoLocalPosition;




            if (isSelected)
            {

                city.CityDeselect();

                if (!city.isSmallCity)
                {
                    city.gameObject.GetComponent<CityToDoController>().CityDeselect();
                }

            }

            city.armyInCity = null;
            city = null;


            for (int i = 0; i < squadsOnMap.Count; i++)
            {
                //squadsOnMap[i].gameObject.SetActive(true);
            }
        }
    }

    public bool RestorUnitsFromCity()
    {

        for (int i = 0; i < squadList.Count; i++)
        {
            SquadController squad = squadList[Random.Range(0, squadList.Count)];

            if (squad.amountUnits > squad.currentAmountUnits)
            {
                squad.currentAmountUnits++;
                return true;
            }
        }
        return false;
    }

    //battle

    public void GetDamage(ArmyController enAmry)
    {
        int enPower = enAmry.armyPower;
        ArmyPowerUpdate();

        if (enAmry.squadList.Count == 0 || squadList.Count == 0)
        {
            return;
        }

        Debug.Log("GetDamage enPower - " + enPower);
        Debug.Log("GetDamage plPower - " + ArmyPowerUpdate());

        float powerResult = enPower / enAmry.squadList.Count;
        Debug.Log("GetDamage powerResult - " + powerResult);

        int damageCount = enAmry.squadList.Count;
        Debug.Log("GetDamage damageCount - " + damageCount);
        for (int i = 0; i < damageCount; i++)
        {
            SquadController randomSquad = squadList[Random.Range(0, squadList.Count)];
            int multiplayerCount = 1;

            if (randomSquad.AiPowerCalculate() < powerResult)
            {
                multiplayerCount++;
            }

            randomSquad.AutoBattle_GetUnitDie(multiplayerCount);
        }
        //SqaudsUpdate();


    }

    public void RedyToBattle()
    {
        inBattle = true;
        isMoved = false;

        targetObject.position = transform.position;
        agent.Stop();

        collider.enabled = false;
        //armyInfoPanel.SetActive(false);
        targetObject.gameObject.SetActive(false);
        lineRenderer.gameObject.SetActive(false);

        SetFormation(Vector3.zero);
    }

    public void Retreat(Vector3 battlePos)
    {
        retreat = true;

        SqaudsUpdate();
        SquadMoraleChange(-2f);

        inBattle = false;
        isMoved = true;

        Vector3 dir = transform.position - battlePos;

        targetObject.position = targetObject.position + dir.normalized * 7f;
        SetMoving(true);

        collider.enabled = true;
        armyInfoPanel.SetActive(true);
        targetObject.gameObject.SetActive(true);

        SpeedCalculation();
    }

    public void ResetAfterBattle()
    {
        inBattle = false;
        isMoved = false;

        targetObject.position = transform.position;
        agent.Stop();

        collider.enabled = true;
        armyInfoPanel.SetActive(true);
        //targetObject.gameObject.SetActive(true);

        SpeedCalculation();
    }


    // Update is called once per frame
    void FixedUpdate()
    {

        if (retreat)
        {
            retreatCurrentTime += Time.deltaTime;

            if(retreatCurrentTime >= retreatTime)
            {

                retreat = false;
                retreatCurrentTime = 0;
            }
        }

        if (isMoved)
        {
            ArmyMovement();

        }
        else
        {
            if (isFormated)
            {
                currentFormationTime += Time.deltaTime;

                if(currentFormationTime > formationTime)
                {

                    //squadsOnMap[formationIndex].transform.position = formationPoints[formationIndex].position;
                    //squadsOnMap[formationIndex].transform.rotation = formationPoints[formationIndex].rotation;
                   

                    

                    if(formationIndex >= squadsOnMap.Count)
                    {
                        //isFormated = false;
                        //currentFormationTime = 0;
                        //formationIndex = 0;
                    }
                }
            }
        }

        currentMoraleUpdateTime += Time.deltaTime;

        if (currentMoraleUpdateTime > moraleUpdateTime)
        {
            if (isMoved)
            {
                for (int i = 0; i < squadList.Count; i++)
                {
                    squadList[i].MoraleChange(-squadList[i].lostMoraleThenRun*5f);
                }
                UpdateArmyInfo();
            }
            else if(!inBattle)
            {
                if (armyMorale <= armyMoraleMax)
                {
                    for (int i = 0; i < squadList.Count; i++)
                    {
                        squadList[i].MoraleChange(squadList[i].recoveryMoraleSpeed);
                    }
                    
                    UpdateArmyInfo();
                }
            }



                 
            currentMoraleUpdateTime = 0;
        }
    }

    void ArmyMovement()
    {
        //Vector3 targetPos = Vector3.MoveTowards(transform.position, targetObject.position, armySpeed * Time.fixedDeltaTime);
        moveDirection = (transform.position - targetObject.position).normalized;

        agent.SetDestination(targetObject.position);

        currentTimeToCaptureGrid += Time.deltaTime;

        if(currentTimeToCaptureGrid > armyTimeToCaptureGrid)
        {
            GridCell c = grid.GetCellAt(transform.position);
            if (c != null)
            {
                if ((!c.isPlayer && isPlayer) || (c.isPlayer && !isPlayer))
                {
                    c.Capture(isPlayer);
                }
            }

            currentTimeToCaptureGrid = 0;
        }

        // Сдвигаем историю назад и записываем новую позицию в начало
        for (int i = historyLength - 1; i > 0; i--)
        {
            history[i] = history[i - 1];
        }
        history[0] = transform.position;

        // Расставляем сегменты по истории с интервалом
        for (int i = 0; i < squadsOnMap.Count; i++)
        {
            int index = Mathf.Min((i + 1) * Mathf.RoundToInt(offset*15f*armySpeed), historyLength - 1);


            if(i > isActive.Length)
            {
                isActive = new bool[squadsOnMap.Count];

                    return;
            }

            if (!isActive[i] && i > 0)
            {
                if (isActive[i - 1] == true)
                {

                    if (Vector3.Distance(lastPositionToOffset, transform.position) >= offset * armySpeed)
                    {
                        isActive[i] = true;
                        lastPositionToOffset = transform.position;


                        

                        return;
                    }
                }
            }

            if (isActive[i])
            {
               

                // Поворот к следующей точке истории (просто для вида)
                //Vector3 dir = history[index - 1] - history[index];
                //if (dir.sqrMagnitude > 0.001f)
                //{
                //    //ssquadsOnMap[i].rotation = Quaternion.LookRotation(dir);
                //}

                if (Vector3.Distance(squadsOnMap[i].position, targetObject.position) < minDistance)
                {
                    squadsOnMap[i].transform.position = formationPoints[i].position;
                    isActive[i] = false;
                }
                else
                {
                    squadsOnMap[i].position = history[index];
                }
            }
            else
            {
                if (Vector3.Distance(lastPositionToOffset, targetObject.position) > offset)
                {
                    squadsOnMap[i].transform.position = Vector3.MoveTowards(squadsOnMap[i].transform.position, lastPositionToOffset, armySpeed / 3 * Time.deltaTime);
                }
            }

            
        }

        // Когда хвост близко и агент остановился — выравниваем формацию
        if (Vector3.Distance(targetObject.position, transform.position) <= minDistance)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance && agent.velocity.sqrMagnitude == 0f)
            {
                //

                for (int i = 0; i < isActive.Length; i++)
                {
                    isActive[i] = false;


                }

                SetMoving(false);
                SetFormation(moveDirection);
                lineRenderer.gameObject.SetActive(false);
            }
        }

        



        lineRenderer.SetPosition(0,transform.position);
       // lineRenderer.SetPosition(1, targetObject.transform.position);


    }

    

  

    public void SetMoving(bool move)
    {
        //if(isMoved && !move)
        //{
        //    SetFormation();
        //}

        isMoved = move;

        if (move)
        {

            agent.isStopped = false;
            formationIndex = 0;
            isFormated = false;

            targetObject.gameObject.SetActive(true);
            baseObject.gameObject.SetActive(false);
            TargetPointCheck();


            NavMeshPath path = agent.path;

            agent.CalculatePath(targetObject.position, path);

            if (path.corners.Length < 2)
            { // если путь ещё не построен

                
            }
            else
            {
                
            }
            Debug.Log("path.corners " + path.corners.Length);

            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
            lineRenderer.gameObject.SetActive(true);

            isActive = new bool[squadsOnMap.Count];
            isActive[0] = true; // первый сразу активен
        }
        else
        {
            agent.isStopped = true;
            targetObject.gameObject.SetActive(false);
            baseObject.gameObject.SetActive(true);

            


            GridCell c = grid.GetCellAt(transform.position);
            if (c != null)
            {
                if ((!c.isPlayer && isPlayer) || (c.isPlayer && !isPlayer))
                {
                    c.Capture(isPlayer);
                }
            }

        }

        if (inCity)
        {
            ExitFromCity();
        }



    }

    public void SpeedCalculation()
    {
        float t = 1f - (squadList.Count / 20f);
        t = t * t;
        armySpeed = Mathf.Lerp(armySpeedMin, armySpeedMax, t);
        agent.speed = armySpeed;
    }

    //formation
    IEnumerator SquadsToFormation( float delay)
    {
        for (int i = 0; i < squadsOnMap.Count; i++)
        {
            squadsOnMap[i].transform.position = formationPoints[i].position;
           // squadsOnMap[i].transform.rotation = formationPoints[i].rotation;


            yield return new WaitForSeconds(delay); // Ждем перед следующим юнитом
        }
    }

    public void SetFormation(Vector3 direction)
    {
        isFormated = true;



        currentFormationTime = 0;
        formationIndex = 0;

        if (inBattle)
        {
            Vector3 dirToArmy = (transform.position - enemyArmy.transform.position).normalized;

            formationObject.transform.rotation = Quaternion.LookRotation(dirToArmy);
        }
        else if (isMoved)
        {
            formationObject.transform.rotation = Quaternion.LookRotation(direction);

        }

        StartCoroutine(SquadsToFormation(0.05f));
    }
}
