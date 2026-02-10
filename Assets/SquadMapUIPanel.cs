using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadMapUIPanel : MonoBehaviour
{
    // Start is called before the first frame update
    Button button;

    [SerializeField] public TextMeshProUGUI unitsAmmount;
    [SerializeField] public TextMeshProUGUI unitsMaxAmmount;
    [SerializeField] public TextMeshProUGUI levelText;

    [SerializeField] public TextMeshProUGUI squadName;
    [SerializeField] public Image unitType;
    [SerializeField] public Image countBar;
    [SerializeField] public Transform moraleBar;
    [SerializeField] public Image moraleBarImage;
    [SerializeField] private Gradient moraleGradient;

    [SerializeField] public Image retreatIndicator;
    [SerializeField] public Image damageIndicator;

    [SerializeField] public Image createProgressBar;

    [SerializeField] public Image[] unitIcons;

    [SerializeField] public TextMeshProUGUI nowUnits;
    [SerializeField] public TextMeshProUGUI nowCoins;

    [SerializeField]  Color defaultPriceColor;
    [SerializeField] Color redPriceColor;

    [SerializeField] public Image buyButton;

    [SerializeField] Color defaultButtonColor;
    [SerializeField] Color redButtonColor;


    public void UpdateSquadUiPanel(SquadController squad)
    {
        unitsAmmount.text = "" + squad.currentAmountUnits;
        unitsMaxAmmount.text = "/" + squad.amountUnits;
        levelText.text = "" +squad.levelSquad;



        countBar.fillAmount = 1f - (squad.currentAmountUnits / squad.amountUnits);

        //moraleBar.ChangeProgress(squad.currentMorale, squad.maxMorale) ;
        moraleBar.localScale = new Vector3(1f, squad.currentMorale / squad.maxMorale, 1f);
        moraleBarImage.color = moraleGradient.Evaluate(squad.currentMorale / squad.maxMorale);

       
    }

    public void UpdateBattleIndicators(SquadController squad)
    {
        if(squad.currentMorale/squad.maxMorale < 0.35)
        {
            retreatIndicator.gameObject.SetActive(true);
        }
        else
        {
            retreatIndicator.gameObject.SetActive(false);
        }

        if (squad.damaged)
        {
            damageIndicator.gameObject.SetActive(true);
        }
        else
        {
            damageIndicator.gameObject.SetActive(false);
        }
    }

        public void CanShowStats(bool canShow, SquadController squad,UIManager uimanager)
    {
        if(button == null)
        {
            button = GetComponent<Button>();
        }

        if (canShow)
        {

            button.onClick.RemoveAllListeners();

            button.onClick.AddListener(()=> ShowStatButton(squad, uimanager));
        }
        else
        {

        }
    }

    public void ShowStatButton(SquadController squad, UIManager uimanager)
    {

        uimanager.StatsUIPanelActivation(squad);
    }


        public void UpdateToDoCosts(float units, float coins)
    {
        nowUnits.text = "" + units;

        nowCoins.text = "" + coins;

        

    }

    public void UpdateToDoPriceColors(bool units, bool coins)
    {

       
        if (units)
        {
            nowUnits.color = redPriceColor;
        }
        else
        {
            nowUnits.color = defaultPriceColor;
        }


        if (coins)
        {
            nowCoins.color = redPriceColor;
        }
        else
        {
            nowCoins.color = defaultPriceColor;
        }

        if (coins || units)
        {

            buyButton.color = redButtonColor;
            Debug.Log("UpdateToDoPriceColors");
        }
        else
        {
            buyButton.color = defaultButtonColor;
        }

    }

}

