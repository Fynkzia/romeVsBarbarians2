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

    [SerializeField] public CoinsInfo coinsUIPanel;
    [SerializeField] public GameObject mapUiObject;
    [SerializeField] public GameObject battleUiObject;

    [SerializeField] public RawImage rawBattleUiPanel;

    [SerializeField] public Button mapButton;
    [SerializeField] public List<Button> battleButtons;
    [SerializeField] public Transform activeSceneSprite;

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
        }

        activeSceneSprite.position = newPos;
    }

    public void CityUIActivation(CityController city)
    {
        cityUIPanel.gameObject.SetActive(true);
        cityUIPanel.UpdateCityUIPanel(city);
    }

    public void ArmyUIActivation(ArmyController army)
    {
        armyUIPanel.gameObject.SetActive(true);

        armyUIPanel.Clear();

        selectedArmy = army;

        for (int i = 0; i < army.squadList.Count; i++)
        {
            SquadMapUIPanel newSquadPanel = armyUIPanel.AddSquad(army.squadList[i].mapUIPanel);

            int index = i;
            newSquadPanel.GetComponent<Button>().onClick.AddListener(() => army.SquadSelect(index));
        }

        armyUIPanel.UpdateSquadsInfo(army.squadList.ToArray(), army.UnitsCountUpdate(), army.ArmyPowerUpdate());
    }


    public void ArmyUIListUpdate(ArmyController army)
    {

        armyUIPanel.Clear();
        armyUIPanel.UpdateSquadsInfo(army.squadList.ToArray(), army.UnitsCountUpdate(), army.ArmyPowerUpdate());

    }

    // Update is called once per frame
        public void BattlePanelActivation(MapBattleController battleController)
    {
       

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

        battleUIPanel.BattlePanelClear();

        upperUIPanel.SetActive(true);

        if(selectedArmy != null)
        {
            selectedArmy.ArmyDeselect();
        }
    }

    public void MapUiActivation(bool active)
    {
        mapUiObject.SetActive(active);
        battleUiObject.SetActive(!active);
        UIReset();
    }

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
}
