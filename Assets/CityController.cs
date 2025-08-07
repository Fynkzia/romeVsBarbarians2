using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using TMPro;

public class CityController : MonoBehaviour
{
    public bool isSelected;

    public bool isPlayer = true;
    public bool isCampainTarget = false;

    public string cityName;

    public float cityTickTime;

    public int cityBuildingsStart;

    public int cityBuildings;
    public float cityUnits;

    public int cityBuildingsMax;
    public int cityUnitsMax;

    public GameObject playerMainBuild;
    public GameObject enemyMainBuild;

    public GameObject playerBuild;
    public GameObject enemyBuild;

    public Transform[] spawnPoints;
    public List<GameObject> buildings;

    public GameObject captureFx;

    public ArmyController armyInCity;
    public ArmyController armyObject;

    public Transform armyPoint;

    private float currentTime;
    private int untisToBuild;

    [SerializeField] public TextMeshProUGUI cityNameText;
    [SerializeField] public TextMeshProUGUI buildingsCount;
    [SerializeField] public TextMeshProUGUI buildingsCountChange;
    [SerializeField] public TextMeshProUGUI unitsCount;
    [SerializeField] public TextMeshProUGUI unitsCountChange;

    public MapControlManager mapControlManager;

    public event Action<CityController> OnCityCaptured;



    // Start is called before the first frame update
    void Start()
    {
        if(mapControlManager == null)
        {
            mapControlManager = GameObject.Find("MapControlManager").GetComponent<MapControlManager>();
        }

       

        for (int i = 0; i < cityBuildingsStart; i++)
        {
           
            AddBuildings();
        }

        buildingsCount.text = "" + cityBuildings;

        if(armyInCity != null)
        {
            armyInCity.transform.position = armyPoint.position;
        }

       // buildingsCountChange.text = "+" + cityBuildings;

        //unitsCount.text = "" + cityUnits;
        //unitsCountChange.text = "+" + cityBuildings;

        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > cityTickTime)
        {
            if (cityUnits < cityUnitsMax)
            {
                cityUnits += cityBuildings/120f; // чтоб получить 0.5 юнита в минуту


               
            }

            if (cityUnits > 0)
            {
                if(armyInCity != null)
                {
                    if (armyInCity.RestorUnitsFromCity())
                    {

                        cityUnits--;
                    }
                }



            }



           

            unitsCount.text = "" + (int)cityUnits;

           // unitsCountChange.text = "+" + cityBuildings / 50f;

            currentTime = 0;
        }
    }

    public void AddBuildings()
    {
        cityBuildings++;

        buildingsCount.text = "" + cityBuildings;

        if (isPlayer)
        {
            GameObject newBuild = Instantiate(playerBuild, spawnPoints[cityBuildings]);
            buildings.Add(newBuild);
        }
        else
        {
            GameObject newBuild = Instantiate(enemyBuild, spawnPoints[cityBuildings]);
            buildings.Add(newBuild);
        }

        if (isSelected) {
           mapControlManager.uiManager.cityUIPanel.UpdateCityUIPanel(this);
         }
    }

    public void CitySelected()
    {
        


        isSelected = true;


       

       

    }



    public void CityDeselect()
    {
        isSelected = false;

      

    }

    public void CityCaptured(bool player)
    {

        OnCityCaptured?.Invoke(this); // ивент идет на компайн манагер

        if (player)
        {
            playerMainBuild.SetActive(true);
            enemyMainBuild.SetActive(false);

            Instantiate(captureFx, transform.position, transform.rotation);
        }
        else
        {
            enemyMainBuild.SetActive(true);
            playerMainBuild.SetActive(false);
        }


        for (int i = 0; i < buildings.Count; i++)
        {
            Destroy(buildings[i]);
        }

        buildings.Clear();

        cityBuildings = 0;
        buildingsCount.text = "" + cityBuildings;

        cityUnits = 0;
        unitsCount.text = "" + (int)cityUnits;

        isPlayer = player;

    }
}
