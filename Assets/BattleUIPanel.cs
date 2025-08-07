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


    [SerializeField] private Image startbattleTimer;

    [SerializeField] private Button toBattleButton;
    [SerializeField] private Button autoBattleButton;
    [SerializeField] private Button retreatBattleButton;

    [SerializeField] private GameObject autoBattleButtonObject;
    [SerializeField] private GameObject autoBattlePanel;
    [SerializeField] private GameObject battlePanel;


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

        for (int i = 0; i < mapBattleController.playerArmy.squadList.Count; i++)
        {
            SquadController squad = mapBattleController.playerArmy.squadList[i];
            playerArmyPanel.AddSquad(squad.mapUIPanel);
        }

        for (int i = 0; i < mapBattleController.enemyArmy.squadList.Count; i++)
        {
            SquadController squad = mapBattleController.enemyArmy.squadList[i];
            enemyArmyPanel.AddSquad(squad.mapUIPanel);
        }

        playerArmyPanel.UpdateSquadsInfo(mapBattleController.playerArmy.squadList.ToArray(), mapBattleController.playerArmy.UnitsCountUpdate(), mapBattleController.playerArmy.ArmyPowerUpdate());
        enemyArmyPanel.UpdateSquadsInfo(mapBattleController.enemyArmy.squadList.ToArray(), mapBattleController.enemyArmy.UnitsCountUpdate(), mapBattleController.enemyArmy.ArmyPowerUpdate());

        //playerArmyUnitsCountText.text = "" + mapBattleController.playerArmy.UnitsCountUpdate();
        //enemyArmyUnitsCountText.text = "" + mapBattleController.enemyArmy.UnitsCountUpdate();


        //playerArmyPowerText.text = "" + mapBattleController.playerArmy.ArmyPowerUpdate();

        //enemyArmyPowerText.text = "" + mapBattleController.enemyArmy.ArmyPowerUpdate();
    }

        public void BattlePanelActivate()
    {
        for (int i = 0; i < mapBattleController.playerArmy.squadList.Count; i++)
        {
            SquadController squad = mapBattleController.playerArmy.squadList[i];
            playerArmyPanel.AddSquad(squad.mapUIPanel);
        }

        for (int i = 0; i < mapBattleController.enemyArmy.squadList.Count; i++)
        {
            SquadController squad = mapBattleController.enemyArmy.squadList[i];
            enemyArmyPanel.AddSquad(squad.mapUIPanel);
        }

        playerArmyPanel.UpdateSquadsInfo(mapBattleController.playerArmy.squadList.ToArray(), mapBattleController.playerArmy.UnitsCountUpdate(), mapBattleController.playerArmy.ArmyPowerUpdate());
        enemyArmyPanel.UpdateSquadsInfo(mapBattleController.enemyArmy.squadList.ToArray(), mapBattleController.enemyArmy.UnitsCountUpdate(), mapBattleController.enemyArmy.ArmyPowerUpdate());

        

        toBattleButton.onClick.RemoveAllListeners();
        retreatBattleButton.onClick.RemoveAllListeners();

        toBattleButton.onClick.AddListener(() => mapBattleController.StartBattle());
        toBattleButton.onClick.AddListener(() => SceneLoader.Instance.EnterBattle(mapBattleController.sceneIndex, mapBattleController.playerArmy, mapBattleController.enemyArmy));

        retreatBattleButton.onClick.AddListener(() => mapBattleController.PlayerRetreat());

        battlePanel.SetActive(false);
        autoBattlePanel.SetActive(false);
        autoBattleButtonObject.SetActive(false);

        Debug.Log("BattlePanelActivate");

        if (!mapBattleController.inAutoBattle)
        {
            
            battlePanel.SetActive(true);
            autoBattlePanel.SetActive(false);

            if (mapBattleController.inBattle)
            {
                autoBattleButtonObject.SetActive(false);
            }
            else
            {
                autoBattleButtonObject.SetActive(true);
            }
        }

        if (mapBattleController.inAutoBattle)
        {
            autoBattlePanel.SetActive(true);

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
