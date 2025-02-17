using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadMapUIPanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public TextMeshProUGUI unitsAmmount;
    [SerializeField] public TextMeshProUGUI squadName;
    [SerializeField] public Image unitType;
    [SerializeField] public Image countBar;
    [SerializeField] public Image moraleBar;

    [SerializeField] public Image createProgressBar;

    [SerializeField] public Image[] unitIcons;

    [SerializeField] public TextMeshProUGUI nowUnits;
    [SerializeField] public TextMeshProUGUI nowCoins;


    public void UpdateSquadUiPanel(SquadController squad)
    {
        unitsAmmount.text = "" + squad.currentAmountUnits;

        unitsAmmount.text = "" + squad.currentAmountUnits;

        countBar.fillAmount = squad.currentAmountUnits / squad.amountUnits;

    }

}

