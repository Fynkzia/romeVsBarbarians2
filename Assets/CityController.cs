using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class CityController : MonoBehaviour
{
    public bool isSelected;

    public bool isSmallCity;

    public bool isPlayer = true;
    public bool isCampainTarget = false;

    public string cityName;

    public float cityTickTime;

    public int cityBuildingsStart;

    public int cityBuildings;
    public float cityUnits;

    public float coinsAdd;
    public float coinTime = 60f;
    float currentCoinTime;


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
    public ArmyController armyEnemyObject;

    public Transform armyPoint;
    public Transform armyInfoPoint;
    public GameObject armyInfoObject;

    private float currentTime;
    

    

    public MapControlManager mapControlManager;
    public CityToDoController toDoController ;

    public event Action<CityController> OnCityCaptured;

    public CityInfo cityInfo;
    public ResourceManager resourceManager;

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

       

        if (armyInCity != null)
        {
            armyInCity.transform.position = armyPoint.position;
        }

        cityInfo.SetupInfo(this);

       


    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        currentCoinTime += Time.deltaTime;

        if (currentCoinTime > coinTime)
        {

            if (isPlayer)
            {
                resourceManager.ChangeAmountOfCoins((int)coinsAdd);
            }

            currentCoinTime = 0;

        }


        if (currentTime > cityTickTime)
        {
            if (cityUnits < cityUnitsMax)
            {
                cityUnits += cityBuildings/5f; 

                cityUnitsMax = cityBuildings * 5;

                coinsAdd = cityBuildings * 0.01f + cityUnits * 0.05f;




            }

            if ( cityBuildingsMax > cityBuildings+1)
            {
                if (isSmallCity)
                {
                    AddBuildings();
                }
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




            cityInfo.UpdateCounts(cityBuildings, (int)cityUnits, (int)coinsAdd);

            currentTime = 0;
        }
    }

    public void AddBuildings()
    {
        cityBuildings++;

        cityUnitsMax = cityBuildings * 5;

        coinsAdd = cityBuildings * 0.01f + cityUnits * 0.05f;

        cityInfo.UpdateCounts(cityBuildings, (int)cityUnits, (int)coinsAdd);

        if (isPlayer)
        {
            GameObject newBuild = Instantiate(playerBuild, spawnPoints[cityBuildings].position, spawnPoints[cityBuildings].rotation,transform);
            buildings.Add(newBuild);
        }
        else
        {
            GameObject newBuild = Instantiate(enemyBuild, spawnPoints[cityBuildings].position, spawnPoints[cityBuildings].rotation, transform);
            buildings.Add(newBuild);
        }

        if (isSelected) {
           mapControlManager.uiManager.cityUIPanel.UpdateCityUIPanel(this);
         }
    }

    public void CitySelected()
    {


        if (!isSmallCity)
        {
            isSelected = true;

        }
       

       

    }



    public void CityDeselect()
    {
        if (!isSmallCity)
        {
            isSelected = false;
        }

      

    }

    public void CityCaptured(bool player)
    {

        OnCityCaptured?.Invoke(this); // ивент идет на компайн манагер

        if (player)
        {
            playerMainBuild.SetActive(true);
            enemyMainBuild.SetActive(false);

            Instantiate(captureFx, transform.position, transform.rotation);

            gameObject.tag = "Player";
            if (!isSmallCity)
            {
                toDoController.enabled = true;
            }
            isPlayer = true;
        }
        else
        {
            enemyMainBuild.SetActive(true);
            playerMainBuild.SetActive(false);

            gameObject.tag = "Enemy";

            if (!isSmallCity) { 
                toDoController.enabled = false;
        }
            isPlayer = false;
        }


        for (int i = 0; i < buildings.Count; i++)
        {
            Destroy(buildings[i]);
        }

        buildings.Clear();

        cityBuildings = 1;
       

        cityUnits = 0;


        cityInfo.SetupInfo(this);

    }
}
