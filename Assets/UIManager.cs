using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class UIManager : MonoBehaviour
{
    [SerializeField] public ArmyUIPanel armyUIPanel;
    [SerializeField] public CityUIPanel cityUIPanel;
    [SerializeField] public HireUIPanel hireUIPanel;
    [SerializeField] public BattleUIPanel battleUIPanel;
    [SerializeField] public SquadStatsUIPanel squadStatsUIPanel;
    [SerializeField] public WinScreenUIPanel winScreen;
    [SerializeField] public GameObject pausePanel;

    [SerializeField] public CoinsInfo coinsUIPanel;
    [SerializeField] public GameObject mapUiObject;
    [SerializeField] public GameObject battleUiObject;

    [SerializeField] public RawImage rawBattleUiPanel;

    [SerializeField] public Button mapButton;
    [SerializeField] public Button newBattleButton;
    [SerializeField] public List<Button> battleButtons;
    [SerializeField] public List<GameObject> alertObject;
    [SerializeField] public Transform activeSceneSprite;

    [SerializeField] private Transform battlePowerBar;
    [SerializeField] public List<GameObject> reinforcementsPlayer;
    [SerializeField] public List<GameObject> reinforcementsEnemy;

    public Animator transitionAnimation;
    public Animator WinEffect;
    public Animator LoseEffect;

    public GameObject campainWinScreen;
    public GameObject campainLoseScreen;

    [SerializeField] public Button campainGoalButton;

    public GameObject upperUIPanel;

    ArmyController selectedArmy;




    // Start is called before the first frame update
    void Start()
    {
        UIReset();

        //transitionAnimation.SetTrigger("In");
        //transitionAnimation.SetTrigger("Out");
    }


    public void StatsUIPanelActivation(SquadController squad)
    {
       // UIReset();
        squadStatsUIPanel.gameObject.SetActive(true);
        squadStatsUIPanel.UpdateStatUIPanle(squad);
    }

    public void StatsUIPanelDeactivtion()
    {
        squadStatsUIPanel.gameObject.SetActive(false);
        
    }

    public void NewBattleButtonActivate(int sceneIndex,bool active)
    {
        if (active)
        {
            newBattleButton.gameObject.SetActive(true);
         
        }
        else
        {
            newBattleButton.gameObject.SetActive(false);
        }

    }

        public void UpdateActiveSceneSpritePosition(int activeScene)
    {
        Vector3 newPos;
        if (activeScene == -1)
        {
            newPos = new Vector3(mapButton.transform.position.x, activeSceneSprite.position.y, 0f);
        }
        else
        {
            newPos = new Vector3(battleButtons[activeScene].transform.position.x, activeSceneSprite.position.y, 0f);

            if (alertObject[activeScene].active == true)
            {
                alertObject[activeScene].SetActive(false);
            }
        }

        activeSceneSprite.position = newPos;

        
    }

    public void CityUIActivation(CityController city)
    {
        armyUIPanel.gameObject.SetActive(false);
        cityUIPanel.gameObject.SetActive(true);
        cityUIPanel.UpdateCityUIPanel(city);
    }

    public void ArmyUIActivation(ArmyController army)
    {
        armyUIPanel.gameObject.SetActive(true);

        armyUIPanel.Clear();

        selectedArmy = army;
        StartCoroutine(SpawnSquadsPanel(army));



       
    }

    private IEnumerator SpawnSquadsPanel(ArmyController army)
    {
        armyUIPanel.UpdateArmyInfo(army.UnitsCountUpdate(), army.ArmyPowerUpdate(), army.squadList.Count);

        for (int i = 0; i < army.squadList.Count; i++)
        {
            SquadMapUIPanel newSquadPanel = armyUIPanel.AddSquad(army.squadList[i].mapUIPanel);

            int index = i;
            //newSquadPanel.GetComponent<Button>().onClick.AddListener(() => army.SquadSelect(index));
            newSquadPanel.CanShowStats(true, army.squadList[i], this);

            yield return new WaitForSeconds(0.07f);
        }

        armyUIPanel.UpdateSquadsInfo(army.squadList.ToArray());
        
    }


    public void ArmyUIListUpdate(ArmyController army)
    {

        //armyUIPanel.Clear();
        armyUIPanel.UpdateSquadsInfo(army.squadList.ToArray());
        armyUIPanel.UpdateArmyInfo(army.UnitsCountUpdate(), army.ArmyPowerUpdate(), army.squadList.Count);
    }

    // Update is called once per frame
        public void BattlePanelActivation(MapBattleController battleController)
    {
        UIReset();

        battleUIPanel.mapBattleController = battleController;
        battleUIPanel.gameObject.SetActive(true) ;

        battleUIPanel.BattlePanelActivate();

        upperUIPanel.SetActive(false);


    }

    public void BattlePanelDeactivation()
    {

       

       
        battleUIPanel.gameObject.SetActive(false);

       

        upperUIPanel.SetActive(true);
    }

    public void BattlePanelUpdate(MapBattleController battleController)
    {

        battleUIPanel.BattlePanelUpdate();
    }
    public void UIReset()
    {
        armyUIPanel.gameObject.SetActive(false);
        cityUIPanel.gameObject.SetActive(false);
        hireUIPanel.gameObject.SetActive(false);
        battleUIPanel.gameObject.SetActive(false);
        squadStatsUIPanel.gameObject.SetActive(false);

        battleUIPanel.BattlePanelClear();

        upperUIPanel.SetActive(true);

        if(selectedArmy != null)
        {
            selectedArmy.ArmyDeselect();
        }

        winScreen.gameObject.SetActive(false);
    }

    public void MapUiActivation(bool active)
    {
        mapUiObject.SetActive(active);
        battleUiObject.SetActive(!active);
        UIReset();

        if(active == false)
        {

        }
    }


    //Battle UI

    public void UpdateBattlePowerBar(float bar)
    {
        battlePowerBar.localScale = new Vector3(bar, 1, 1);
    }

    public void UpdateBattleReinforcementsCount(int player, int enemy)
    {
        for (int i = 0; i < reinforcementsPlayer.Count; i++)
        {
            reinforcementsPlayer[i].SetActive(false);
        }

        for (int i = 0; i < reinforcementsEnemy.Count; i++)
        {
            reinforcementsEnemy[i].SetActive(false);
        }


        for (int i = 0; i < player; i++)
        {
            reinforcementsPlayer[i].SetActive(true);
        }

        for (int i = 0; i < enemy; i++)
        {
            reinforcementsEnemy[i].SetActive(true);
        }
    }
    //Battle UI


    public void TransitionAnimation(bool active)
    {
        if (active)
        {
            transitionAnimation.SetTrigger("In");
        }
        else
        {
            transitionAnimation.SetTrigger("Out");
        }
    }

    public void CampainWinLoose(bool win)
    {
        if (win)
        {
            campainWinScreen.SetActive(true);
        }
        else
        {
            campainLoseScreen.SetActive(true);
        }
    }
    public void BattleWinLoose(bool win,int kills, int loses, int xp, int coins)
    {
        if (win)
        {
            //WinEffect.gameObject.SetActive(true);
            winScreen.PanelInit(win,kills, loses, xp, coins);
        }
        else
        {
            //LoseEffect.gameObject.SetActive(true);

            winScreen.PanelInit(win, kills, loses, xp, coins);
        }

       
    }

    public void PausePanelActvation(bool pause)
    {

        if (pause)
        {
            pausePanel.gameObject.SetActive(true);

            Time.timeScale = 0;
        }
        else
        {
            pausePanel.gameObject.SetActive(false);

            Time.timeScale = 1;
        }
    }

    public void GoToMenuButton()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);


    }
}
