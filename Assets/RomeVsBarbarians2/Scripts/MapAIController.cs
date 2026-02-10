using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;

public class MapAIController : MonoBehaviour
{

    [Header("City Action Patern")]

    [SerializeField] public float[] cityActionPatern;
    [Space(10)]
    [SerializeField] public int cityIndex;
    [SerializeField] public int cityActionIndex;

    public SquadController[] squadHireListInCity;

    [Header("Army Action Priority")]


[SerializeField] public float[] armyActionPriority;
    // 0 - стоим, ничего не делаем
    // 1 - идем на вражескую армию (если далеко идем в сторону, если близко атакуем)
   
    // 2 - помагаем союзной армии рядом если она в бою и проигрывает
    // 3 - отходим
    // 4 - идем в город 

    // 5 - атакуем вражеский город
   
    // 6 - соединяемся с другой армией 

    







    [Space(10)]

[SerializeField] public List<ArmyController> actionArmyQueueArray;
    [SerializeField] public List<CityController> actionCityQueueArray;

    [SerializeField] public List<ArmyController> allEnemiesList;
    [SerializeField] public List<ArmyController> allPlayerList;

    [SerializeField] public List<CityController> allEnemiesCityList;
    [SerializeField] public List<CityController> allPlayerCityList;

    [SerializeField] public List<CityController> nearPlayerCityList;
    [SerializeField] public List<CityController> dangerCity;


    [SerializeField] List<ArmyController> playerNearArmies = new List<ArmyController>();
    [SerializeField]  List<ArmyController> playerFarArmies = new List<ArmyController>();

    [SerializeField] List<ArmyController> enemyNearArmies = new List<ArmyController>();
    [SerializeField] List<ArmyController> enemyFarArmies = new List<ArmyController>();

    [SerializeField] public ArmyController bestArmyToAttack;
    [SerializeField] public ArmyController bestArmyToHelp;

    [SerializeField] public ArmyController bestArmyToStrategyAttack;
    [SerializeField] public ArmyController bestArmyToStrategyMove;

    [SerializeField] public ArmyController bestArmyToRetret;

    [SerializeField] public ArmyController bestArmyToJoint;

    [SerializeField] public CityController bestCityToRetret;
    [SerializeField] public CityController bestCityToGo;
    [SerializeField] public CityController bestCityToAttack;



    [SerializeField] public float allEnemyPower;
    [SerializeField] public float nearEnemyPower;

    [SerializeField] public float nearPlayerPower;
    [SerializeField] public float allPlayerPower;




    [Space(10)]
    [SerializeField] public float armyTimeAction;
    [SerializeField] public float cityTimeAction;


    [SerializeField] private LayerMask playerArmyLayer;

  [SerializeField] private LayerMask enemyArmyLayer;
    [SerializeField] private LayerMask playerCityLayer;

    [SerializeField] private GameObject drawingPrefab;



[Header("AI Settings")]

    [Space(10)]


    [SerializeField] public float timeToGetActions;
    [SerializeField] public float timeToGetCityActions;


    [SerializeField] public float difficulty;

[SerializeField] public float farRadius;
[SerializeField] public float nearRadius;
[SerializeField] public float nearEnemyRadius;

    [SerializeField] public float enemyStartCoins;
    //[SerializeField] public float enemyAddCoins;
    //[SerializeField] public float enemyAddCoinsPerCity;

    [SerializeField] public float enemyCoins;


    [Header("NavMesh Settings")]
    [SerializeField] private NavMeshAgent agent;
    private NavMeshPath path;

   
    LineRenderer lineRenderer;

    [Header("Other")]

    [SerializeField] MapSceneManager mapSceneManager;

    private void Start()
    {
        enemyCoins = enemyStartCoins;
    }

    // Update is called once per frame
    void Update()
    {
        if (allEnemyPower > allEnemyPower)
        {
            armyTimeAction += Time.deltaTime / 2;
        }
        else
        {
            armyTimeAction += Time.deltaTime;
        }

        cityTimeAction += Time.deltaTime / (allEnemiesCityList.Count + 1);


        if (cityTimeAction > timeToGetCityActions )
        {
            //city actions

            SetToDoCityes();


            AiCitiesAction();
             



           


            cityTimeAction = 0;
        }

        if (armyTimeAction > timeToGetActions)
        {
            if (actionArmyQueueArray.Count > 0)
            {
                if (actionArmyQueueArray[0] != null)
                {
                    AiArmyAction(actionArmyQueueArray[0]);

                    if (actionArmyQueueArray.Count > 0)
                    {
                        actionArmyQueueArray.RemoveAt(0);
                    }
                }
                else
                {
                    actionArmyQueueArray.RemoveAt(0);
                }
            }

            //actions ++;
            armyTimeAction = 0;

            if (actionArmyQueueArray.Count == 0)
            {



                SetArmyQueue();
                SetAllEnemiesList();
                SetAllPlayersList();
                SetAllPlayersCityList();
                SetAllEnemyCityList();


            }

           
        }

       


    }

    public void AiCitiesAction()
    {
        CityController bestCityToSpawn = null;

        float addPowerCoef = 1;

        if(allEnemiesCityList.Count < allPlayerCityList.Count)
        {
            addPowerCoef += 0.5f;
        }

        if (allEnemiesCityList.Count == 0)
        {
            addPowerCoef += 1f;
        }

        if (allEnemyPower + 0f < allPlayerPower  + (50f * addPowerCoef)) { // 1 насколько нам похуй на 2 насколько нам надо быть больше

           

            for (int i = 0; i < actionCityQueueArray.Count; i++)
            {
                if (actionCityQueueArray[i].armyInCity != null)
                {
                    if (actionCityQueueArray[i].armyInCity.armyPower < allPlayerPower / 5)
                    {
                        if (actionCityQueueArray[cityIndex].cityUnits > 50f)
                        {
                            bestCityToSpawn = actionCityQueueArray[i];
                            i = actionCityQueueArray.Count;
                        }
                    }
                }
            }

            if(bestCityToSpawn == null)
            {
                bestCityToSpawn = actionCityQueueArray[Random.Range(0, actionCityQueueArray.Count)] ;
            }

            if (enemyCoins > 50f && bestCityToSpawn.cityUnits > 50f)
            {
                cityActionIndex = Random.Range(1, 3);

            }
            else
            {
                if (enemyCoins > 100)
                {
                    bestCityToSpawn = actionCityQueueArray[Random.Range(0, actionCityQueueArray.Count)];
                    cityActionIndex = -1;
                }
                else
                {
                    cityActionIndex = 0;
                }
            }
        }
        else
        {
            if (enemyCoins > 100)
            {
                bestCityToSpawn = actionCityQueueArray[Random.Range(0, actionCityQueueArray.Count)];
                cityActionIndex = -1;
            }
            else
            {
                cityActionIndex = 0;
            }
            
        }
            


                if (cityActionIndex == 0)// ничего не делаем
                {
                    

                }else
                if (cityActionIndex == -1)// делаем дом
                {
                    if (enemyCoins > 10)
                    {
                bestCityToSpawn.toDoController.AiDoHouse();
                        enemyCoins -= 10f;

                       
                       
                    }
               
            }
                else
                if (cityActionIndex == 1 )// делаем отряд 0
                {

                    if (enemyCoins > 50 && bestCityToSpawn.cityUnits > 50)
                    {
                bestCityToSpawn.toDoController.AiToDoSquad(0);

                        enemyCoins -= 50f;
                bestCityToSpawn.cityUnits -= 50;

                      
                       
                    }
               
            }
            else
                if (cityActionIndex == 2)// делаем отряд 1
            {
                if (enemyCoins > 50f && bestCityToSpawn.cityUnits > 50f)
                {
                bestCityToSpawn.toDoController.AiToDoSquad( 1);

                    enemyCoins -= 50f;
                bestCityToSpawn.cityUnits -= 50f;

                   
                   
                }
                

            }
            else
                if (cityActionIndex == 3 )// делаем отряд 2
            {
                if (enemyCoins > 50f && bestCityToSpawn.cityUnits > 50f)
                {
                bestCityToSpawn.toDoController.AiToDoSquad(2);

                    enemyCoins -= 50f;
                bestCityToSpawn.cityUnits -= 50f;

                    
                }

                
                }
            else
            {
               
            }





        


        

    }



    public void SetAttackNearPriority(ArmyController army)
    {

        float lastPowerToAttack = -2000f;
        int bestIndexToAttack = 0;

        if (playerNearArmies.Count > 0) //  1 - бежим в атаку на конкретный отряда - если он близко!
        {


            for (int i = 0; i < playerNearArmies.Count; i++)
            {
                // вычесляем лучшего для аттаки - ан вы
                float p = army.armyPower - playerNearArmies[i].armyPower;
                if (p > lastPowerToAttack)
                {
                    lastPowerToAttack = p;
                    bestIndexToAttack = i;
                }


               


            }
            bestArmyToAttack = playerNearArmies[bestIndexToAttack];

            if (bestArmyToAttack.armyPower*0.8f < army.armyPower)
            {
                armyActionPriority[1] += 2;
            }


            if (bestArmyToAttack.armyPower < army.armyPower)
            {
                armyActionPriority[1] += 3;
            }

            if (bestArmyToAttack.armyPower*2f < army.armyPower)
            {
                armyActionPriority[1] += 5;// 
            }

            if (bestArmyToAttack.armyMorale < army.armyMorale)
            {
                armyActionPriority[1] += 1;//  
            }
            if (bestArmyToAttack.squadList.Count < army.squadList.Count)
            {
                armyActionPriority[1] += 2;//  
            }





        }
    }

    public void SetRetreatPriority(ArmyController army)
    {
        if (playerNearArmies.Count > 0) //  4- отходим
        {
            float p = 0f;

            int armyIndex = 0;

            for (int i = 0; i < playerNearArmies.Count; i++)
            {


                if (p < playerNearArmies[i].armyPower)
                {
                    p = playerNearArmies[i].armyPower;
                    armyIndex = i;


                }




            }
            bestArmyToRetret = playerNearArmies[armyIndex];

            if (p > army.armyPower*0.8)
            {
                armyActionPriority[3] += 2;
            }
            if (p > army.armyPower)
            {
                armyActionPriority[3] += 3;
               
            }
            if (p > army.armyPower * 2)
            {
                armyActionPriority[3] += 5;
            }

            army.ai_needHelp = true;


            if (army.inCity && armyActionPriority[3] <= 5) /// если уже в городе - хуйня идея
            {
                armyActionPriority[3] = 0;
                armyActionPriority[0] += 10f; // оставаться на месте получается лучше +10
            }
            else {


                if (allEnemiesCityList.Count > 0)
                {
                    bestCityToGo = null;

                    if (armyActionPriority[3] > 0)
                    {

                        for (int i = 0; i < allEnemiesCityList.Count; i++)
                        {
                            if (army.inCity && army.city != allEnemiesCityList[i])
                            {

                                float dist = Vector3.Distance(allEnemiesCityList[i].transform.position, army.transform.position);

                                //if (dist < 30f)
                                // {
                                bestCityToGo = allEnemiesCityList[i];
                                i = allEnemiesCityList.Count;
                                //}

                            }

                        }
                        if (bestCityToGo != null)
                        {

                            armyActionPriority[3] = 0;
                            armyActionPriority[4] = 10;
                        }




                    }
                }



            }

        }
    }

    public void SetReinforcementPriority(ArmyController army)
    {
        if (SceneLoader.Instance.mapBattleControllers.Count > 0)// коенретно идем баттл помочь
        {

            bestArmyToStrategyMove = null;


            for (int i = 0; i < SceneLoader.Instance.mapBattleControllers.Count; i++)
            {
                MapBattleController battle = SceneLoader.Instance.mapBattleControllers[i];

                
                if ( battle.playerArmy.armyPower > battle.enemyArmy.armyPower )
                {


                    bestArmyToStrategyMove = battle.enemyArmy;


                    armyActionPriority[11] = 10;


                }
                else if(battle.playerArmy.armyPower > battle.enemyArmy.armyPower-150f)
                {
                    bestArmyToStrategyMove = battle.enemyArmy;


                    armyActionPriority[11] = 5;
                }



            }

            if (bestArmyToStrategyMove != null)
            {
               
            }
        }
    }

    public void SetAttackCityPriority(ArmyController army, bool power, bool distance)
    {
        if (power)
        {
            float lastPowerToAttackCity = 0;
            bestCityToAttack = null;


            for (int i = 0; i < allPlayerCityList.Count; i++)
            {


                float p = -1000f;

                if (allPlayerCityList[i].armyInCity != null)
                {
                    p = army.armyPower - allPlayerCityList[i].armyInCity.armyPower;
                }
                else
                {
                    p = army.armyPower;
                }

                if (p > lastPowerToAttackCity)
                {
                    lastPowerToAttackCity = p;
                    bestCityToAttack = allPlayerCityList[i];
                }






            }


            if (bestCityToAttack != null)
            {
               

                armyActionPriority[7] += 10;
            }
            else
            {
                
            }

        }
        else if (distance)
        {
            bestCityToAttack = null;
            float minCityDist = 1000f;

            for (int i = 0; i < allPlayerCityList.Count; i++)
            {

                float dist = Vector3.Distance(allPlayerCityList[i].transform.position, army.transform.position);

                if (dist < minCityDist)
                {
                    minCityDist = dist;
                    bestCityToAttack = allPlayerCityList[i];
                }



            }

            if (bestCityToAttack != null)
            {
                armyActionPriority[7] += 10;
            }

        }

    }

    public void SetGoToCityPriority(ArmyController army, bool power, bool distance)
    {
        if (power)
        {
            

        }
        else if (distance)
        {
            bestCityToAttack = null;
            float minCityDist = 1000f;

            for (int i = 0; i < allEnemiesCityList.Count; i++)
            {

                float dist = Vector3.Distance(allEnemiesCityList[i].transform.position, army.transform.position);

                if (dist < minCityDist)
                {
                    minCityDist = dist;
                    bestCityToGo = allEnemiesCityList[i];
                }



            }

            if (bestCityToGo != null)
            {
                armyActionPriority[4] += 10;
            }

        }

    }



    public void AiArmyAction(ArmyController army) {

        for (int i = 0; i < armyActionPriority.Length; i++) ///// очистка
        {
            armyActionPriority[i] = 0;
        }

        if (army == null) { return; }
      

        if (army.ai_currentState == 11 // идем в бой
            || army.inBattle == true // уже в бою


         ) {
            armyTimeAction = timeToGetActions;
            return;
        } 


        // познаем обстановку вокруг оттряда
        FarPlayerArmySearch(army);
        NearPlayerArmySearch(army);
        NearEnemyArmySearch(army);
        CalculateAllPlayerPower();
        CalculateAllEnemyPower();
        NearPlayerCitySearch(army);
        DangerCitySearch();

        army.ArmyPowerUpdate();


        //////////////////
        if (army.armyPower < allPlayerPower / 5) { // слишком маленькие

            Debug.Log("army.armyPower < allPlayerPower / 5");




            SetRetreatPriority(army);

            if (!army.inCity)
            {
                SetGoToCityPriority(army, false, true);
            }


            if (enemyNearArmies.Count > 0)
            {
                bestArmyToJoint = null;
                for (int i = 0; i < enemyNearArmies.Count; i++)
                {
                    if (army != enemyNearArmies[i])
                    {
                        if (bestArmyToJoint != null)
                        {
                            if (bestArmyToJoint.armyPower > enemyNearArmies[i].armyPower)

                                bestArmyToJoint = enemyNearArmies[i];



                        }
                        else
                        {
                            bestArmyToJoint = enemyNearArmies[i];
                        }
                    }

                }

                if (bestArmyToJoint != null)
                {
                    armyActionPriority[6] = 10;
                }
            }



            

            

            

        }


        /////////
        if (army.armyPower <= allPlayerPower / 2f && army.armyPower > allPlayerPower / 5f

            &&

            allEnemyPower < allPlayerPower)
        {

            Debug.Log("army.armyPower <= allPlayerPower / 2f && army.armyPower > allPlayerPower / 5f && allEnemyPower < allPlayerPower");




            SetRetreatPriority(army);

            if(armyActionPriority[3] > 0) //есть угроза
            {



            }
            else
            {
                SetAttackNearPriority(army);
            }



            if (dangerCity.Count > 0 )
            {
                if (army.inCity && !dangerCity.Contains(army.city))
                {
                    armyActionPriority[0] += 5;
                }
                else
                {
                    for (int i = 0; i < dangerCity.Count; i++)
                    {
                        if (dangerCity[i].armyInCity != null)
                        {
                            
                                armyActionPriority[4] = 5;
                                bestCityToGo = dangerCity[i]; //на пол пути
                            
                               
                            
                        }
                    }
                }

                

            }


            SetReinforcementPriority(army);


            if (allEnemiesList.Count > 1)
            {
                for (int i = 0; i < allEnemiesList.Count; i++)
                {
                    if (allEnemiesList[i] != army)
                    {
                        if (allEnemiesList[i].ai_needHelp )
                        {

                            armyActionPriority[6] = 10;
                            bestArmyToHelp = allEnemiesList[i];
                        }
                           
                       
                       
                    }


                }
            }





            if (playerNearArmies.Count > 0)
            {
               

                if (armyActionPriority[4] < 5)
                {
                   
                }
                else
                {
                    if (enemyNearArmies.Count > 0)
                    {
                        armyActionPriority[4] = 0;
                        armyActionPriority[3] = 10;
                        bestArmyToHelp = enemyNearArmies[0];
                    }
                    else
                    {
                       
                    }
                }
            }
            else
            {
                SetAttackCityPriority(army, false, true);

            }

                    //if (allEnemiesCityList.Count > 0 && playerNearArmies.Count == 0)
                    //{
                    //    for (int i = 0; i < allEnemiesCityList.Count; i++)
                    //    {
                    //        if (allEnemiesCityList[i].armyInCity != null)
                    //        {
                    //            if (allEnemiesCityList[i].armyInCity.armyPower < allPlayerPower / 4f)
                    //            {
                    //                armyActionPriority[6] += 10;
                    //                bestCityToGo = allEnemiesCityList[i]; //на пол пути пряму в город
                    //            }
                    //        }
                    //    }

                    //}

                    //if (allEnemiesCityList.Count > 0)
                    //{
                    //    for (int i = 0; i < allEnemiesCityList.Count; i++)
                    //    {
                    //        if (allEnemiesCityList[i].is != null)
                    //        {
                    //            if (allEnemiesCityList[i].armyInCity.armyPower < allPlayerPower / 4f)
                    //            {
                    //                armyActionPriority[6] += 10;
                    //                bestCityToGo = allEnemiesCityList[i]; //на пол пути пряму в город
                    //            }
                    //        }
                    //    }

                    //}

                }


        ///////////////////////////
        if ((army.armyPower <= allPlayerPower / 2f && army.armyPower > allPlayerPower / 5f)

            &&

            allEnemyPower > allPlayerPower)
        {


           
            Debug.Log("(army.armyPower <= allPlayerPower / 2f && army.armyPower > allPlayerPower / 5f) && allEnemyPower > allPlayerPower");

            if (allPlayerCityList.Count > 0 && playerNearArmies.Count == 0)  // 7 - атакуем вражеский город 
            {

                SetAttackCityPriority(army, true, false);
            }
            else if( playerNearArmies.Count > 0)
            {
                SetRetreatPriority(army);

                if(armyActionPriority[4] < 5)
                {
                    SetAttackNearPriority(army);
                }
                else
                {
                    if (enemyNearArmies.Count > 0)
                    {
                        armyActionPriority[4] = 0;
                        armyActionPriority[3] = 10;
                        bestArmyToHelp = enemyNearArmies[0];
                    }

                }
            }

            SetReinforcementPriority(army);



        }

        if (army.armyPower > allPlayerPower / 2f )
        {
            Debug.Log("army.armyPower > allPlayerPower / 2f");
            SetRetreatPriority(army);
            SetAttackNearPriority(army);

            //if (army.isMoved)
            //{
            //    if (army.armyMorale / army.armyMoraleMax < 0.30)
            //    {
            //        armyActionPriority[0] = 20; // останавливаемся бо нет морали
            //    }
            //}

            if (armyActionPriority[1] == 0)
            {
                if (allPlayerCityList.Count > 0)
                {


                    SetAttackCityPriority(army, false, true);
                }
                else if (allPlayerList.Count > 0)
                {
                    bestArmyToAttack = null;
                    float minArmyDist = 1000f;

                    for (int i = 0; i < allPlayerList.Count; i++)
                    {

                        float dist = Vector3.Distance(allPlayerList[i].transform.position, army.transform.position);

                        if (dist < minArmyDist)
                        {
                            minArmyDist = dist;
                            bestArmyToAttack = allPlayerList[i];
                        }



                    }

                    if (bestCityToAttack != null)
                    {
                        armyActionPriority[1] += 10;
                    }
                }

            }


        }





        ///------ финальное решение -------
       


        float lastPriority = -10f;
        int bestAction = 0;
        for (int i = 0; i < armyActionPriority.Length; i++)  ///------ ищим самое выгодное действие
        {
            if (armyActionPriority[i] > lastPriority)
            {
                lastPriority = armyActionPriority[i];
                bestAction = i;
            }
        }

        

        if(bestAction == army.ai_currentState)
        {
            //return;
        }



        if (bestAction == 0) // бездействуем
        {
            army.ai_currentState = 0;

            army.SetMoving(false);
            army.SetFormation(army.moveDirection);
            return;
        }


        if (bestAction == 1) // 1 - идем на вражескую армию - если она близко
        {
            army.ai_currentState = 1;
            DrawPathAndGo(army, bestArmyToAttack.transform.position,null);

          
            return;
        }
       
        if (bestAction == 2)     // 2 - идем в баттл
        {

            army.ai_currentState = 2;
            DrawPathAndGo(army, bestArmyToHelp.transform.position, null);

           

           
            return;
        }
        if (bestAction == 3)  // 3 - отходим
        {
           


            Vector3 backVector;

            if (bestArmyToRetret != null)
            {
                backVector = Vector3.Normalize(army.transform.position - bestArmyToRetret.transform.position);
                DrawPathAndGo(army, army.transform.position + (backVector * nearRadius), null);
                army.ai_currentState = 3;
            }
            else
            {
                return;
            }

          
          
        }
        if (bestAction == 4) //4 - идем город
        {
            army.ai_currentState = 4;
            army.goToCity = true;
            DrawPathAndGo(army, bestCityToGo.transform.position, null);
        }
       
        if (bestAction == 5) // 5 - атакуем вражеский город 
        {
            army.ai_currentState = 5;

            if (Vector3.Distance(army.transform.position, bestCityToAttack.transform.position) > 30f)
            {

                DrawHalfPathAndGo(army, bestCityToAttack.transform.position);
            }
            else
            {
                DrawPathAndGo(army, bestCityToAttack.transform.position, null);
            }
        }

        if (bestAction == 6) //6 - cоединяемся с армией
        {
            army.ai_currentState = 6;
            army.goToJoint  = true;
            DrawPathAndGo(army, bestArmyToJoint.transform.position, null);
        }






        if (bestAction == 11)  // 11[strategy move] - отправка микро подкрепа потому что надо 
        {
            if (army.ai_currentState != bestAction)
            {
                army.ai_currentState = 11;

                if (bestArmyToStrategyMove.inBattle) {


                    for (int i = 0; i < SceneLoader.Instance.mapBattleControllers.Count; i++)
                    {
                        if(SceneLoader.Instance.mapBattleControllers[i].enemyArmy == bestArmyToStrategyMove) {
                            DrawPathAndGo(army, SceneLoader.Instance.mapBattleControllers[i].transform.position, null);
                        }


                    }

                   
                }
                else
                {
                    //DrawPathAndGo(army, bestArmyToStrategyMove.transform.position, null);
                }
                
            }
        }


    }

    public void CalculateAllPlayerPower() 
    {
        float power = 0;
        for (int i = 0; i < allPlayerList.Count; i++)
        {


            power += allPlayerList[i].armyPower;
        }
        allPlayerPower = power; /// allPlayerList.Count;
        
    }

    public void CalculateAllEnemyPower() 
    {
        float power = 0;
        for (int i = 0; i < allEnemiesList.Count; i++)
        {


            power += allEnemiesList[i].armyPower;
        }
        allEnemyPower = power; /// allEnemiesList.Count;

    }

    public void SetArmyQueue()
    {

        actionArmyQueueArray = new List<ArmyController>();


        for (int i = 0; i < mapSceneManager.enemiesArmiesObject.transform.childCount; i++)
        {
            actionArmyQueueArray.Add(mapSceneManager.enemiesArmiesObject.transform.GetChild(i).GetComponent<ArmyController>());
        }


    }

    public void SetAllEnemiesList()
    {

        allEnemiesList = new List<ArmyController>();



        for (int i = 0; i < mapSceneManager.enemiesArmiesObject.transform.childCount; i++)
        {
            if (mapSceneManager.enemiesArmiesObject.transform.GetChild(i).gameObject.activeSelf)
            {
                allEnemiesList.Add(mapSceneManager.enemiesArmiesObject.transform.GetChild(i).GetComponent<ArmyController>());
            }
        }


     

    }

    public void SetAllEnemyCityList()
    {

        allEnemiesCityList = new List<CityController>();



        for (int i = 0; i < mapSceneManager.citiesObject.transform.childCount; i++)
        {
            GameObject city = mapSceneManager.citiesObject.transform.GetChild(i).gameObject;
            if (city.activeSelf && city.tag == "Enemy")
            {
                allEnemiesCityList.Add(mapSceneManager.citiesObject.transform.GetChild(i).GetComponent<CityController>());
            }
        }


    }

    public void SetToDoCityes()
    {
        actionCityQueueArray = new List<CityController>();

        for (int i = 0; i < mapSceneManager.citiesObject.transform.childCount; i++)
        {
            GameObject city = mapSceneManager.citiesObject.transform.GetChild(i).gameObject;
            if (city.activeSelf && city.tag == "Enemy" )
            {
                CityController cityController = mapSceneManager.citiesObject.transform.GetChild(i).GetComponent<CityController>();

                if (!cityController.isSmallCity)
                {

                    actionCityQueueArray.Add(mapSceneManager.citiesObject.transform.GetChild(i).GetComponent<CityController>());
                }
            }
        }

    }

    public void SetAllPlayersList()
    {

        allPlayerList = new List<ArmyController>();

        for (int i = 0; i < mapSceneManager.playerArmiesObject.transform.childCount; i++)
        {
            if (mapSceneManager.playerArmiesObject.transform.GetChild(i).gameObject.activeSelf)
            {
                allPlayerList.Add(mapSceneManager.playerArmiesObject.transform.GetChild(i).GetComponent<ArmyController>());
            }

        }


    }

    public void SetAllPlayersCityList()
    {

        allPlayerCityList = new List<CityController>();



        for (int i = 0; i < mapSceneManager.citiesObject.transform.childCount; i++)
        {
            GameObject city = mapSceneManager.citiesObject.transform.GetChild(i).gameObject;
            if (city.activeSelf && city.tag == "Player")
            {
                allPlayerCityList.Add(mapSceneManager.citiesObject.transform.GetChild(i).GetComponent<CityController>());
            }
        }


    }




    public void NearPlayerCitySearch(ArmyController army)
    {
        Collider[] nearColliders = Physics.OverlapSphere(army.transform.position, nearRadius, playerCityLayer);

        nearPlayerCityList = new List<CityController>();

        for (int i = 0; i < nearColliders.Length; i++)
        {
            if (nearColliders[i].tag == "Player")
            {
                CityController city = nearColliders[i].GetComponent<CityController>();

                if (!nearPlayerCityList.Contains(city))
                {
                    nearPlayerCityList.Add(city);




                }
            }

        }


    }
    public void DangerCitySearch()
    {
       

        dangerCity = new List<CityController>();

        for (int i = 0; i < allEnemiesCityList.Count; i++)
        {

            Collider[] nearColliders = Physics.OverlapSphere(allEnemiesCityList[i].transform.position, nearRadius, playerArmyLayer);

            if (nearColliders.Length > 0)
                {
                dangerCity.Add(allEnemiesCityList[i]);




                }
            

        }


    }


    public void FarPlayerArmySearch(ArmyController army)
{

    Collider[] nearColliders = Physics.OverlapSphere(army.transform.position, farRadius, playerArmyLayer);

    playerFarArmies = new List<ArmyController>();

    for (int i = 0; i < nearColliders.Length; i++)
    {
        if (nearColliders[i].tag == "Player")
        {
                ArmyController newArmy = nearColliders[i].GetComponent<ArmyController>();

            if (!playerFarArmies.Contains(newArmy))
            {
                    playerFarArmies.Add(newArmy);




            }
        }

    }

}

    public void NearPlayerArmySearch(ArmyController army)
    {

        Collider[] nearColliders = Physics.OverlapSphere(army.transform.position, nearRadius, playerArmyLayer);

        playerNearArmies = new List<ArmyController>();

        for (int i = 0; i < nearColliders.Length; i++)
        {
            if (nearColliders[i].tag == "Player")
            {
                ArmyController newArmy = nearColliders[i].GetComponent<ArmyController>();

                if (!playerNearArmies.Contains(newArmy) && !newArmy.inBattle)
                {
                    playerNearArmies.Add(newArmy);




                }
            }

        }

    }

    public void NearEnemyArmySearch(ArmyController army)
    {

        Collider[] nearColliders = Physics.OverlapSphere(army.transform.position, nearRadius, enemyArmyLayer);

        enemyNearArmies = new List<ArmyController>();

        for (int i = 0; i < nearColliders.Length; i++)
        {
            if (nearColliders[i].tag == "Enemy")
            {
                ArmyController newArmy = nearColliders[i].GetComponent<ArmyController>();

                if (!enemyNearArmies.Contains(newArmy) && !newArmy.inBattle)
                {
                    enemyNearArmies.Add(newArmy);




                }
            }

        }

    }



    private void DrawPathAndGo(ArmyController army, Vector3 targetPos, ArmyController jointArmy)
{
        army.targetObject.transform.position = targetPos;
        army.SetMoving(true);

        if(jointArmy != null)
        {
            army.goToJoint = true;
            army.jointArmy = jointArmy;
        }
    }

    private void DrawHalfPathAndGo(ArmyController army, Vector3 targetPos)
    {


        Vector3 dir = (targetPos - army.transform.position).normalized;
        float distance = 25f * army.armyMorale/army.armyMoraleMax; // сколько шагнуть
        Vector3 between = army.transform.position + dir * distance;

        army.targetObject.transform.position = between;
        army.SetMoving(true);
        Debug.Log("DrawHalfPathAndGo = "+ between);
    }


    public void ArmyRetreat(ArmyController army,Vector3 center) {

    Vector3 dir = Vector3.Normalize(center - army.transform.position);
    
    //SquadHalfWayToPoint(enemySquad,center-dir*nearRadius*2f);

}


    public void DeleteArmyFromQueue(ArmyController army)
    {

        if (actionArmyQueueArray.Contains(army))
        {
            actionArmyQueueArray.Remove(army);
        }




    }






   
}
