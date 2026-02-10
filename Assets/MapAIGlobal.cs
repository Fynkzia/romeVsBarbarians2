using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Linq;

public class MapAIGlobal : MonoBehaviour
{
    [Header("Analysis data")]

    [SerializeField] public List<ArmyController> allEnemyArmies;
    [SerializeField] public List<ArmyController> allPlayerArmies;

    [SerializeField] public List<ArmyController> smallArmies;
    [SerializeField] public List<ArmyController> mediumArmies;
    [SerializeField] public List<ArmyController> largeArmies;

    [SerializeField] public List<CityController> allEnemyCityes;
    [SerializeField] public List<CityController> allPlayerCityes;

    [SerializeField] public List<CityController> borderEnemyCityes;
    [SerializeField] public List<CityController> borderPlayerCityes;

    [SerializeField] public List<MapBattleController> battles;

    [SerializeField]
    public float nearEnemyRadius;

    [SerializeField]
    public float farEnemyRadius;

    [SerializeField] public float minPowerMultiplayer;
    [SerializeField] public float maxPowerMultiplayer;


    [SerializeField] public float actionTime;
    [SerializeField] public float currentActionTime;

    [SerializeField] public float allEnemyPower;
    [SerializeField] public float allPlayerPower;


    [SerializeField] public float avgEnemyPower;
    [SerializeField] public float avgPlayerPower;

    [SerializeField] public int[] blockActions;
    [SerializeField] public float[] actionPriority;
    // 0 идем атаковать армию, которая угражает нашему городу
    // 1 отводим армию
    // 2 нападаем на армию
    // 3 помош армии?
    // 4 двигаем в пограничные города
    // 5 атакуюем пограничные города
    //6 в бой нужно подкрепление?
    //7 нужно соеденение?

    //10 улучшения в городе
    //11 спавн в городе

    [SerializeField] public ArmyController bestArmyToDefenceCity;
    [SerializeField] public ArmyController targetArmyToDefenceCity;

    [SerializeField] public ArmyController bestArmyToGoBack;
    [SerializeField] public ArmyController targetArmyToGoBack;

    [SerializeField] public ArmyController bestArmyToAttack;
    [SerializeField] public ArmyController targetArmyToAttack;

    [SerializeField] public ArmyController bestArmyToHelp;
    [SerializeField] public ArmyController targetArmyToHelp;


    [SerializeField] public ArmyController bestArmyToReinforce;
    [SerializeField] public MapBattleController targetBattleToReinforce;

    [SerializeField] public ArmyController bestArmyToGoJoin;
    [SerializeField] public ArmyController targetArmyToGoJoin;

    [SerializeField] public ArmyController bestArmyToGoBorder;
    [SerializeField] public CityController targetBorderCity;

    [SerializeField] public ArmyController bestArmyAttackBorder;
    [SerializeField] public CityController targetAttackBorderCity;

    [SerializeField] public CityController bestCityToSpawn;
    [SerializeField] public CityController bestCityUpgrade;

    [Header("Other")]

    [SerializeField] MapSceneManager mapSceneManager;
    [SerializeField] public MapAICities aICities;

    [Header("Difficult Settings")]
    [SerializeField]
    public float difficult = 0;

    [SerializeField]
    public bool mediumArmiesAttackEmptyBorders = false;
    [SerializeField]
    public bool mediumArmiesAttackAnyBorders = false;
    [SerializeField]
    public float attackBordersBlockActions = 0;

    [SerializeField]
    public float spawnBlockActions = 0;
    [SerializeField]
    public float buildBlockActions = 0;

    [Header("debag")]
    [SerializeField] public List<ArmyController> dangerArmies = new List<ArmyController>();

    void Update()
    {
        currentActionTime += Time.deltaTime;

        if(currentActionTime > actionTime)
        {

            AiAction();
            currentActionTime = 0;

        }

    }

    public void AiAction()
    {
        for (int i = 0; i < actionPriority.Length; i++) ///// очистка
        {
            actionPriority[i] = 0;
        }

        SetData();



        /// 0 0 идем атаковать армию, которая угражает нашему городу////
        


        actionPriority[0] = GetPriority_DangerCity() * 1.2f;




        /// 1 ////

        actionPriority[1] = GetPriority_GoBack() * 1.3f;

        /// 2 ////

        actionPriority[2] = GetPriority_Attack() * 1.1f;

        /// 3 ////

        actionPriority[3] = GetPriority_Help() * 1.4f;

        /// 4 ////

        actionPriority[4] = GetPriority_GoBorders();
        /// 5 ////

        actionPriority[5] = GetPriority_AttackBorders();

        /// 6 ////

        actionPriority[6] = GetPriority_GoReinfocre() * 1.4f;
        /// 7  ///
        actionPriority[7] = GetPriority_GoToJoin() * 1.3f;

        //10 /// город расширяется
        actionPriority[10] = GetPriority_CityBuild();
        // 11 // город спавнит пачку
        actionPriority[11] = GetPriority_CitySpawn();

        for (int i = 0; i < blockActions.Length; i++) ///// блок на несколько ходов что не спамило
        {
            if (blockActions[i] > 0)
            {
                actionPriority[i] = 0;
                blockActions[i]--;
            }
        }


        float lastPriority = 0f;
        int bestAction = -1;
        for (int i = 0; i < actionPriority.Length; i++) ///// финальное решение
        {
            if (actionPriority[i] > lastPriority)
            {
                lastPriority = actionPriority[i];
                bestAction = i;
            }
        }

        Debug.Log("action Ai : " + bestAction);

        if (bestAction == -1)
        {
            return;
        }

        
        
        if (bestAction == 0)
        {
            if (Vector3.Distance(bestArmyToDefenceCity.transform.position, targetArmyToDefenceCity.transform.position) > nearEnemyRadius*0.9f)
            {
                DrawHalfPathAndGo(bestArmyToDefenceCity, targetArmyToDefenceCity.transform.position);
            }
            else {
                DrawPathAndGo(bestArmyToDefenceCity, targetArmyToDefenceCity.transform.position, null);
            }

            bestArmyToDefenceCity.ai_currentPriority = (int)actionPriority[bestAction];

        }
        if (bestAction == 1)
        {

            Vector3 backVector;

           
                backVector = Vector3.Normalize(bestArmyToGoBack.transform.position - targetArmyToGoBack.transform.position);
                DrawPathAndGo(bestArmyToGoBack, targetArmyToGoBack.transform.position + (backVector * nearEnemyRadius), null);

            bestArmyToGoBack.ai_currentPriority = (int)actionPriority[bestAction];
            bestArmyToGoBack.ai_needHelp = true;
        }
        if (bestAction == 2)
        {

            DrawPathAndGo(bestArmyToAttack, targetArmyToAttack.transform.position, null);
            bestArmyToAttack.ai_currentPriority = (int)actionPriority[bestAction];
        }
        if (bestAction == 3)
        {
            if (bestArmyToHelp.ai_currentPriority < actionPriority[bestAction])
            {
                if (Vector3.Distance(bestArmyToHelp.transform.position, targetArmyToHelp.transform.position) > nearEnemyRadius * 0.9f)
                {
                    DrawHalfPathAndGo(bestArmyToHelp, targetArmyToHelp.transform.position);
                }
                else
                {
                    DrawPathAndGo(bestArmyToHelp, targetArmyToHelp.transform.position, targetArmyToHelp);
                }

                bestArmyToHelp.ai_currentPriority = (int)actionPriority[bestAction];
                targetArmyToHelp.ai_needHelp = false;
            }

            blockActions[3] = 3;
        }
        if (bestAction == 4) // идем в город
        {

            if (bestArmyToGoBorder.ai_currentPriority < actionPriority[bestAction])
            {
                if (Vector3.Distance(bestArmyToGoBorder.transform.position, targetBorderCity.transform.position) > nearEnemyRadius * 0.9f)
                {
                    DrawHalfPathAndGo(bestArmyToGoBorder, targetBorderCity.transform.position);
                }
                else
                {
                    DrawPathAndGo(bestArmyToGoBorder, targetBorderCity.transform.position, null);

                }

                bestArmyToGoBorder.ai_currentPriority = (int)actionPriority[bestAction];
            }
           
            blockActions[4] = 3;

        }
        if (bestAction == 5)
        {
            if (bestArmyAttackBorder.ai_currentPriority < actionPriority[bestAction])
            {

                if (Vector3.Distance(bestArmyAttackBorder.transform.position, targetAttackBorderCity.transform.position) > nearEnemyRadius * 0.9f)
                {
                    DrawHalfPathAndGo(bestArmyAttackBorder, targetAttackBorderCity.transform.position);
                }
                else
                {
                    DrawPathAndGo(bestArmyAttackBorder, targetAttackBorderCity.transform.position, null);

                }


                bestArmyAttackBorder.ai_currentPriority = (int)actionPriority[bestAction];
            }

           
            blockActions[5] = (int)attackBordersBlockActions;
        }
        if (bestAction == 6)
        {

            DrawPathAndGo(bestArmyToReinforce, targetBattleToReinforce.transform.position, null);
            blockActions[6] = (int)actionPriority[6];
            bestArmyToReinforce.ai_currentPriority = (int)actionPriority[bestAction];
        }
        if (bestAction == 7)
        {
            if (bestArmyToGoJoin.ai_currentPriority < actionPriority[bestAction])
            {
                blockActions[7] = (int)actionPriority[7];
                DrawPathAndGo(bestArmyToGoJoin, targetArmyToGoJoin.transform.position, targetArmyToGoJoin);

                bestArmyToGoJoin.ai_currentPriority = (int)actionPriority[bestAction];
            }
        }


        if (bestAction == 10)
        {
            blockActions[10] = (int)buildBlockActions;
            aICities.AiCitiesAction(bestCityUpgrade,-1);
        }
        if (bestAction == 11)
        {
            blockActions[11] = (int)spawnBlockActions;
            aICities.AiCitiesAction(bestCityToSpawn, Random.Range(0,3));
            aICities.AiCitiesAction(bestCityToSpawn, Random.Range(0, 3));
            aICities.AiCitiesAction(bestCityToSpawn, Random.Range(0, 3));
        }
    }


    public float GetPriority_DangerCity()
    {
        dangerArmies = new List<ArmyController>();
        float score = 0;

        for (int i = 0; i < allPlayerArmies.Count; i++)
        {
            if (allPlayerArmies[i].isCapturing)
            {
                
                dangerArmies.Add(allPlayerArmies[i]);
               
            }
        }

       


           

        

        for (int i = 0; i < allEnemyCityes.Count; i++)
        {

            for (int j = 0; j < allPlayerArmies.Count; j++)
            {
                if (allEnemyCityes[i].armyInCity == null)
                {
                    if (Vector3.Distance(allEnemyCityes[i].transform.position, allPlayerArmies[j].transform.position) < nearEnemyRadius)
                    {
                        dangerArmies.Add(allPlayerArmies[j]);
                       
                    }
                }
            }

              
        }

        if (dangerArmies.Count > 0)
        {

            var result = GetBestAttackPair(allEnemyArmies, dangerArmies);

            bestArmyToDefenceCity = result.ourArmy;
            targetArmyToDefenceCity = result.enemyArmy;
            score = result.score;

            if (result.score < 50)
            {
                score = 10;
            }
            else if (result.score < 100)
            {
                score = 7;
            }
            else if (result.score < 200)
            {
                score = 5;
            }
        }
        else
        {
            score = 0;
        }


        return score;
    }

    public float GetPriority_GoBack()
    {
        float score = 0;
        float maxScore = 0;

        for (int i = 0; i < allEnemyArmies.Count; i++)
        {

            for (int j = 0; j < allPlayerArmies.Count; j++)
            {
                if(allEnemyArmies[i] == null || allPlayerArmies[j] == null)
                {
                    break;
                }

                if (!allEnemyArmies[i].inBattle && !allPlayerArmies[j].inBattle)
                {
                    if (Vector3.Distance(allEnemyArmies[i].transform.position, allPlayerArmies[j].transform.position) < nearEnemyRadius)
                    {
                        if(allEnemyArmies[i].armyPower * minPowerMultiplayer < allPlayerArmies[j].armyPower && allEnemyArmies[i].armyPower * maxPowerMultiplayer > allPlayerArmies[j].armyPower)
                        {
                            score = 5;

                           
                        }

                        if (allEnemyArmies[i].armyPower * minPowerMultiplayer < allPlayerArmies[j].armyPower && allEnemyArmies[i].armyPower * maxPowerMultiplayer < allPlayerArmies[j].armyPower)
                        {
                            score = 10;

                          
                        }


                    }

                    if (allEnemyArmies[i] == null || allPlayerArmies[j] == null)
                    {
                        break;
                    }

                    if (score > maxScore)
                    {
                        maxScore = score;

                        bestArmyToGoBack = allEnemyArmies[i];
                        targetArmyToGoBack = allPlayerArmies[j];
                    }
                }
            }


        }

        return maxScore;
    }

    public float GetPriority_Attack()
    {
        float score = 0;
        float maxScore = 0;

        for (int i = 0; i < allEnemyArmies.Count; i++)
        {
           
            for (int j = 0; j < allPlayerArmies.Count; j++)
            {
                if (allEnemyArmies[i] == null || allPlayerArmies[j] == null)
                {
                    break;
                }

                if (!allEnemyArmies[i].inBattle && !allPlayerArmies[j].inBattle)
                {
                    if (Vector3.Distance(allEnemyArmies[i].transform.position, allPlayerArmies[j].transform.position) < nearEnemyRadius)
                    {
                        if (allEnemyArmies[i].armyPower  > allPlayerArmies[j].armyPower * minPowerMultiplayer && allEnemyArmies[i].armyPower  < allPlayerArmies[j].armyPower * maxPowerMultiplayer)
                        {
                            score = 5;


                        }

                        if (allEnemyArmies[i].armyPower > allPlayerArmies[j].armyPower * minPowerMultiplayer && allEnemyArmies[i].armyPower > allPlayerArmies[j].armyPower * maxPowerMultiplayer)
                        {
                            score = 10;


                        }


                    }

                    if (score > maxScore)
                    {
                        maxScore = score;

                        bestArmyToAttack = allEnemyArmies[i];
                        targetArmyToAttack = allPlayerArmies[j];
                    }
                }
            }


        }

        return maxScore;
    }


    public float GetPriority_Help()
    {
        float score = 0;
        float maxScore = 10;

        targetArmyToHelp = null;

        for (int i = 0; i < allEnemyArmies.Count; i++)
        {
            if (allEnemyArmies[i].ai_needHelp)
            {
                targetArmyToHelp = allEnemyArmies[i];
                break;
            }

        }


        if(targetArmyToHelp != null)
        {

            if (smallArmies.Count > 0)
            {
                bestArmyToHelp = GetClosestArmy(smallArmies, targetArmyToHelp.transform.position);

            }else if(mediumArmies.Count > 0)
            {
                bestArmyToHelp = GetClosestArmy(mediumArmies, targetArmyToHelp.transform.position);
            }
          
        }

        if (targetArmyToHelp == null || bestArmyToHelp == null)
        {
            return 0;
        }
        else
        {
            return maxScore;
        }
    }

    public float GetPriority_GoBorders()
    {
        List<ArmyController> armiesToGoBorders = new List<ArmyController>();

        float score = 0;
        float maxScore = 0;

        for (int i = 0; i < mediumArmies.Count; i++)
        {
            if (mediumArmies[i].inCity)
            {
                if (!borderEnemyCityes.Contains(mediumArmies[i].city))
                {
                    armiesToGoBorders.Add(mediumArmies[i]);


                }

            }
            else
            {
                armiesToGoBorders.Add(mediumArmies[i]);
            }
        }

        if (armiesToGoBorders.Count > 0)
        {
            for (int i = 0; i < borderEnemyCityes.Count; i++)
            {
                if(borderEnemyCityes[i].armyInCity == null)
                {
                    score = 10;
                }
                else
                {
                    if (smallArmies.Contains(borderEnemyCityes[i].armyInCity)){
                        score = 7;
                    }
                    else if(mediumArmies.Contains(borderEnemyCityes[i].armyInCity))
                    {
                        score = 5;
                    }
                    else if (largeArmies.Contains(borderEnemyCityes[i].armyInCity))
                    {
                        score = 2;
                    }

                }



                if (score > maxScore)
                {
                    maxScore = score;

                   
                    targetBorderCity = borderEnemyCityes[i];
                }
            }
        }

        if (armiesToGoBorders.Count > 0 && targetBorderCity != null)
        {
            bestArmyToGoBorder = GetClosestArmy(armiesToGoBorders, targetBorderCity.transform.position);
        }



            return maxScore;
        }

    public float GetPriority_AttackBorders()
    {
        if(borderPlayerCityes.Count == 0)
        {
            return 0;
        }

        //if (largeArmies.Count == 0)
        //{
        //    return 0;
        //}


        List<ArmyController> armiesToGoBorders = new List<ArmyController>();

        float score = 0;
        float maxScore = 10;

        float power = 0f;
        float minPower = 0f;

        for (int i = 0; i < borderPlayerCityes.Count; i++)
        {
            if(borderPlayerCityes[i].armyInCity == null)
            {
                power = 0f;
            }
            else
            {
                power = borderPlayerCityes[i].armyInCity.armyPower;
            }

            if(power <= minPower)
            {
                minPower = power;
                targetAttackBorderCity = borderPlayerCityes[i];
            }
        }

        for (int i = 0; i < largeArmies.Count; i++)
        {
            if (!largeArmies[i].inBattle)
            {
                armiesToGoBorders.Add(largeArmies[i]);
            }

        }

        if (mediumArmiesAttackEmptyBorders)
        {
            if (targetAttackBorderCity.armyInCity == null && largeArmies.Count == 0)
            {
                for (int i = 0; i < mediumArmies.Count; i++)
                {
                    if (!mediumArmies[i].inBattle && mediumArmies[i].armyPower >= minPower)
                    {
                        armiesToGoBorders.Add(mediumArmies[i]);
                    }

                }
            }
        }

        if (mediumArmiesAttackAnyBorders)
        {
            
                for (int i = 0; i < mediumArmies.Count; i++)
                {
                    if (!mediumArmies[i].inBattle && mediumArmies[i].armyPower >= minPower)
                    {
                        armiesToGoBorders.Add(mediumArmies[i]);
                    }

                }
            
        }

        if (armiesToGoBorders.Count == 0)
        {
            Debug.Log("action Ai : armiesToGoBorders.Count == 0 ");
            return  0;
        }

        if (targetAttackBorderCity == null)
        {
            Debug.Log("action Ai : targetAttackBorderCity == null ");
            return 0;
        }


        bestArmyAttackBorder = GetClosestArmy(armiesToGoBorders, targetAttackBorderCity.transform.position);


        return maxScore;
    }

    public float GetPriority_GoReinfocre()
    {
        float score = 0;
        float maxScore = 0;
        bestArmyToReinforce = null;

       if (battles.Count == 0)
        {
            return maxScore;
        }
        else
        {
            for (int i = 0; i < battles.Count; i++)
            {
                if( battles[i].enemyArmy.armyPower  < battles[i].playerArmy.armyPower * minPowerMultiplayer)
                {
                    score = 5;
                }

                if (battles[i].enemyArmy.armyPower * maxPowerMultiplayer < battles[i].playerArmy.armyPower )
                {
                    score = 7;

                    if(battles[i].playerArmy.armyPower > allPlayerPower / 2)// это ебейший бой братик
                    {
                        score = 10;
                    }
                }

                if(score > maxScore)
                {
                    maxScore = score;

                    targetBattleToReinforce = battles[i];
                }
            }
            Debug.Log("AI_Reinfocre score" + maxScore);
            List<ArmyController> armiesToGoreinforce = new List<ArmyController>();

            if (maxScore == 5)
            {

                armiesToGoreinforce.AddRange(mediumArmies);
                armiesToGoreinforce.AddRange(smallArmies);

                float minPower = 1000;
                int bestId = -1;
                for (int i = 0; i < armiesToGoreinforce.Count; i++)
                {
                    if (armiesToGoreinforce[i].armyPower < minPower && !armiesToGoreinforce[i].inBattle)
                    {
                        if (armiesToGoreinforce[i].armyPower + targetBattleToReinforce.enemyArmy.armyPower > targetBattleToReinforce.playerArmy.armyPower)
                        {


                            bestId = i;
                            minPower = armiesToGoreinforce[i].armyPower;
                        }
                    }
                }

                if(bestId >= 0)
                {

                    bestArmyToReinforce = armiesToGoreinforce[bestId];
                }
            }else if(maxScore > 5)
            {
                armiesToGoreinforce.AddRange(smallArmies);
                armiesToGoreinforce.AddRange(mediumArmies);
                armiesToGoreinforce.AddRange(largeArmies);

                float maxPower = 0;
                int bestId = -1;
                for (int i = 0; i < armiesToGoreinforce.Count; i++)
                {
                    if (armiesToGoreinforce[i].armyPower > maxPower && !armiesToGoreinforce[i].inBattle)
                    {
                        //if (armiesToGoreinforce[i].armyPower + targetBattleToReinforce.enemyArmy.armyPower > targetBattleToReinforce.playerArmy.armyPower)
                        //{


                            bestId = i;
                        maxPower = armiesToGoreinforce[i].armyPower;
                        //}
                    }
                }

                if (bestId >= 0)
                {

                    bestArmyToReinforce = armiesToGoreinforce[bestId];
                }
            }

            if(bestArmyToReinforce == null)
            {
                maxScore = 0;
            }

        }

        return maxScore;
    }

    public float GetPriority_GoToJoin()
    {
        float score = 0;
        float maxScore = 0;
        int largeArmyPlayer = 0;

        for (int i = 0; i < allPlayerArmies.Count; i++)
        {
            if (allPlayerArmies[i].armyPower > avgEnemyPower * maxPowerMultiplayer)
            {
                largeArmyPlayer++;
            }

        }

            if (mediumArmies.Count > 1 && largeArmies.Count < 1)
        {
            score = 10;
        }
        else 
        if (mediumArmies.Count > 1 && largeArmies.Count <= largeArmyPlayer)
        {
            score = 5;
        }

        for (int i = 0; i < mediumArmies.Count; i++)
        {
            for (int j = 0; j < mediumArmies.Count; j++)
            {
                if(mediumArmies[i] != mediumArmies[j])
                {


                    if(mediumArmies[i].armyPower > mediumArmies[j].armyPower)
                    {
                        bestArmyToGoJoin = mediumArmies[j];
                        targetArmyToGoJoin = mediumArmies[i];
                        break;
                    }
                }

             }
        }

           if(score == 0 || bestArmyToGoJoin == null)
        {
            maxScore = 0;
        }
        else
        {
            maxScore = score;
        }

        return maxScore;
    }

    public float GetPriority_CityBuild()
    {
        float score = 0;
        float maxScore = 0;

        bestCityUpgrade = null;
        if (aICities.enemyCoins > 160 && allEnemyPower>allPlayerPower*minPowerMultiplayer)
        {
            maxScore = 10;
        }

        float lastBuildingsCount = 1000f;
        for (int i = 0; i < allEnemyCityes.Count; i++)
        {
            if (!allEnemyCityes[i].isSmallCity && allEnemyCityes[i].cityBuildings < allEnemyCityes[i].cityBuildingsMax)
            {
                if (lastBuildingsCount >= allEnemyCityes[i].cityBuildings) {
                    bestCityUpgrade = allEnemyCityes[i];
                        }
            }

        }

        Debug.Log("GetPriority_CityBuild " + bestCityUpgrade.name);
        if(bestCityUpgrade == null)
        {
            maxScore = 0;
        }

        return maxScore;
    }

    public float GetPriority_CitySpawn()
    {
        float score = 0;
        float maxScore = 0;

        List<CityController> bestCities = new List<CityController>();

        bestCityToSpawn = null;
        if (aICities.enemyCoins > 150f && allEnemyPower < allPlayerPower * minPowerMultiplayer)
        {
            maxScore = 10;
        }

       
        for (int i = 0; i < borderEnemyCityes.Count; i++)
        {
            if (!borderEnemyCityes[i].isSmallCity)
            {
                bestCities.Add(borderEnemyCityes[i]);
            }

        }

        if(bestCities.Count == 0)
        {
            for (int i = 0; i < allEnemyCityes.Count; i++)
            {
                if (!allEnemyCityes[i].isSmallCity)
                {
                    bestCities.Add(allEnemyCityes[i]);
                }

            }
        }

        if (bestCities.Count == 0)
        {


        }
        else
        {
            bestCityToSpawn = bestCities[Random.Range(0, bestCities.Count)];
        }

            if (bestCityToSpawn == null)
        {
            maxScore = 0;
        }

        return maxScore;
    }

    public static (ArmyController ourArmy, ArmyController enemyArmy, float score)
        GetBestAttackPair(List<ArmyController> ourArmies, List<ArmyController> enemyArmies)
    {
        ArmyController bestOur = null;
        ArmyController bestEnemy = null;
        float bestScore = float.MaxValue;

        foreach (var our in ourArmies)
        {
            if (our.inBattle)
            {
                continue;
            }
            foreach (var enemy in enemyArmies)
            {
                float distance = Vector3.Distance(our.transform.position, enemy.transform.position);

                // защита от деления на ноль
                float powerRatio = (enemy.armyPower <= 0) ? 0.1f : (float)enemy.armyPower;

                float utility = distance * (powerRatio / our.armyPower);

                // чем меньше utility — тем выгоднее
                if (utility < bestScore)
                {
                    bestScore = utility;
                    bestOur = our;
                    bestEnemy = enemy;
                }
            }
        }

        return (bestOur, bestEnemy, bestScore);
    }

    public static ArmyController GetClosestArmy(List<ArmyController> armies, Vector3 point)
    {
        ArmyController closest = null;
        float closestDist = float.MaxValue;

        foreach (var army in armies)
        {
            if(army.transform.position == point)
            {
                continue;
            }
            float dist = Vector3.Distance(army.transform.position, point);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = army;
            }
        }

        return closest;
    }

    public void SetData()
    {
        allPlayerArmies = new List<ArmyController>();

        for (int i = 0; i < mapSceneManager.playerArmiesObject.transform.childCount; i++)
        {
            if (mapSceneManager.playerArmiesObject.transform.GetChild(i).gameObject.activeSelf)
            {
                allPlayerArmies.Add(mapSceneManager.playerArmiesObject.transform.GetChild(i).GetComponent<ArmyController>());
            }

        }

        allEnemyArmies = new List<ArmyController>();

        for (int i = 0; i < mapSceneManager.enemiesArmiesObject.transform.childCount; i++)
        {
            if (mapSceneManager.enemiesArmiesObject.transform.GetChild(i).gameObject.activeSelf)
            {
                allEnemyArmies.Add(mapSceneManager.enemiesArmiesObject.transform.GetChild(i).GetComponent<ArmyController>());
            }

        }
        CalculateAllPlayerPower();
        CalculateAllEnemyPower();

        smallArmies  = new List<ArmyController>();
        for (int i = 0; i < allEnemyArmies.Count; i++)
        {
            if (allEnemyArmies[i].armyPower < avgPlayerPower*0.8)
            {
                smallArmies.Add(allEnemyArmies[i]);
            }

        }

        mediumArmies = new List<ArmyController>();
        for (int i = 0; i < allEnemyArmies.Count; i++)
        {
            if (allEnemyArmies[i].armyPower > avgPlayerPower * 0.8 && allEnemyArmies[i].armyPower < avgPlayerPower * 1.5)
            {
                mediumArmies.Add(allEnemyArmies[i]);
            }

        }

        largeArmies = new List<ArmyController>();
        for (int i = 0; i < allEnemyArmies.Count; i++)
        {
            if (allEnemyArmies[i].armyPower >= avgPlayerPower * 1.5)
            {
                largeArmies.Add(allEnemyArmies[i]);
            }

        }


        SetAllEnemyCityList();
        SetAllPlayersCityList();

        GetAdaptiveBorderCities(allPlayerCityes, allEnemyCityes ,1.2f,out borderPlayerCityes, out borderEnemyCityes);

       

        battles = SceneLoader.Instance.mapBattleControllers;

    }

    public void CalculateAllPlayerPower()
    {
        float power = 0;
        for (int i = 0; i < allPlayerArmies.Count; i++)
        {


            power += allPlayerArmies[i].armyPower;
        }
        allPlayerPower = power;
        avgPlayerPower = power / allPlayerArmies.Count;

    }

    public void CalculateAllEnemyPower()
    {
        float power = 0;
        for (int i = 0; i < allEnemyArmies.Count; i++)
        {


            power += allEnemyArmies[i].armyPower;
        }
        allEnemyPower = power;
        avgEnemyPower = power / allEnemyArmies.Count;

    }

    public void SetAllEnemyCityList()
    {

        allEnemyCityes = new List<CityController>();



        for (int i = 0; i < mapSceneManager.citiesObject.transform.childCount; i++)
        {
            GameObject city = mapSceneManager.citiesObject.transform.GetChild(i).gameObject;
            if (city.activeSelf && city.tag == "Enemy")
            {
                allEnemyCityes.Add(mapSceneManager.citiesObject.transform.GetChild(i).GetComponent<CityController>());
            }
        }


    }


    public void SetAllPlayersCityList()
    {

        allPlayerCityes = new List<CityController>();



        for (int i = 0; i < mapSceneManager.citiesObject.transform.childCount; i++)
        {
            GameObject city = mapSceneManager.citiesObject.transform.GetChild(i).gameObject;
            if (city.activeSelf && city.tag == "Player")
            {
                allPlayerCityes.Add(mapSceneManager.citiesObject.transform.GetChild(i).GetComponent<CityController>());
            }
        }


    }

    public static void GetAdaptiveBorderCities(
        List<CityController> playerCities,
        List<CityController> enemyCities,
        float borderMultiplier,  // например 1.2f
        out List<CityController> playerBorder,
        out List<CityController> enemyBorder)
    {
        playerBorder = new List<CityController>();
        enemyBorder = new List<CityController>();

        // список расстояний ближайших "паров"
        List<float> nearestDistances = new List<float>();

        // ближайшие пары для игрока
        Dictionary<CityController, float> playerNearest = new Dictionary<CityController, float>();
        foreach (var pCity in playerCities)
        {
            float minDist = float.MaxValue;

            foreach (var eCity in enemyCities)
            {
                float dist = Vector3.Distance(pCity.transform.position, eCity.transform.position);
                if (dist < minDist)
                    minDist = dist;
            }

            if (minDist < float.MaxValue)
            {
                playerNearest[pCity] = minDist;
                nearestDistances.Add(minDist);
            }
        }

        // ближайшие пары для врага
        Dictionary<CityController, float> enemyNearest = new Dictionary<CityController, float>();
        foreach (var eCity in enemyCities)
        {
            float minDist = float.MaxValue;

            foreach (var pCity in playerCities)
            {
                float dist = Vector3.Distance(eCity.transform.position, pCity.transform.position);
                if (dist < minDist)
                    minDist = dist;
            }

            if (minDist < float.MaxValue)
            {
                enemyNearest[eCity] = minDist;
                nearestDistances.Add(minDist);
            }
        }

        // средняя дистанция ближайших городов
        float avg = 0;
        if (nearestDistances.Count > 0)
            avg = nearestDistances.Sum() / nearestDistances.Count;

        float borderLimit = avg * borderMultiplier;

        // формируем пограничные города
        foreach (var kvp in playerNearest)
        {
            if (kvp.Value <= borderLimit)
                playerBorder.Add(kvp.Key);
        }

        foreach (var kvp in enemyNearest)
        {
            if (kvp.Value <= borderLimit)
                enemyBorder.Add(kvp.Key);
        }
    }


    // controll

    private void DrawPathAndGo(ArmyController army, Vector3 targetPos, ArmyController jointArmy)
    {
        army.targetObject.transform.position = targetPos;
        army.SetMoving(true);

        if (jointArmy != null)
        {
            army.goToJoint = true;
            army.jointArmy = jointArmy;
        }
    }

    private void DrawHalfPathAndGo(ArmyController army, Vector3 targetPos)
    {


        Vector3 dir = (targetPos - army.transform.position).normalized;
        float distance = 20f * army.armyMorale / army.armyMoraleMax; // сколько шагнуть
        Vector3 between = army.transform.position + dir * distance;

        army.targetObject.transform.position = between;
        army.SetMoving(true);
        Debug.Log("DrawHalfPathAndGo = " + between);
    }

}