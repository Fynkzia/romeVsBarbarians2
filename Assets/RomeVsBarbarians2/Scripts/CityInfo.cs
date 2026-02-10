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

    [SerializeField] public SpriteRenderer regionSprite;

    [SerializeField] public Color regionPlayerColor;
    [SerializeField] public Color regionEnemyColor;

    public Transform coinBar;

    public void UpdateCoinBar(float bar)
    {


        coinBar.localScale = new Vector3(bar, 1, 1);
    }

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
            regionSprite.color = regionPlayerColor;
        }
        else
        {
            colorLine.color = enemyColor;
            regionSprite.color = regionEnemyColor;
        }


    }
}
