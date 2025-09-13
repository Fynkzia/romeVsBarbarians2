using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

public class AIController : MonoBehaviour
{
[Header("Action Priority")]


[SerializeField] public float[] actionPrioruty;
    // 0 - стоим, ничего не делаем
    // 1 - бежим в атаку на конкретный отряда - если он близко!
    // 2 - идем к подходящему отряду чтоб дать ему песды - если он далеко!
    // 3 - отходим, ссымся. Если в бою - выходим из боя
    // 4 - бежим на помощь союзному отряду
    // 5 - бежим в точку выгодную точку (defence zone + small city)
    // 6 -  пробуем штурмануть группой
    // 7 - есть вариант комуто зайти в спину?
    // 8 - нам ничего не угражает - стратегический мув (улучшение позиции)


    [Space(10)]

[SerializeField] public List<SquadController> actionQueueArray;

    [SerializeField] public List<SquadController> allEnemiesList;
    [SerializeField] public List<SquadController> allPlayerList;


    [SerializeField] List<SquadController> playerNearSquads = new List<SquadController>();
[SerializeField]  List<SquadController> playerFarSquads = new List<SquadController>();

    [SerializeField] List<SquadController> enemyNearSquads = new List<SquadController>();
    [SerializeField] List<SquadController> enemyFarSquads = new List<SquadController>();

    [SerializeField] public List<SquadController> playerGroups;

    [SerializeField] List<AIDefencePoint> defencePoints = new List<AIDefencePoint>();

    [SerializeField] private SquadController bestSquadToAttack;
    [SerializeField] private SquadController bestSquadToRetreat;
    [SerializeField] private SquadController bestSquadToHelp;
    [SerializeField] private AIDefencePoint bestDefencePoints;


    [SerializeField] public float allEnemyPower;

    [SerializeField] public float allPlayerPower;

    [SerializeField] public float nearEnemyPower;

    [SerializeField] public float nearPlayerPower;


    [SerializeField] public List<SquadController> squadsToDelete;

    [Space(10)]
    [SerializeField] public float timeAction;


 [SerializeField] private LayerMask playerUnitLayer ;

  [SerializeField] private LayerMask enemyUnitLayer ;

 [SerializeField] private GameObject drawingPrefab;



[Header("AI Settings")]
    [Space(10)]
    
[SerializeField] public float difficulty;
[SerializeField] public float timeToGetActions;
[SerializeField] public float farRadius;
[SerializeField] public float nearRadius;
[SerializeField] public float nearEnemyRadius;

    [Header("Stategy movements")]
    [Space(10)]

    [SerializeField] public int strategyCounter;
    [SerializeField] public int strategyMoveAttemp;
    [SerializeField] public float enemyPower;
    [SerializeField] public float playerPower;

    [SerializeField] public float[] attackSpacing;

    [SerializeField] private List<Vector3> attackFormation;
    [SerializeField] private List<Vector3> defenceFormation;


    [SerializeField] private bool strategyMove;
    [SerializeField] private bool defenceMove;
    [SerializeField] private bool attackMove;
    [SerializeField] private int squadFormationIndex;

    List<SquadController> squadListToStrategy;


    [SerializeField] public Transform enemyObject;
    [SerializeField] public Transform playerObject;
    [SerializeField] public Transform defenceObject;





[Header("UI Settings")]
    [Space(10)]
[SerializeField] public Animator supportUI;
[SerializeField] public TextMeshProUGUI timerSupportUI;
[SerializeField] public TextMeshProUGUI countSupportUI;

[SerializeField] public Image supportUIBar;

[SerializeField] public bool supportUIShow;
[SerializeField] public bool supportUIAlert;

[Header("NavMesh Settings")]
    [SerializeField] private NavMeshAgent agent;
    private NavMeshPath path;

    [SerializeField] public Transform targettt;
    LineRenderer lineRenderer;

    [SerializeField] public BattleSceneManager battleSceneManager;



    // Start is called before the first frame update
    void Start()
    {
       


    }

    // Update is called once per frame
    void Update()
    {
        timeAction += Time.deltaTime;


        if (timeAction > timeToGetActions)
        {
            if (actionQueueArray.Count > 0)
            {
                if (actionQueueArray[0] != null)
                {
                    AiAction(actionQueueArray[0]);

                    if (actionQueueArray.Count > 0)
                    {
                        actionQueueArray.RemoveAt(0);
                    }
                }
                else
                {
                    actionQueueArray.RemoveAt(0);
                }
            }
           
            //actions ++;
            timeAction = 0;

            if (actionQueueArray.Count == 0)
            {
                if (squadsToDelete.Count > 0)
                {
                    DeleteSquads();
                }

                SetAllEnemiesList();


                SetQueue();
                PlayerSquadsCollectToGroup();
                SetAllDefencePoint();
                // actions = 0;
                strategyMove = false;
                attackMove = false;
                defenceMove = false;
            }

        }

       



    }


    public void AiAction(SquadController squad)
    {

        for (int i = 0; i < actionPrioruty.Length; i++) ///// очистка
        {
            actionPrioruty[i] = 0;
        }

        if (squad.squadDie) { return; }

        // познаем обстановку вокруг оттряда
        FarPlayerSquadList(squad);
        NearPlayerSquadList(squad);
        NearEnemySquadList(squad);
        //глобальные
        AllPowerCalculate();


        actionPrioruty[0] += enemyNearSquads.Count*0.7f; // рядом с союзниками стоять комфортно
        if (squad.currentMorale / squad.maxMorale < 0.6)
        {
            actionPrioruty[0] += (1-(squad.currentMorale / squad.maxMorale)) * 10; // мало морали - лучще постоять
        }

        if (allPlayerPower > allEnemyPower)
        {
            actionPrioruty[0] += 1; // лучше постоять если мы лохи по паверу

        }

        float lastPowerToAttack = -2000f;
        int bestIndexToAttack = 0;
        int countToAttck = 0;

        if (playerNearSquads.Count > 0) //  1 - бежим в атаку на конкретный отряда - если он близко!
        {


            for (int i = 0; i < playerNearSquads.Count; i++)
            {
                float p = PowerСomparison(squad, playerNearSquads[i]);
                if (p > lastPowerToAttack)
                {
                    lastPowerToAttack = p;
                    bestIndexToAttack = i;
                }
                if (p > 0)
                {
                    countToAttck++;
                }


            }
            bestSquadToAttack = playerNearSquads[bestIndexToAttack];

            actionPrioruty[1] += countToAttck;//  пересмотрт!!!!!!
            actionPrioruty[1] += lastPowerToAttack / 10;//  пересмотрт!!!!!!


            if (squad.predictEnemy != null) /// если чел уже идет к врагу или сражается - множим на 0;
            {
                if (squad.isMoved || squad.inBattle)
                {

                    if (squad.predictEnemy.transform.parent.gameObject == bestSquadToAttack.gameObject)
                    {
                        actionPrioruty[1] *= 0f;
                    }
                }
            }
        }

        if (squad.squadDie) { return; }

        if (playerFarSquads.Count > 0 && playerNearSquads.Count == 0) //  2 - идем к подходящему отряду чтоб дать ему песды - если он далеко!
        {


            for (int i = 0; i < playerFarSquads.Count; i++)
            {
                float p = PowerСomparison(squad, playerFarSquads[i]);
                if (p > lastPowerToAttack)
                {
                    lastPowerToAttack = p;
                    bestIndexToAttack = i;
                }
                if (p > 0) //
                {
                    countToAttck++;
                }


            }

            bestSquadToAttack = playerFarSquads[bestIndexToAttack];

            actionPrioruty[2] += countToAttck;//  пересмотрт!!!!!!
            actionPrioruty[2] += lastPowerToAttack / 20;//  пересмотрт!!!!!!

            if (playerFarSquads.Count == 1) // если один отряд - хотим сильнее напасть
            {

                actionPrioruty[2] += 1;
            }

            if (bestSquadToAttack.currentMorale / bestSquadToAttack.maxMorale < 0.5f) // если мало морали - хотим сильнее напасть
            {

                actionPrioruty[2] += 1;
            }

            if (bestSquadToAttack.ai_squadPower < allPlayerPower / allPlayerList.Count) // если есредний павер ниже чем у всех - хотим еще сильнее
            {

                actionPrioruty[2] += 1;
            }


            if (squad.predictEnemy != null) /// если чел уже идет к врагу или сражается - множим на 0;
            {
                if (squad.isMoved || squad.inBattle)
                {

                    if (squad.predictEnemy.transform.parent.gameObject == bestSquadToAttack.gameObject)
                    {
                        actionPrioruty[2] *= 0f;
                    }
                }
            }

        }

        if (squad.squadDie) { return; }

        if (playerNearSquads.Count > 0) //  3 - отходим, ссымся. Если в бою - выходим из боя
        {
            float lastPowerToRetreat = 0;
            int bestIndexToRetreat = 0;
            int countToRetreat = 0;

            for (int i = 0; i < playerNearSquads.Count; i++)
            {
                float p = PowerСomparison(squad, playerNearSquads[i]);

                if (p < lastPowerToRetreat)
                {
                    lastPowerToRetreat = p;
                    bestIndexToRetreat = i;
                }
                if (p < 0)
                {
                    countToRetreat++;
                }


            }
            bestSquadToRetreat = playerNearSquads[bestIndexToRetreat];

            actionPrioruty[3] += countToRetreat;//  пересмотрт!!!!!!
            actionPrioruty[3] += -lastPowerToRetreat / 10;//  пересмотрт!!!!!!

            if (squad.inBattle)
            {

                actionPrioruty[3] *= 0f;
                actionPrioruty[0] *= 0f; /// переделать!!!

                float powerCoef = (squad.enemyController[0].ai_squadPower / squad.ai_squadPower) / 10f;

                if (squad.currentMorale / squad.maxMorale < 0.35f + powerCoef)// пересмотрт!!!!!!
                {
                    actionPrioruty[3] += squad.enemyController.Count;

                    actionPrioruty[3] += 10 / squad.currentMorale;

                }


            }

        }

        if (squad.squadDie) { return; }

        float lastPowerToHelp = 0;
        int bestIndexToHelp = 0;


        if (enemyNearSquads.Count > 0) // 4 - бежим на помощь союзному отряду
        {
            for (int i = 0; i < enemyNearSquads.Count; i++)
            {
                if (enemyNearSquads[i].ai_needHelp && enemyNearSquads[i].inBattle)
                {

                    float p = PowerСomparison(enemyNearSquads[i], enemyNearSquads[i].enemyController[0]);
                    float p2 = PowerСomparison(squad, enemyNearSquads[i].enemyController[0]);

                    if (p + p2 > lastPowerToHelp)
                    {
                        lastPowerToHelp = p + p2;
                        bestIndexToHelp = i;

                        actionPrioruty[4]++;//  пересмотрт!!!!!!
                    }

                }

            }

            bestSquadToHelp = enemyNearSquads[bestIndexToHelp];


            actionPrioruty[4] += lastPowerToHelp / 10; // пересмотрт!!!!!!

        }

        if (squad.squadDie) { return; }


        float lastPriorityToDefence = -10f;
        int bestIndexToDefence = 0;

        if (defencePoints.Count > 0) // 5 - идем на выгодную дефенсЗону
        {
            for (int i = 0; i < defencePoints.Count; i++)
            {


                float p = defencePoints[i].defencePriority;


                if (p > lastPriorityToDefence)
                {
                    lastPriorityToDefence = p;
                    bestIndexToDefence = i;


                }



            }

            bestDefencePoints = defencePoints[bestIndexToDefence];


            actionPrioruty[5] += lastPriorityToDefence; // пересмотрт!!!!!!

        }


        if (squad.squadDie) { return; }

        if (actionQueueArray.Count == allEnemiesList.Count) //  8 - стратегический мув!
        {
            squadListToStrategy = new List<SquadController>();

            for (int i = 0; i < allEnemiesList.Count; i++)
            {
                if (!allEnemiesList[i].inBattle)
                {
                    if (allEnemiesList[i].ai_currentState != 1 && allEnemiesList[i].ai_currentState != 4)
                    {
                        squadListToStrategy.Add(allEnemiesList[i]);
                    }

                }
                else
                {

                    strategyMoveAttemp = 0;
                }
            }




            if (squadListToStrategy.Count >= 0)
            {

                //strategyCounter = 0;
                strategyMoveAttemp++;
                strategyMove = true;
                squadFormationIndex = 0;

                
               


                 

                attackFormation = GenerateAttackFormation(squadListToStrategy.Count);
                defenceFormation = GenerateDefenseFormation(squadListToStrategy.Count);


                if (allEnemyPower >= allPlayerPower + 300f)
                {
                    attackMove = true;
                    defenceMove = false;
                    
                }
                else if (allEnemyPower >= allPlayerPower)
                {
                    attackMove = true;
                    defenceMove = false;
                }
                else if (allEnemyPower+300 <= allPlayerPower)
                {

                    attackMove = false;
                    defenceMove = true;
                }
                else if (allEnemyPower <= allPlayerPower)
                {

                    if (Random.Range(0, 100) > 50)
                    {
                        attackMove = true;
                        defenceMove = false;
                    }
                    else
                    {
                        attackMove = false;
                        defenceMove = true;
                    }
                }


                //for (int i = 0; i < squadListToStrategy.Count; i++)
                //{

                //    if (strategyMoveAttemp >= 5)
                //    {

                //        DrawPathAndGo(squadListToStrategy[i], squadListToStrategy[i].transform.position, attackFormation[i], 2);
                //    }
                //    else
                //    {
                //        DrawPathAndGo(squadListToStrategy[i], squadListToStrategy[i].transform.position, attackFormation[i], 1);
                //    }

                //}







                //return; // все делают стратегический мув - конец!!!

            }

        }

        if (strategyMove && squadListToStrategy.Contains(squad))
        {
            actionPrioruty[8] = 10;

        }

        if (squad.squadDie) { return; }

        float lastPriority = -10f;
        int bestAction = 0;
        for (int i = 0; i < actionPrioruty.Length; i++) ///// финальное решение
        {
            if (actionPrioruty[i] > lastPriority)
            {
                lastPriority = actionPrioruty[i];
                bestAction = i;
            }
        }



        if (bestAction == squad.ai_currentState)
        {
            squad.ai_currentState = 0;
            return;
        }



        if (bestAction == 0) // бездействуем
        {
            squad.ai_currentState = 0;
            return;
        }


        if (bestAction == 1) // идием пиздицца
        {
            squad.ai_currentState = 1;
            DrawPathAndGo(squad, squad.transform.position, bestSquadToAttack.transform.position, 1);

            squad.isGoingToEnemy = true;
            squad.predictEnemy = bestSquadToAttack.TriggerObject;

            battleSceneManager.AttackAlert();
            return;
        }
        if (bestAction == 2) // подтягиваемся к врагам на пол пути
        {
            squad.ai_currentState = 2;
            DrawPathAndGo(squad, squad.transform.position, bestSquadToAttack.transform.position, 2); // пол пути
            return;
        }
        if (bestAction == 3) //отходим, ссымся.Если в бою - выходим из боя
        {

            squad.ai_currentState = 3;
            Vector3 backVector;

            if (bestSquadToRetreat != null)
            {
                backVector = Vector3.Normalize(squad.transform.position - bestSquadToRetreat.transform.position);
            }
            else
            {
                backVector = Vector3.Normalize(squad.transform.position - squad.enemyController[0].transform.position);
            }


            DrawPathAndGo(squad, squad.transform.position, squad.transform.position + (backVector * nearRadius), 1);
            return;
        }
        if (bestAction == 4) // бежим на помощь союзному отряду
        {
            squad.ai_currentState = 4;
            DrawPathAndGo(squad, squad.transform.position, bestSquadToHelp.transform.position, 1);
            return;
        }
        if (bestAction == 5) // бежим в зону которую нужно защищать
        {
            squad.ai_currentState = 5;

            bestDefencePoints.defencePriority -= 0.5f; // понижаем приоритет шоб много туда не бежало

            float defRadius = bestDefencePoints.radius;
            Vector3 randomPos = new Vector3(Random.Range(-defRadius, defRadius), 0f, Random.Range(-defRadius, defRadius));

            DrawPathAndGo(squad, squad.transform.position, bestDefencePoints.transform.position + randomPos, 1);
            return;
        }

        if (bestAction == 8) // // 8 - нам ничего не угражает - стратегический мув (улучшение позиции)
        {
            if (attackMove == true)
            {

                DrawPathAndGo(squad, squad.transform.position, attackFormation[squadFormationIndex], 1);
            }else if(defenceMove == true)
            {
                DrawPathAndGo(squad, squad.transform.position, defenceFormation[squadFormationIndex], 1);
            }

            squadFormationIndex++;
            return;
        }
    }



public void SetAllEnemiesList()
    {

        allEnemiesList = new List<SquadController>();



        for (int i = 0; i < enemyObject.transform.childCount; i++)
        {
            if (enemyObject.transform.GetChild(i).gameObject.activeSelf)
            {
                allEnemiesList.Add(enemyObject.transform.GetChild(i).GetComponent<SquadController>());
            }
        }


    }

    public void SetAllPlayersList()
    {

        allPlayerList = new List<SquadController>();

        for (int i = 0; i < playerObject.transform.childCount; i++)
        {
            if (playerObject.transform.GetChild(i).gameObject.activeSelf)
            {
                allPlayerList.Add(playerObject.transform.GetChild(i).GetComponent<SquadController>());
            }

        }


    }

    public void SetQueue()
    {

        actionQueueArray = new List<SquadController>();


        for (int i = 0; i < enemyObject.transform.childCount; i++)
        {
            actionQueueArray.Add(enemyObject.transform.GetChild(i).GetComponent<SquadController>());
        }


    }

    public void SetAllDefencePoint()
    {

        defencePoints = new List<AIDefencePoint>();



        for (int i = 0; i < defenceObject.transform.childCount; i++)
        {
            AIDefencePoint ai_defencePoint = defenceObject.transform.GetChild(i).GetComponent<AIDefencePoint>();
             ai_defencePoint.CheckDefencePoint();

            if (ai_defencePoint.needToDefence)
            {
                defencePoints.Add(ai_defencePoint);
               
            }
        }


    }

    public void AllPowerCalculate()
    {
        allEnemyPower = 0;
        allPlayerPower = 0;

        for (int i = 0; i < allEnemiesList.Count; i++)
        {
            allEnemyPower += allEnemiesList[i].AiPowerCalculate();

           

        }

        for (int i = 0; i < allPlayerList.Count; i++)
        {
            allPlayerPower += allPlayerList[i].AiPowerCalculate();



        }

    }


    public void FarEnemySquadList(SquadController squad)
{


    Collider[] nearColliders = Physics.OverlapSphere(squad.transform.position, farRadius, enemyUnitLayer);

    enemyFarSquads = new List<SquadController>();




    for (int i = 0; i < nearColliders.Length; i++)
    {
        if (nearColliders[i].tag == "Enemy")
        {
            SquadController SquadNear = nearColliders[i].GetComponent<SquadController>();

            if (!enemyFarSquads.Contains(SquadNear))
            {
                    enemyFarSquads.Add(SquadNear);




            }
        }

    }


}

public void FarPlayerSquadList(SquadController squad)
{

    Collider[] nearColliders = Physics.OverlapSphere(squad.transform.position, farRadius, playerUnitLayer);

    playerFarSquads = new List<SquadController>();

    for (int i = 0; i < nearColliders.Length; i++)
    {
        if (nearColliders[i].tag == "Squad")
        {
            SquadController SquadNear = nearColliders[i].GetComponent<SquadController>();

            if (!playerFarSquads.Contains(SquadNear))
            {
                    playerFarSquads.Add(SquadNear);




            }
        }

    }

}

public void NearEnemySquadList(SquadController squad)
    {
       

        Collider[] nearColliders = Physics.OverlapSphere(squad.transform.position, nearEnemyRadius, enemyUnitLayer);

        enemyNearSquads = new List<SquadController>();
      



        for (int i = 0; i < nearColliders.Length; i++)
        {
            if (nearColliders[i].tag == "Enemy")
            {
                SquadController SquadNear = nearColliders[i].GetComponent<SquadController>();

                if (!enemyNearSquads.Contains(SquadNear))
                {
                    enemyNearSquads.Add(SquadNear);

                   

                   
                }
            }

        }


    }

public void NearPlayerSquadList(SquadController squad)
    {

        Collider[] nearColliders = Physics.OverlapSphere(squad.transform.position, nearEnemyRadius, playerUnitLayer);

        playerNearSquads = new List<SquadController>();

        for (int i = 0; i < nearColliders.Length; i++)
        {
            if (nearColliders[i].tag == "Squad")
            {
                SquadController SquadNear = nearColliders[i].GetComponent<SquadController>();

                if (!playerNearSquads.Contains(SquadNear))
                {
                    playerNearSquads.Add(SquadNear);

                    

                    
                }
            }

        }

    }

    



    public void PlayerSquadsCollectToGroup()
    {

        SetAllPlayersList();


        playerGroups = allPlayerList;

        HashSet<SquadController> visited = new HashSet<SquadController>(); // Для отслеживания посещенных юнитов
        int groupId = 0; // Идентификатор для новой группы

        foreach (var squad in playerGroups)
        {
            if (!visited.Contains(squad))
            {
                // Создаем новую группу
                groupId++;
                List<SquadController> group = new List<SquadController>();

                // Выполняем поиск для текущего юнита
                FindGroup(squad, group, visited);

 
            }
        }

      
    }

    private void FindGroup(SquadController squad, List<SquadController> group, HashSet<SquadController> visited)
    {
        Queue<SquadController> queue = new Queue<SquadController>();
        queue.Enqueue(squad);

        float squadTotalPower = 0f;

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (!visited.Contains(current))
            {
                visited.Add(current); // Помечаем текущий юнит как посещенный
                group.Add(current); // Добавляем юнита в текущую группу

                squadTotalPower += current.ai_squadPower;

                // Проверяем соседей
                Collider[] neighbors = Physics.OverlapSphere(current.transform.position,nearRadius,playerUnitLayer);

                foreach (var neighbor in neighbors)
                {
                    SquadController SquadNear = neighbor.GetComponent<SquadController>();

                    if (playerGroups.Contains(SquadNear) && !visited.Contains(SquadNear))
                    {
                        queue.Enqueue(SquadNear);
                    }
                }
            }
        }

        for (int i = 0; i < group.Count; i++)
        {
            group[i].ai_groupPower = squadTotalPower;
        }

}








    private void DrawPathAndGo(SquadController squad,Vector3 startPos, Vector3 targetPos, int pathCoef)
{
        GameObject drawing = Instantiate(drawingPrefab);
        lineRenderer = drawing.GetComponent<LineRenderer>();

        agent.enabled = false;

        agent.transform.position = startPos;
        agent.enabled = true;


        path = new NavMeshPath();

        agent.CalculatePath(targetPos, path);

        if (path.corners.Length < 2) return;



        if (path.corners.Length >= 4)// пол пути
        {
            int middleIndex = path.corners.Length / pathCoef;

            for (int i = 0; i < middleIndex; i++)
            {
                lineRenderer.positionCount++;

                lineRenderer.SetPosition(lineRenderer.positionCount - 1, path.corners[i]);
            }


                
        }
        else 
        {

            if (pathCoef >= 2) // пол пути
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, startPos);


                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, (startPos + targetPos) / 2);
            }
            else  // полные путь
            {
                lineRenderer.positionCount = path.corners.Length;
                lineRenderer.SetPositions(path.corners);
            }
        
        }

            

        

        if (squad.isMoved)
        {
            squad.indexMove = 0;
            squad.movingPositions = null;
            // CancelMovement();
            if (lineRenderer != null)
            {
                Destroy(squad.lineRenderer.gameObject);
            }

        }

        if (squad.inBattle)
        {
            squad.SetBattle(false);
            squad.escape = true;
        }

        squad.lineRenderer = lineRenderer;

        squad.SetMoving(true);
    }


public void SqauadRetreat(SquadController enemySquad,Vector3 center) {

    Vector3 dir = Vector3.Normalize(center - enemySquad.transform.position);
    
    //SquadHalfWayToPoint(enemySquad,center-dir*nearRadius*2f);

}






public float PowerСomparison(SquadController squad,SquadController playerSquad ){
        squad.AiPowerCalculate();
        playerSquad.AiPowerCalculate();

        float squadPwoer = squad.ai_squadPower + (squad.currentAmountUnits * squad.CountCoefAnalis(playerSquad.type).x);

        float playerGroupPower = (playerSquad.ai_squadPower - playerSquad.ai_squadPower)/2f; // учитываем шо там по паверу вокруг него, чтоб не ломится на превосходящие силы
              float playerSquadPwoer = playerSquad.ai_squadPower + (playerSquad.currentAmountUnits * playerSquad.CountCoefAnalis(squad.type).x) + playerGroupPower;

        return squadPwoer - playerSquadPwoer;
 }



public void DeleteSquads() {

        
         for (int i = 0; i < squadsToDelete.Count; i++)
        {

           // Destroy(squadsToDelete[i].gameObject);

        }
        squadsToDelete.Clear();

    }


    public void DeleteSquadFromQueue(SquadController squad)
    {

        if (actionQueueArray.Contains(squad))
        {
            actionQueueArray.Remove(squad);
        }




    }


    List<Vector3> GenerateAttackFormation(int unitCount)
    {
        float minDistance = 20f ;
        float spacing =  unitCount;
        Vector3 playerPos = new Vector3();
        Vector3 center = new Vector3();

       if(spacing > 40)
        {
            spacing = 40;
        }
        for (int i = 0; i < allPlayerList.Count; i++)
        {
            playerPos += allPlayerList[i].transform.position;

        }
        playerPos /= allPlayerList.Count;


        for (int i = 0; i < allEnemiesList.Count; i++)
        {
            center += allEnemiesList[i].transform.position;

        }
        center /= allEnemiesList.Count;

        //center += playerPos - dirToPlayer * distance;


        List<Vector3> positions = new List<Vector3>();

        // направление на игрока
        Vector3 dirToPlayer = (playerPos - center).normalized;

        // перпендикуляр для линии
        Vector3 right = Vector3.Cross(Vector3.up, dirToPlayer);

        center = Vector3.MoveTowards(center, playerPos, 20f * strategyMoveAttemp);

        if(strategyMoveAttemp > 10)
        {
            center = playerPos - dirToPlayer * minDistance;
        }


        // center = playerPos;

        


        int half = unitCount / 2;
        for (int i = 0; i < unitCount; i++)
        {
            int offset = i - half;
            Vector3 pos = center + right * offset * spacing;
            positions.Add(pos);
        }

        return positions;
    }

    List<Vector3> GenerateDefenseFormation(  int unitCount )
    {
        List<Vector3> positions = new List<Vector3>();


        float radius = 5f * unitCount;
        Vector3 playerPos = new Vector3();
        Vector3 center = new Vector3();


        for (int i = 0; i < allPlayerList.Count; i++)
        {
            playerPos += allPlayerList[i].transform.position;

        }
        playerPos /= allPlayerList.Count;


        for (int i = 0; i < allEnemiesList.Count; i++)
        {
            center += allEnemiesList[i].transform.position;

        }
        center /= allEnemiesList.Count;

        // направление на игрока
        Vector3 dirToPlayer = (playerPos - center).normalized;
        float baseAngle = Mathf.Atan2(dirToPlayer.z, dirToPlayer.x);

        // угол разлёта (полукруг)
        float startAngle = baseAngle - Mathf.PI / 2f;
        float endAngle = baseAngle + Mathf.PI / 2f;

        for (int i = 0; i < unitCount; i++)
        {
            float t = (float)i / (unitCount - 1);
            float angle = Mathf.Lerp(startAngle, endAngle, t);

            Vector3 pos = center + new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            positions.Add(pos);
        }

        return positions;
    }






}
