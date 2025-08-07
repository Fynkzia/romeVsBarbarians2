using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class CityUIPanel : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI cityNameText;
    [SerializeField] public TextMeshProUGUI buildingsCount;
    [SerializeField] public TextMeshProUGUI buildingsCountChange;
    [SerializeField] public TextMeshProUGUI unitsCount;
    [SerializeField] public TextMeshProUGUI unitsCountChange;

    public void UpdateCityUIPanel(CityController city)
    {
        cityNameText.text = city.cityName;

        buildingsCount.text = "" + city.cityBuildings;
        // buildingsCountChange.text = "" + city.cityBuildings;

        unitsCount.text = "" + (int)city.cityUnits;
        unitsCountChange.text = "(+" + Math.Round(city.cityBuildings / 120f, 2) + ")"; 
    }
}
