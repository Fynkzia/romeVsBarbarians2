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

    public float uiPanelUpdateTime;
    float uiTime;
 

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

    [SerializeField] private LayerMask armyMask;

    private void Start()
    {
        mapSceneManager = GameObject.Find("MapSceneManager").GetComponent<MapSceneManager>();
    }

    


        public void ArmyCheck()
    {
        BoxCollider coll = GetComponent<BoxCollider>();
        Collider[] hitColliders = Physics.OverlapBox(coll.center, coll.size, transform.rotation, armyMask);



        

        foreach (var hitCollider in hitColliders)
        {

            if ( (hitCollider.gameObject.tag == "Player") || (hitCollider.gameObject.tag == "Enemy" ))
            {

                ArmyController newArmy = hitCollider.gameObject.GetComponent<ArmyController>();

                if (newArmy.isPlayer)
                {
                   if(playerArmy != newArmy)
                    {
                        AddSquadsToArmy(true, newArmy);
                    }

                }
                else
                {
                    if (enemyArmy != newArmy)
                    {
                        AddSquadsToArmy(false, newArmy);
                    }
                }



            }

            Debug.Log("ArmyCheck");

        }
        //cencelInfoObject.gameObject.SetActive(false);



    }


    public void Select(bool select)
    {
        isSelect = select;
        battleInfo.Select(select);

        if (select)
        {
            playerArmy.armyInfoPanel.gameObject.SetActive(false);
            enemyArmy.armyInfoPanel.gameObject.SetActive(false);
        }
        else
        {
            playerArmy.armyInfoPanel.gameObject.SetActive(true);
            enemyArmy.armyInfoPanel.gameObject.SetActive(true);
        }
        

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
        if (SceneLoader.Instance.activeScene == -1)
        {
            mapSceneManager.uIManager.BattlePanelDeactivation();




            SceneLoader.Instance.EnterBattle(sceneIndex, playerArmy, enemyArmy);
            inBattle = true;


            battleInfo.BattleStart();

            BattleStart?.Invoke();
        }

    }
    public void StartAutoBattle()
    {

        inAutoBattle = true;
        battleInfo.AutoBattleStart();

        SceneLoader.Instance.StartAutoBattle(sceneIndex);

        AutoBattleStart?.Invoke();
    }

    public void BattleWin()
    {
        if(enemyArmy.inCity && enemyArmy.city != null)
        {
            enemyArmy.city.CityCaptured(true);
        }
        if (city != null)
        {
            city.CityInBattle(false);
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

        if (city != null)
        {
            city.CityInBattle(false);
        }

        playerArmy.ArmyDestroy();
        enemyArmy.ResetAfterBattle();
        Destroy(gameObject);
    }

    public void PlayerRetreat()
    {

        if (city != null)
        {
            city.CityInBattle(false);
        }

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
                    //return;
                }
            }
          
        }
        else
        {
            for (int i = 0; i < army.squadList.Count; i++)
            {
                if (enemyArmy.squadList.Count < 20)
                {
                    enemyArmy.AddSquadFromArmy(army.squadList[i]);


                    squadListToAdd.Add(army.squadList[i]);
                }
                else
                {
                    //return;
                }
            }
        }

        for (int i = 0; i < squadListToAdd.Count; i++)
        {
           // Debug.Log("squadListToAdd =" + i);
            army.RemoveSquad(squadListToAdd[i]);
            
        }

        if (inBattle)
        {
           
            SceneLoader.Instance.LoadNewSqads(sceneIndex, squadListToAdd.ToArray(),army);
        }

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

                //StartAutoBattle();
                StartBattle();

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

        if (isSelect)
        {
            uiTime += Time.deltaTime;

            if (uiTime > uiPanelUpdateTime)
            {

                mapSceneManager.uIManager.battleUIPanel.BattlePanelUpdate();
                uiTime = 0;

              
            }
        }
    }

    //UI

   
}
