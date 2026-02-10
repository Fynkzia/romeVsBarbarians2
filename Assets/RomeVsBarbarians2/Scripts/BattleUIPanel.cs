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
    [SerializeField] private Gradient victoryChanseGradient;

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

    public void BattlePanelCreate()
    {
        BattlePanelClear();
        playerArmyPanel.UpdateArmyInfo( mapBattleController.playerArmy.UnitsCountUpdate(), mapBattleController.playerArmy.ArmyPowerUpdate(),0);
        enemyArmyPanel.UpdateArmyInfo( mapBattleController.enemyArmy.UnitsCountUpdate(), mapBattleController.enemyArmy.ArmyPowerUpdate(),0);

        StartCoroutine(SpawnSquadsPanel());
    }

        public void BattlePanelUpdate()
    {
        //BattlePanelClear();

        //int reward = 0;

        //StartCoroutine(SpawnSquadsPanel());

        //playerArmyUnitsCountText.text = "" + mapBattleController.playerArmy.UnitsCountUpdate();
        //enemyArmyUnitsCountText.text = "" + mapBattleController.enemyArmy.UnitsCountUpdate();


        //playerArmyPowerText.text = "" + mapBattleController.playerArmy.ArmyPowerUpdate();

        //enemyArmyPowerText.text = "" + mapBattleController.enemyArmy.ArmyPowerUpdate();

        //rewardText.text = "" + reward;
        playerArmyPanel.UpdateArmyInfo(mapBattleController.playerArmy.UnitsCountUpdate(), mapBattleController.playerArmy.ArmyPowerUpdate(),0);
        enemyArmyPanel.UpdateArmyInfo(mapBattleController.enemyArmy.UnitsCountUpdate(), mapBattleController.enemyArmy.ArmyPowerUpdate(),0);

        playerArmyPanel.UpdateSquadsInfo(mapBattleController.playerArmy.squadList.ToArray());
        enemyArmyPanel.UpdateSquadsInfo(mapBattleController.enemyArmy.squadList.ToArray());

        playerArmyPanel.UpdateSquadsIndicators(mapBattleController.playerArmy.squadList.ToArray());
        enemyArmyPanel.UpdateSquadsIndicators(mapBattleController.enemyArmy.squadList.ToArray());

    float playerP = mapBattleController.playerArmy.ArmyPowerUpdate();
        float enemyP = mapBattleController.enemyArmy.ArmyPowerUpdate();



         vicChanse = playerP / (playerP + enemyP);

        victoryBar.transform.localScale = new Vector3(vicChanse, 1, 1);

        victoryChanseText.text = Mathf.RoundToInt(vicChanse * 100f) + "%";
        victoryChanseText.color = victoryChanseGradient.Evaluate(vicChanse);

    }

    private IEnumerator SpawnSquadsPanel()
    {
        for (int i = 0; i < mapBattleController.playerArmy.squadList.Count; i++)
        {
            SquadController squad = mapBattleController.playerArmy.squadList[i];
            playerArmyPanel.AddSquad(squad.mapUIPanel);

            yield return new WaitForSeconds(0.07f);
        }

        for (int i = 0; i < mapBattleController.enemyArmy.squadList.Count; i++)
        {
            SquadController squad = mapBattleController.enemyArmy.squadList[i];
            enemyArmyPanel.AddSquad(squad.mapUIPanel);

            yield return new WaitForSeconds(0.07f);
            //reward += squad.coinsFromDie;
        }

        if (gameObject.activeSelf == true)
        {
            playerArmyPanel.UpdateSquadsInfo(mapBattleController.playerArmy.squadList.ToArray());
            enemyArmyPanel.UpdateSquadsInfo(mapBattleController.enemyArmy.squadList.ToArray());
        }
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

        BattlePanelCreate();

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




            if (vicChanse > 0.9f)
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
                autoBattleButtonObject.SetActive(false);
            }
            else if (mapBattleController.inBattle)
            {
                autoBattlePanel.SetActive(false);
                autoBattleButtonObject.SetActive(false);
                battlePanel.SetActive(true);
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
