using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Random = UnityEngine.Random;



public class CityController : MonoBehaviour
{
    public bool isSelected;

    public bool isSmallCity;
    public bool inBattle;

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
    public Transform armySelectPoint;



    private float currentTime;

    [SerializeField] private float shakeDuration = 1f; // Длительность тряски
    [SerializeField] private float shakeIntensity = 1f; // Интенсивность тряски
    [SerializeField] private float decayRate = 1f;

    [SerializeField] private GameObject meshObject;
    

    private float currentShakeDuration; // Оставшееся время тряски
    private Vector3 originalPosition; // Исходное положение объекта




    public MapControlManager mapControlManager;
    public CityToDoController toDoController ;

    public event Action<CityController> OnCityCaptured;

    public CityInfo cityInfo;
    public ResourceManager resourceManager;
    public MapAIGlobal aIController;
    public Collider selectCollider;

    public AddCoinsEffect coinFx;

    public float DistanceTo(Vector3 p) => Vector3.Distance(transform.position, p);


    // Start is called before the first frame update
    void Start()
    {
        if(mapControlManager == null)
        {
            mapControlManager = GameObject.Find("MapControlManager").GetComponent<MapControlManager>();
        }

        if (isPlayer)
        {
            playerMainBuild.SetActive(true);
            enemyMainBuild.SetActive(false);



            gameObject.tag = "Player";
            if (!isSmallCity)
            {
                toDoController.enabled = true;
            }

        }
        else
        {
            enemyMainBuild.SetActive(true);
            playerMainBuild.SetActive(false);

            gameObject.tag = "Enemy";

            if (!isSmallCity)
            {
                toDoController.enabled = true;
            }

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

        cityBuildingsMax = spawnPoints.Length-1;

        originalPosition = meshObject.transform.localPosition;
        currentShakeDuration = 0;

        

    }

    // Update is called once per frame
    void Update()
    {
        if (!inBattle)
        {
            currentTime += Time.deltaTime;
            currentCoinTime += Time.deltaTime;

            if (currentCoinTime > coinTime)
            {

                if (isPlayer)
                {
                    resourceManager.ChangeAmountOfCoins((int)coinsAdd);

                    AddCoinsEffect coins = Instantiate(coinFx, transform);

                    coins.transform.position = transform.position;

                    coins.transform.rotation = Quaternion.Euler(0,-77,0);

                    coins.SetEffect((int)coinsAdd);
                }
                else
                {
                    aIController.aICities.enemyCoins += (int)coinsAdd;
                }

                currentCoinTime = 0;

            }
            else
            {
                if (isPlayer)
                {
                    cityInfo.UpdateCoinBar(currentCoinTime / coinTime);
                }
            }


            if (currentTime > cityTickTime)
            {
                if (cityUnits < cityUnitsMax)
                {
                    cityUnits += cityBuildings / 5f;

                    cityUnitsMax = cityBuildings * 5;

                    coinsAdd = cityBuildings * 0.01f + cityUnits * 0.05f;




                }

                if (cityBuildingsMax > cityBuildings + 1)
                {
                    if (isSmallCity)
                    {
                        AddBuildings();
                    }
                }

                if (cityUnits > 0)
                {
                    if (armyInCity != null)
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


            if (currentShakeDuration > 0)
            {
                // Рассчитываем смещение тряски
                float damping = Mathf.Clamp01(currentShakeDuration / shakeDuration);
                float offsetX = Random.Range(-1f, 1f) * shakeIntensity * damping;
                float offsetY = Random.Range(-1f, 1f) * shakeIntensity * damping;

                // Применяем смещение к объекту
                meshObject.transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

                // Уменьшаем время тряски с учетом затухания
                currentShakeDuration -= Time.deltaTime * decayRate;

                // Если тряска закончилась, возвращаем объект на исходную позицию
                if (currentShakeDuration <= 0)
                {
                    meshObject.transform.localPosition = originalPosition;
                }
            }
        }
    }

    public void GetDamage(int armypower)
    {
        int multiplayer = 1 + (armypower / 400);
        if (cityUnits > 0)
        {
           

            cityUnits -= 3 * multiplayer;

        }

        if (cityBuildings > 0)
        {
            for (int i = 0; i < multiplayer; i++)
            {
                if (cityBuildings > 0)
                {
                    cityBuildings--;

                    Destroy(buildings[buildings.Count - 1]);

                    buildings.RemoveAt(buildings.Count - 1);
                }
            }

            

            cityInfo.UpdateCounts(cityBuildings, (int)cityUnits, (int)coinsAdd);

            currentShakeDuration = shakeDuration;
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

       


            if (armyInCity != null && armyInCity.isPlayer)
            {
               // armyInCity.SelectInCity();
            }
        }

    }

    public void CityInBattle(bool state)
    {
        if (state)
        {
            inBattle = true;
            
            selectCollider.enabled = false;

            if (!isSmallCity)
            {
                toDoController.inBattle = true;
            }
        }
        else
        {
            inBattle = false;
            
            selectCollider.enabled = true;

            if (!isSmallCity)
            {
                toDoController.inBattle = false;
            }
        }



    }

    public void CityDeselect()
    {
        if (!isSmallCity)
        {
            isSelected = false;


            if (armyInCity != null && armyInCity.isPlayer)
            {
                //armyInCity.DeselectInCity();
            }
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
                toDoController.enabled = true;
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
