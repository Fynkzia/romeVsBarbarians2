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
    // 1 - идем на вражескую армию - если она близко
    // 2 - подтягиваесмя к выгодной цели - типа на пол пути
    // 3 - помагаем союзной армии рядом если она в бою и проигрывает
    // 4 - отходим
    // 5 - отсупаем в город потому что страшно
    // 6 - идем в город - выгодно
    // 7 - атакуем вражеский город
    // 8 - нет рядом городов  - подходим к выгодному городу
    // 9 - соединяемся с другой армией потому что выгодно
    // 10[strategy move] - идем стратегическую атаку ()
    // 11[strategy move] - отправка микро подкрепа потому что надо 
    // 12[strategy move] - создание армии







    [Space(10)]

[SerializeField] public List<ArmyController> actionArmyQueueArray;
    [SerializeField] public List<CityController> actionCityQueueArray;

    [SerializeField] public List<ArmyController> allEnemiesList;
    [SerializeField] public List<ArmyController> allPlayerList;

    [SerializeField] public List<CityController> allEnemiesCityList;
    [SerializeField] public List<CityController> allPlayerCityList;

    [SerializeField] public List<CityController> nearPlayerCityList;


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
    [SerializeField] public CityController bestCityToAttackFromFar;


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
    [SerializeField] public float enemyAddCoins;
    [SerializeField] public float enemyAddCoinsPerCity;

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
        armyTimeAction += Time.deltaTime;
        cityTimeAction += Time.deltaTime;


        if (cityTimeAction > timeToGetCityActions *( allEnemiesCityList.Count+1))
        {
            //city actions
            if (allEnemiesCityList.Count == 0)
            {
                SetAllEnemyCityList();

            }
            AiCitiesAction();
             



            if (allPlayerCityList.Count == 0) { 
                SetAllPlayersCityList();
             }


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

                
                  
            }

           
        }

       


    }

    public void AiCitiesAction()
    {
        

            


                if (cityActionPatern[cityActionIndex] == 0)// ничего не делаем
                {
                    cityActionIndex++;
                    cityIndex++;

                }else
                if (cityActionPatern[cityActionIndex] == -1)// делаем дом
                {
                    if (enemyCoins > 10)
                    {
                        allEnemiesCityList[cityIndex].toDoController.AiDoHouse();
                        enemyCoins -= 10f;

                       
                        cityIndex++;
                    }
                cityActionIndex++;
            }
                else
                if (cityActionPatern[cityActionIndex] == 1 && allEnemyPower < allPlayerPower * 2)// делаем отряд 0
                {

                    if (enemyCoins > 50 && allEnemiesCityList[cityIndex].cityUnits > 50)
                    {
                        allEnemiesCityList[cityIndex].toDoController.AiToDoSquad(0);

                        enemyCoins -= 50f;
                    allEnemiesCityList[cityIndex].cityUnits -= 50;

                      
                        cityIndex++;
                    }
                cityActionIndex++;
            }
            else
                if (cityActionPatern[cityActionIndex] == 2 && allEnemyPower < allPlayerPower * 2)// делаем отряд 1
            {
                if (enemyCoins > 50f && allEnemiesCityList[cityIndex].cityUnits > 50f)
                {
                    allEnemiesCityList[cityIndex].toDoController.AiToDoSquad( 1);

                    enemyCoins -= 50f;
                    allEnemiesCityList[cityIndex].cityUnits -= 50f;

                   
                    cityIndex++;
                }
                cityActionIndex++;

            }
            else
                if (cityActionPatern[cityActionIndex] == 3 && allEnemyPower < allPlayerPower * 2)// делаем отряд 2
            {
                if (enemyCoins > 50f && allEnemiesCityList[cityIndex].cityUnits > 50f)
                {
                    allEnemiesCityList[cityIndex].toDoController.AiToDoSquad(2);

                    enemyCoins -= 50f;
                    allEnemiesCityList[cityIndex].cityUnits -= 50f;

                    cityIndex++;
                }

                 cityActionIndex++;
                }
            else
            {
                cityActionIndex++;
            }





        


        if (cityIndex >= allEnemiesCityList.Count)
        {
            cityIndex = 0;

            
        }
        if (cityActionIndex >= cityActionPatern.Length)
        {
            cityActionIndex = 0;
        }

        enemyCoins += enemyAddCoins; //после того как все города получат что делать -
        enemyCoins += enemyAddCoinsPerCity * allEnemiesCityList.Count;

    }


    public void AiArmyAction(ArmyController army) {

        for (int i = 0; i < armyActionPriority.Length; i++) ///// очистка
        {
            armyActionPriority[i] = 0;
        }

        if (army == null) { return; }
        if (army.inBattle == true) { return; }

        // познаем обстановку вокруг оттряда
        FarPlayerArmySearch(army);
        NearPlayerArmySearch(army);
        NearEnemyArmySearch(army);
        CalculateAllPlayerPower();
        CalculateAllEnemyPower();
        NearPlayerCitySearch(army);
        // NearEnemySquadList(squad);

        army.ArmyPowerUpdate();

        if (army.armyPower < allPlayerPower / 4) { } // слишком маленькие

        
            //10 - идем в стратегическую атаку - 
            if (allEnemyPower > allPlayerPower/1.2 && //павера больше чем у игрока - 20%
                (army.armyPower > allPlayerPower / 5) &&
                army.armyMorale / army.armyMoraleMax > 0.6f
                ) 
            {


                    float lastPowerToAttackStrategy = -2000f;



                    if (allPlayerList.Count > 0)   //10 - нападаем на самый слабый отряд игрока
                    {


                        for (int i = 0; i < allPlayerList.Count; i++)
                        {
                            // вычесляем лучшего для аттаки 
                            float p = army.armyPower - allPlayerList[i].armyPower;
                            if (p > lastPowerToAttackStrategy)
                            {
                                lastPowerToAttackStrategy = p;
                                bestArmyToStrategyAttack = allPlayerList[i];
                            }




                        }
                        if (bestArmyToStrategyAttack != null)
                        {
                            armyActionPriority[10] += 6;
                        }

                    }

                

            }


        //11 - микро подкреп
        if (allEnemyPower > allPlayerPower / 2 && //павера больше чем у игрока - 50%
           (army.armyPower > allPlayerPower / 5 && army.armyPower < allPlayerPower / 2) &&
           army.armyMorale / army.armyMoraleMax > 0.6f
           )
        {

            bestArmyToStrategyMove = null;
            if (SceneLoader.Instance.mapBattleControllers.Count > 0)// коенретно идем баттл помочь
            {




                for (int i = 0; i < SceneLoader.Instance.mapBattleControllers.Count; i++)
                {
                    MapBattleController battle = SceneLoader.Instance.mapBattleControllers[i];

                    if (battle.enemyArmy.armyPower < battle.playerArmy.armyPower)
                    {
                        bestArmyToStrategyMove = battle.enemyArmy;



                    }



                }
            }

            if (bestArmyToStrategyMove != null)
            {
                armyActionPriority[11] += 7;
            }
            else if(army.ai_currentState != 11) // если уже идет подкруп - пропускаем
            {
                // отсылаем микро подкреп

                for (int i = 0; i < allEnemiesList.Count; i++)
                {
                    if (army != allEnemiesList[i] && allEnemiesList[i].armyPower < allPlayerPower / 1.2)
                    {
                        if (bestArmyToStrategyMove != null)
                        {
                            if (bestArmyToStrategyMove.armyPower > allEnemiesList[i].armyPower)

                                bestArmyToStrategyMove = allEnemiesList[i];



                        }
                        else
                        {
                            bestArmyToStrategyMove = allEnemiesList[i];
                        }

                    }


                }

                if (bestArmyToStrategyMove != null)
                {
                    armyActionPriority[11] += 7;
                }
            }
        }
            

        

        if (army.armyPower < allPlayerPower/2) //9- соединяемся с другой БЛИЖАЙШЕЙ армией когда у обоих меньше павера чем 2 раза от игрока
        {

            bestArmyToJoint = null;
            for (int i = 0; i < enemyNearArmies.Count; i++)
            {
                if (army != enemyNearArmies[i] && enemyNearArmies[i].armyPower < allPlayerPower/1.5)
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
                armyActionPriority[9] += 7;
            }
        }
        else if (army.armyPower < allPlayerPower)
        {
            if (enemyNearArmies.Count > 0)
            {
                armyActionPriority[0] += 1;
            }// рядом с союзниками стоять комфортно, но не сильно
        }


           

        armyActionPriority[0] += (1- army.armyMorale/army.armyMoraleMax)*20f; // мало морали - лучще постоять

        if (army.inCity) // в городе стоять круто++
        {
            armyActionPriority[0] += 1;

            if(army.armyPower < allPlayerPower/ allPlayerList.Count)
            {
                armyActionPriority[0] += 3;

                if (playerNearArmies.Count > 0)
                {
                    armyActionPriority[0] += playerNearArmies.Count;
                }
            }
        }

       

        float lastPowerToAttack = -2000f;
        int bestIndexToAttack = 0;
        int countToAttck = 0; // сока выгодных целей

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


                if (p > 0)// +1 за каждую армию с меньшим павером
                {
                    armyActionPriority[1]++; 
                }


            }
            bestArmyToAttack = playerNearArmies[bestIndexToAttack];

            armyActionPriority[1] += lastPowerToAttack/100f;//  чем больше разница паверов тем больше хочется напасть +1 за каждые 100;

            armyActionPriority[1] += (1 - bestArmyToAttack.armyMorale/bestArmyToAttack.armyMoraleMax) * 2; // если у армии морали 5

            // тут можно вписать сложность



        }

        if (army == null) { return; }


        if (playerFarArmies.Count > 0 && playerNearArmies.Count == 0) //  2 - подтягиваесмя к выгодной цели - типа на пол пути
        {


            for (int i = 0; i < playerFarArmies.Count; i++)
            {
                float p = army.armyPower - playerFarArmies[i].armyPower;
                if (p > lastPowerToAttack)
                {
                    lastPowerToAttack = p;
                    bestIndexToAttack = i;
                }
                if (p > 0)
                {
                    armyActionPriority[2]++;
                }


            }

            bestArmyToAttack = playerFarArmies[bestIndexToAttack];

         
            armyActionPriority[2] += (lastPowerToAttack / 50f) *2;//  чем больше разница паверов тем больше хочется напасть +1 за каждые 100;




        }

       

        float lastPowerToHelp = 0;
        int bestIndexToHelp = 0;
        

        if (enemyNearArmies.Count > 0) // 3 - помагаем союзной армии в бою, если она рядом
        {

            bestArmyToHelp = null;

            for (int i = 0; i < enemyNearArmies.Count; i++)
            {
                if (enemyNearArmies[i].inBattle)
                {
                    //if(bestArmyToHelp != null)
                    //{
                    //    if(bestArmyToHelp.armyPower > enemyNearArmies[i].armyPower)
                    //    {
                    //        bestArmyToHelp = enemyNearArmies[i];
                    //    }
                    //}
                    //else
                    //{
                    //    if(allEnemiesList[i].)
                    //    bestArmyToHelp = allEnemiesList[i];
                    //}

                    float p = enemyNearArmies[i].enemyArmy.armyPower - enemyNearArmies[i].armyPower;
                    

                    if (p > lastPowerToHelp)
                    {
                        lastPowerToHelp = p ;
                        bestIndexToHelp = i;

                       
                    }
                   
                }

            }

            bestArmyToHelp = enemyNearArmies[bestIndexToHelp];


            armyActionPriority[3] += lastPowerToHelp / 100f;

            //float p2 = (army.armyPower + bestArmyToHelp.armyPower) - enemyNearArmies[i].enemyArmy.armyPower;

           // armyActionPriority[3] += lastPowerToHelp / 100f;

        }
        float lastPowerToRetret = -2000f;
        int bestIndexToRetret = 0;

      

        if (playerNearArmies.Count > 0) //  4- отходим
        {


            for (int i = 0; i < playerNearArmies.Count; i++)
            {
              
                float p = army.armyPower - playerNearArmies[i].armyPower;
                if (p < lastPowerToRetret)
                {
                    lastPowerToRetret = p;
                    bestIndexToRetret = i;
                }

               


            }
            bestArmyToRetret = playerNearArmies[bestIndexToRetret];

            
            armyActionPriority[4] += -lastPowerToRetret/50f;//  чем больше разница паверов тем больше хочется свалить от них подальше +1 за каждые 100;

            if (army.inCity) /// если уже в городе - хуйня идея
            {
                armyActionPriority[4] = 0;
                armyActionPriority[0] += 7f; // оставаться на месте получается лучше +10
            }

        }

        if (allEnemiesCityList.Count > 0) 
        {

            if (armyActionPriority[4] > 0) {     // 5 - идем город бо страшно
                 float minDistanceToCity = 1000f;

                for (int i = 0; i < allEnemiesCityList.Count; i++)
                {
                   
                        float dist = Vector3.Distance(allEnemiesCityList[i].transform.position, army.transform.position);

                        if (dist < minDistanceToCity)
                        {
                            bestCityToRetret = allEnemiesCityList[i];
                            minDistanceToCity = dist;
                        }
                    


                }

                armyActionPriority[5] += -lastPowerToRetret / 50f;


                if(minDistanceToCity <= 50f)
                {
                    armyActionPriority[5] += 5f;//заходим в город
                }

                if (minDistanceToCity <= 100f)
                {
                    armyActionPriority[5] += 2f;//заходим в город
                }


            }

            if (army.inCity) /// если уже в городе - хуйня идея
            {
                armyActionPriority[5] = 0;
            }


            // 6 - идем в город бо выгодно


            //if (army.armyMorale/ army.armyMoraleMax < 0.6f)// 6 - идем в город бо выгодно
            //{
            //    armyActionPriority[6] += (1f - (army.armyMorale / army.armyMoraleMax)) * 2f;
            //}

            if (army.armyUnits / army.armyUnitsMax < 0.6f)
            {
                armyActionPriority[6] += (1f - (army.armyUnits / army.armyUnitsMax)) * 10f;
            }

           
            if (army.armyPower < allPlayerPower/allPlayerList.Count)
            {
                armyActionPriority[6] += 3f;

                
            }

            if (army.armyPower < allPlayerPower / 5)
            {
                armyActionPriority[6] += 5f;


            }


            if (armyActionPriority[6] > 0)   //идем в город бо выгодно
            {   
                float minDistanceToCity = 1000f;

                for (int i = 0; i < allEnemiesCityList.Count; i++) // ближайший город
                {
                    
                        float dist = Vector3.Distance(allEnemiesCityList[i].transform.position, army.transform.position);

                        if (dist < minDistanceToCity)
                        {
                            bestCityToGo = allEnemiesCityList[i];
                            minDistanceToCity = dist;
                        }
                    


                }

            if (bestCityToGo.armyInCity != null)
            {
                if (bestCityToGo.armyInCity.armyPower > army.armyPower)
                {
                    armyActionPriority[6] += 2f;
                }

                    

                    if (playerNearArmies.Count > 0) 
                {
                    armyActionPriority[6] += 4f;
                }
            }
            else
            {


            }

                if (army.inCity) /// если уже в городе - хуйня идея
                {
                    armyActionPriority[6] = 0;
                }

            }

            if (nearPlayerCityList.Count > 0)  // 7 - атакуем вражеский город 
            {

                float lastPowerToAttackCity = 0;



                for (int i = 0; i < nearPlayerCityList.Count; i++)
                {
                    if (nearPlayerCityList[i].armyInCity == null) // сразу тригиримся на пустой город
                    {
                        bestCityToAttack = nearPlayerCityList[i];
                        armyActionPriority[7] += 5f;


                        i = nearPlayerCityList.Count;

                    }
                    else
                    {
                        float p = army.armyPower - nearPlayerCityList[i].armyInCity.armyPower;
                        if (p > lastPowerToAttackCity)
                        {
                            lastPowerToAttackCity = p;
                            bestCityToAttack = nearPlayerCityList[i];
                        }



                    }


                }
                armyActionPriority[7] += lastPowerToAttackCity / 50f;

                if (army.armyPower > allPlayerPower)
                {
                    armyActionPriority[7] += 10f;
                }

                if (army.armyPower > allPlayerPower/allPlayerList.Count)
                {
                    armyActionPriority[7] += 3f;
                }

            }
            else // нет рядом городов  - подходим к выгодному городу
            {
                float lastPowerToAttackCity = 0;
                if (army.ai_currentState != 8)
                {


                    for (int i = 0; i < allPlayerCityList.Count; i++)
                    {
                        if (allPlayerCityList[i].armyInCity == null) // сразу тригиримся на пустой город
                        {
                            bestCityToAttackFromFar = allPlayerCityList[i];
                            armyActionPriority[8] += 5f;


                            i = allPlayerCityList.Count;

                        }
                        else
                        {
                            float p = army.armyPower - allPlayerCityList[i].armyInCity.armyPower;
                            if (p > lastPowerToAttackCity)
                            {
                                lastPowerToAttackCity = p;
                                bestCityToAttackFromFar = allPlayerCityList[i];
                            }



                        }


                    }
                    armyActionPriority[8] += lastPowerToAttackCity / 50f;

                    if (army.armyPower > allPlayerPower/allPlayerList.Count)
                    {
                        armyActionPriority[8] += 3f;

                        if (bestCityToAttackFromFar == null)
                        {
                            bestCityToAttackFromFar = allPlayerCityList[0];
                        }
                    }


                }
            }

        }



        ///------ финальное решение -------
        // 0 - стоим, ничего не делаем
        // 1 - идем на вражескую армию - если она близко
        // 2 - подтягиваесмя к выгодной цели - типа на пол пути
        // 3 - помагаем союзной армии
        // 4 - отходим


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
            return;
        }


        if (bestAction == 1) // 1 - идем на вражескую армию - если она близко
        {
            army.ai_currentState = 1;
            DrawPathAndGo(army, bestArmyToAttack.transform.position,null);

          
            return;
        }
        if (bestAction == 2)   // 2 - подтягиваесмя к выгодной цели - типа на пол пути
        {
            army.ai_currentState = 2;
            DrawHalfPathAndGo(army, bestArmyToAttack.transform.position); // пол пути задается делителем 

            return;
        }
        if (bestAction == 3)     // 3 - помагаем союзной армии в бою
        {

            army.ai_currentState = 3;
            DrawPathAndGo(army, bestArmyToHelp.transform.position, null);

           

           
            return;
        }
        if (bestAction == 4)  // 4 - отходим
        {
           


            Vector3 backVector;

            if (bestArmyToRetret != null)
            {
                backVector = Vector3.Normalize(army.transform.position - bestArmyToRetret.transform.position);
                DrawPathAndGo(army, army.transform.position + (backVector * nearRadius), null);
                army.ai_currentState = 4;
            }
            else
            {
                return;
            }

          
          
           // return;
        }
        if (bestAction == 5) //5 - идем город бо страшно
        {
            army.ai_currentState = 5;
            army.goToCity = true;
            DrawPathAndGo(army, bestCityToRetret.transform.position, null);
        }
        if (bestAction == 6) //6 - идем в город бо выгодно
        {
            army.ai_currentState = 6;
            army.goToCity = true;
            DrawPathAndGo(army, bestCityToGo.transform.position, null);
        }
        if (bestAction == 7) // 7 - атакуем вражеский город 
        {
            army.ai_currentState = 7;
            
            DrawPathAndGo(army, bestCityToAttack.transform.position, null);
        }
        if (bestAction == 8) // //8 -  нет рядом городов  - подходим к выгодному городу
        {
            army.ai_currentState = 8;

            DrawHalfPathAndGo(army, bestCityToAttackFromFar.transform.position);
        }
        if (bestAction == 9) // //9  - лоупавер + рядом есть лоупавер, соединяемся
        {
            army.ai_currentState = 9;

            DrawPathAndGo(army, bestArmyToJoint.transform.position, bestArmyToJoint);
        }
        if (bestAction == 10) // //10[strategy move] 
        {
            army.ai_currentState = 10;

            DrawHalfPathAndGo(army, bestArmyToStrategyAttack.transform.position);
        }

        

        if (bestAction == 11)  // 11[strategy move] - отправка микро подкрепа потому что надо 
        {
            army.ai_currentState = 11;

            DrawPathAndGo(army, bestArmyToStrategyMove.transform.position, null);
        }


    }

    public void CalculateAllPlayerPower() //средний павер
    {
        float power = 0;
        for (int i = 0; i < allPlayerList.Count; i++)
        {


            power += allPlayerList[i].armyPower;
        }
        allPlayerPower = power; /// allPlayerList.Count;
        
    }

    public void CalculateAllEnemyPower() //средний павер
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

                if (!playerNearArmies.Contains(newArmy))
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

                if (!enemyNearArmies.Contains(newArmy))
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
        float distance = 15f; // сколько шагнуть
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
