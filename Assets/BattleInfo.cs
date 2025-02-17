using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfo : MonoBehaviour
{
    

    public int sceneIndex;
    public bool isSelect;

    public Button toBattleButton;

    public GameObject toBattleButtonObject;
    public GameObject autoBattleObject;

    public Image startbattleTimer;

    public ArmyController playerArmy;
    public ArmyController enemyArmy;

    public Canvas canvas;

    public MapBattleController mapBattleController;

    public TextMeshProUGUI unitsCountPlayerText;
    public TextMeshProUGUI unitsCountEnemyText;

    public TextMeshProUGUI powerCountPlayerText;
    public TextMeshProUGUI powerCountEnemyText;

    public Image moraleCountPlayerBar;
    public Image moraleCountEnemyrBar;

    public Transform playerSquadsPanel;
    public Transform enemySquadsPanel;

    public List<SquadMapUIPanel> playerSquadsPanelList;
    public List<SquadMapUIPanel> enemySquadsPanelList;

    [SerializeField] private Animator animator;
    MapSceneManager mapSceneManage;

    private void Awake()
    {
        
        mapSceneManage = GameObject.Find("MapSceneManager").GetComponent<MapSceneManager>();
        canvas.worldCamera = mapSceneManage.gameCamera;

    }


    public void Select(bool select)
    {
        isSelect = select;

        if (select)
        {
            UpdateArmyInfo();
            animator.SetBool("Select",true);
        }
        else
        {
            animator.SetBool("Select", false);
        }

        
    }

    public void SelectBattle()
    {
        if (mapSceneManage != null)
        {
            mapSceneManage.controlController.SelectBattle(this);
        }
        else
        {
            mapSceneManage = GameObject.Find("MapSceneManager").GetComponent<MapSceneManager>();
            mapSceneManage.controlController.SelectBattle(this);
        }


        


    }

    public void AutoBattleStart()
    {
        
       
            animator.SetTrigger("AutoBattle");

        toBattleButtonObject.SetActive(false);
        autoBattleObject.SetActive(true);

    }

    public void BattleStart()
    {


        animator.SetTrigger("ToBattle");


        startbattleTimer.gameObject.SetActive(false);

    }

    public void StartBattleTimerUpdate(float fillAmount)
    {
        startbattleTimer.fillAmount = fillAmount;
    }

    public void UpdateArmyInfo()
    {
        ClearSqudsList();
        CreateSqudsList();

        playerArmy.ArmyPowerUpdate();
        enemyArmy.ArmyPowerUpdate();

        powerCountPlayerText.text = "pwr." + playerArmy.armyPower;
        powerCountEnemyText.text = "pwr." + enemyArmy.armyPower;

        unitsCountPlayerText.text = "" + playerArmy.UnitsCountUpdate(); ;
        unitsCountEnemyText.text = "" + enemyArmy.UnitsCountUpdate(); ;

    }

    public void ClearSqudsList()
    {
        for (int i = 0; i < playerSquadsPanelList.Count; i++)
        {
            Destroy(playerSquadsPanelList[i].gameObject);
        }
        for (int i = 0; i < enemySquadsPanelList.Count; i++)
        {
            Destroy(enemySquadsPanelList[i].gameObject);
        }

        playerSquadsPanelList.Clear();
        enemySquadsPanelList.Clear();

    }

    public void CreateSqudsList()
    {
        for (int i = 0; i < playerArmy.squadList.Count; i++)
        {
            SquadMapUIPanel newPanel = Instantiate(playerArmy.squadList[i].mapUIPanel, playerSquadsPanel);
            playerSquadsPanelList.Add(newPanel);
            newPanel.UpdateSquadUiPanel(playerArmy.squadList[i]);
        }
        for (int i = 0; i < enemyArmy.squadList.Count; i++)
        {
            SquadMapUIPanel newPanel = Instantiate(enemyArmy.squadList[i].mapUIPanel, enemySquadsPanel);
            enemySquadsPanelList.Add(newPanel);
            newPanel.UpdateSquadUiPanel(enemyArmy.squadList[i]);
        }

    

    }

}
