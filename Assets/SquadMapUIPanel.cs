using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadMapUIPanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public TextMeshProUGUI unitsAmmount;
    [SerializeField] public TextMeshProUGUI unitsMaxAmmount;
    [SerializeField] public TextMeshProUGUI levelText;

    [SerializeField] public TextMeshProUGUI squadName;
    [SerializeField] public Image unitType;
    [SerializeField] public Image countBar;
    [SerializeField] public BarUI moraleBar;

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

