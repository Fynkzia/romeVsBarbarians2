using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CityInfo : MonoBehaviour
{

    [SerializeField] public TextMeshProUGUI cityNameText;
    [SerializeField] public TextMeshProUGUI buildingsCount;
    [SerializeField] public TextMeshProUGUI buildingsCountChange;
    [SerializeField] public TextMeshProUGUI unitsCount;
    [SerializeField] public TextMeshProUGUI unitsCountChange;

    [SerializeField] public TextMeshProUGUI coinsText;

    [SerializeField] public Image colorLine;

    [SerializeField] public Color playerColor;
    [SerializeField] public Color enemyColor;


    // Start is called before the first frame update
    public void UpdateCounts(int buildings, int units, int coins)
    {
        unitsCount.text = "" + units;
        buildingsCount.text = "" + buildings;

        coinsText.text = "+" + coins + "";
    }

    // Update is called once per frame
    public void SetupInfo(CityController city)
    {
        unitsCount.text = "" + city.cityUnits;
        buildingsCount.text = "" + city.cityBuildings;

        cityNameText.text = "" + city.cityName;

        if (city.isPlayer)
        {
            colorLine.color = playerColor;
        }
        else
        {
            colorLine.color = enemyColor;
        }
    }
}
