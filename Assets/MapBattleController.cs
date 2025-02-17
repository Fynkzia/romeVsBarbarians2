using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBattleController : MonoBehaviour
{
    public BattleInfo battleInfo;

    public ArmyController playerArmy;
    public ArmyController enemyArmy;

    public float startTimeBattle;

    public float battleTickTime;


    public bool playerTick;
    public bool inBattle;
    public bool inAutoBattle;
   

    public float currentStartTimeBattle;

    public float currentBattleTickTime;


    public void StartBattle()
    {

        inBattle = true;
        battleInfo.BattleStart();

    }

    public void BattleWin()
    {
        Destroy(gameObject);

    }
    public void BattleLose()
    {

        Destroy(gameObject);
    }

    public void FightTick()
    {
        if (playerTick)
        {
            enemyArmy.GetDamage(playerArmy);

            playerTick = false;

            battleInfo.UpdateArmyInfo();
        }
        else
        {
            playerArmy.GetDamage(enemyArmy);
            playerTick = true;
            battleInfo.UpdateArmyInfo();
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!inBattle && !inAutoBattle)
        {
            currentStartTimeBattle += Time.deltaTime;

            battleInfo.StartBattleTimerUpdate(1 - (currentStartTimeBattle / startTimeBattle));

            if (currentStartTimeBattle > startTimeBattle)
            {
                inAutoBattle = true;
                currentStartTimeBattle = 0;

                battleInfo.AutoBattleStart();
                
            }
        }

        if (inAutoBattle)
        {
            currentBattleTickTime += Time.deltaTime;

            if (currentBattleTickTime > battleTickTime)
            {
                FightTick();

                currentBattleTickTime = 0 ;

            }
        }
    }
}
