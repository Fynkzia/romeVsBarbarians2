using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIPanel : MonoBehaviour
{
    public MapBattleController mapBattleController;



    [SerializeField] private ArmyUIPanel playerArmyPanel;
    [SerializeField] private ArmyUIPanel enemyArmyPanel;

    [SerializeField] private TextMeshProUGUI playerArmyUnitsCountText;
    [SerializeField] private TextMeshProUGUI enemyArmyUnitsCountText;

    [SerializeField] private TextMeshProUGUI playerArmyPowerText;
    [SerializeField] private TextMeshProUGUI enemyArmyPowerText;

    [SerializeField] private TextMeshProUGUI rewardText;


    [SerializeField] private Image startbattleTimer;

    [SerializeField] private Button toBattleButton;
    [SerializeField] private Button autoBattleButton;
    [SerializeField] private Button retreatBattleButton;

    [SerializeField] private GameObject autoBattleButtonObject;
    [SerializeField] private GameObject autoBattlePanel;
    [SerializeField] private GameObject battlePanel;

    [SerializeField] private GameObject victoryBar;
    [SerializeField] private TextMeshProUGUI victoryChanseText;

    float vicChanse;


    void OnDisable()
    {
        if (mapBattleController != null)
        {

            mapBattleController.BattleTimerUpdate -= StartBattleTimerUpdate;
            mapBattleController.AutoBattleStart -= SetAutoBattle;
            mapBattleController.BattleStart -= SetBattle;
        }
    }

    void OnEnable()
    {
        if (mapBattleController != null)
        {

            mapBattleController.BattleTimerUpdate += StartBattleTimerUpdate;
            mapBattleController.AutoBattleStart += SetAutoBattle;
            mapBattleController.BattleStart += SetBattle;
        }

    }

    public void BattlePanelUpdate()
    {
        BattlePanelClear();

        int reward = 0;

        for (int i = 0; i < mapBattleController.playerArmy.squadList.Count; i++)
        {
            SquadController squad = mapBattleController.playerArmy.squadList[i];
            playerArmyPanel.AddSquad(squad.mapUIPanel);
        }

        for (int i = 0; i < mapBattleController.enemyArmy.squadList.Count; i++)
        {
            SquadController squad = mapBattleController.enemyArmy.squadList[i];
            enemyArmyPanel.AddSquad(squad.mapUIPanel);
            reward += squad.coinsFromDie;
        }

        playerArmyPanel.UpdateSquadsInfo(mapBattleController.playerArmy.squadList.ToArray(), mapBattleController.playerArmy.UnitsCountUpdate(), mapBattleController.playerArmy.ArmyPowerUpdate());
        enemyArmyPanel.UpdateSquadsInfo(mapBattleController.enemyArmy.squadList.ToArray(), mapBattleController.enemyArmy.UnitsCountUpdate(), mapBattleController.enemyArmy.ArmyPowerUpdate());

        //playerArmyUnitsCountText.text = "" + mapBattleController.playerArmy.UnitsCountUpdate();
        //enemyArmyUnitsCountText.text = "" + mapBattleController.enemyArmy.UnitsCountUpdate();


        //playerArmyPowerText.text = "" + mapBattleController.playerArmy.ArmyPowerUpdate();

        //enemyArmyPowerText.text = "" + mapBattleController.enemyArmy.ArmyPowerUpdate();

        rewardText.text = "" + reward;

       



        float playerP = mapBattleController.playerArmy.ArmyPowerUpdate();
        float enemyP = mapBattleController.enemyArmy.ArmyPowerUpdate();



         vicChanse = playerP / (playerP + enemyP);

        victoryBar.transform.localScale = new Vector3(vicChanse, 0, 0);

        victoryChanseText.text = Mathf.RoundToInt(vicChanse * 100f) + "%";

    }




        public void BattlePanelActivate()
    {
        //for (int i = 0; i < mapBattleController.playerArmy.squadList.Count; i++)
        //{
        //    SquadController squad = mapBattleController.playerArmy.squadList[i];
        //    playerArmyPanel.AddSquad(squad.mapUIPanel);
        //}

        //for (int i = 0; i < mapBattleController.enemyArmy.squadList.Count; i++)
        //{
        //    SquadController squad = mapBattleController.enemyArmy.squadList[i];
        //    enemyArmyPanel.AddSquad(squad.mapUIPanel);
        //}

        // playerArmyPanel.UpdateSquadsInfo(mapBattleController.playerArmy.squadList.ToArray(), mapBattleController.playerArmy.UnitsCountUpdate(), mapBattleController.playerArmy.ArmyPowerUpdate());
        // enemyArmyPanel.UpdateSquadsInfo(mapBattleController.enemyArmy.squadList.ToArray(), mapBattleController.enemyArmy.UnitsCountUpdate(), mapBattleController.enemyArmy.ArmyPowerUpdate());

        BattlePanelUpdate();

        toBattleButton.onClick.RemoveAllListeners();
        retreatBattleButton.onClick.RemoveAllListeners();

        toBattleButton.onClick.AddListener(() => mapBattleController.StartBattle());
       

        retreatBattleButton.onClick.AddListener(() => mapBattleController.PlayerRetreat());
        autoBattleButton.onClick.AddListener(() => mapBattleController.StartAutoBattle());

        battlePanel.SetActive(false);
        autoBattlePanel.SetActive(false);
        autoBattleButtonObject.SetActive(false);

        Debug.Log("BattlePanelActivate");

        if (!mapBattleController.inAutoBattle && !mapBattleController.inBattle)
        {
            
            battlePanel.SetActive(true);
            autoBattlePanel.SetActive(false);

            if (mapBattleController.inBattle)
            {
                autoBattleButtonObject.SetActive(false);
            }
            else
            {
               // autoBattleButtonObject.SetActive(true);
            }


            if (vicChanse > 0.75f)
            {
                autoBattleButtonObject.SetActive(true);
            }
            else
            {
                autoBattleButtonObject.SetActive(false);
            }


        }
        else
        {
            if (mapBattleController.inAutoBattle)
            {
                autoBattlePanel.SetActive(true);
            }

        }

        
       



       

    }

    public void StartBattleTimerUpdate(float fillAmount)
    {
       
        startbattleTimer.fillAmount = fillAmount;
    }

   

    public void BattlePanelClear()
    {
        playerArmyPanel.Clear();
        enemyArmyPanel.Clear();
    }

    public void SetBattle()
    {
        autoBattleButtonObject.SetActive(false);
    }
    public void SetAutoBattle()
    {
        autoBattlePanel.SetActive(true);

        battlePanel.SetActive(false);
        autoBattleButtonObject.SetActive(false);
    }



}
