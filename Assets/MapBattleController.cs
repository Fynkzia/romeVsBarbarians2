using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBattleController : MonoBehaviour
{
    public int sceneIndex;
    public bool isSelect;


    public BattleInfo battleInfo;

    public ArmyController playerArmy;
    public ArmyController enemyArmy;
    public CityController city;

    public float startTimeBattle;

    public float battleTickTime;

    public bool playerTick;
    public bool inBattle;
    public bool inAutoBattle;
    public bool cityBattle;


    public float currentStartTimeBattle;

    public float currentBattleTickTime;

    public event Action<float> BattleTimerUpdate;
    public event Action BattleStart;
    public event Action AutoBattleStart;

    public event Action OnBattleWin;
    public event Action OnBattleLose;

    public ResourceManager resourceManager;
    public MapSceneManager mapSceneManager;

    private void Start()
    {
        mapSceneManager = GameObject.Find("MapSceneManager").GetComponent<MapSceneManager>();
    }

    public void Select(bool select)
    {
        isSelect = select;
        battleInfo.Select(select);

        

    }

    public void SelectThisBattle()
    {
         if (mapSceneManager != null)
        {
            mapSceneManager.controlController.SelectBattle(this);
        }
        else
        {
            mapSceneManager = GameObject.Find("MapSceneManager").GetComponent<MapSceneManager>();
            mapSceneManager.controlController.SelectBattle(this);
        }





    }

    public void StartBattle()
    {
        mapSceneManager.uIManager.BattlePanelDeactivation();

        inBattle = true;
        battleInfo.BattleStart();

        BattleStart?.Invoke();

    }
    public void StartAutoBattle()
    {

        inAutoBattle = true;
        battleInfo.AutoBattleStart();

        AutoBattleStart?.Invoke();
    }

    public void BattleWin()
    {
        if(enemyArmy.inCity && enemyArmy.city != null)
        {
            enemyArmy.city.CityCaptured(true);
        }


        enemyArmy.ArmyDestroy();
        playerArmy.ResetAfterBattle();

        Destroy(gameObject);

    }
    public void BattleLose()
    {
        if (playerArmy.inCity && playerArmy.city != null)
        {
            playerArmy.city.CityCaptured(false);
        }

        playerArmy.ArmyDestroy();
        enemyArmy.ResetAfterBattle();
        Destroy(gameObject);
    }

    public void PlayerRetreat()
    {

        SceneLoader.Instance.ArmyRetreat(sceneIndex);
        mapSceneManager.uIManager.BattlePanelDeactivation();


            enemyArmy.ResetAfterBattle();
            playerArmy.Retreat(transform.position);
      

        Destroy(gameObject);
    }

    public void AddSquadsToArmy(bool isPlayer,ArmyController army)
    {
        List<SquadController> squadListToAdd = new List<SquadController>();
        if (isPlayer)
        {
            for (int i = 0; i < army.squadList.Count; i++)
            {
                if (playerArmy.squadList.Count < 20)
                {
                    playerArmy.AddSquadFromArmy(army.squadList[i]);
                    

                    squadListToAdd.Add(army.squadList[i]);
                }
                else
                {
                    return;
                }
            }
          
        }
        else
        {
            
        }

        for (int i = 0; i < squadListToAdd.Count; i++)
        {
           // Debug.Log("squadListToAdd =" + i);
            army.RemoveSquad(squadListToAdd[i]);
            
        }

        SceneLoader.Instance.LoadNewSqads(sceneIndex, squadListToAdd.ToArray());

    }

    public void FightTick()
    {
        if (playerTick)
        {
            enemyArmy.GetDamage(playerArmy);

            for (int i = 0; i < enemyArmy.squadList.Count; i++)
            {
                if (enemyArmy.squadList[i].squadDie)
                {

                    resourceManager.ChangeAmountOfCoins(enemyArmy.squadList[i].coinsFromDie);
                }
            }

            enemyArmy.SqaudsUpdate();

            playerTick = false;

           
        }
        else
        {
            playerArmy.GetDamage(enemyArmy);
            playerArmy.SqaudsUpdate();
            playerTick = true;
            
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!inBattle && !inAutoBattle)
        {
            currentStartTimeBattle += Time.deltaTime;

         
            BattleTimerUpdate?.Invoke(1f - (currentStartTimeBattle / startTimeBattle));

            

            if (currentStartTimeBattle > startTimeBattle)
            {
              
                currentStartTimeBattle = 0;

                StartAutoBattle();


            }
        }

        if (inAutoBattle)
        {
            currentBattleTickTime += Time.deltaTime;

            if (currentBattleTickTime > battleTickTime)
            {
                if (enemyArmy.squadList.Count == 0)
                {
                    BattleWin();
                    return;
                }
                else if(playerArmy.squadList.Count == 0)
                {
                    BattleLose();
                    return;
                }

                FightTick();

                currentBattleTickTime = 0 ;

            }
        }else if (inBattle && !inAutoBattle)
        {
            currentBattleTickTime += Time.deltaTime;

            if (currentBattleTickTime > battleTickTime)
            {
                enemyArmy.UpdateArmyInfo();
                playerArmy.UpdateArmyInfo();

                currentBattleTickTime = 0;

            }
            
        }
    }
}
