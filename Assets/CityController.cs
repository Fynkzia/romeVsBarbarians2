using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CityController : MonoBehaviour
{
    public string cityName;

    public float cityTickTime;

    public int cityBuildingsStart;

    public int cityBuildings;
    public int cityUnits;

    public int cityBuildingsMax;
    public int cityUnitsMax;

    public GameObject build;
    public Transform[] spawnPoints;
    public List<GameObject> buildings;

    public ArmyController armyInCity;
    public ArmyController armyObject;

    private float currentTime;
    private int untisToBuild;

    [SerializeField] public TextMeshProUGUI cityNameText;
    [SerializeField] public TextMeshProUGUI buildingsCount;
    [SerializeField] public TextMeshProUGUI buildingsCountChange;
    [SerializeField] public TextMeshProUGUI unitsCount;
    [SerializeField] public TextMeshProUGUI unitsCountChange;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cityBuildingsStart; i++)
        {
            cityBuildings++;
            AddBuildings();
        }

        buildingsCount.text = "" + cityBuildings;

        buildingsCountChange.text = "+" + cityBuildings / 50;

        unitsCount.text = "" + cityUnits;
        unitsCountChange.text = "+" + cityBuildings;

        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > cityTickTime)
        {
            if (cityUnits < cityUnitsMax)
            {
                cityUnits += cityBuildings;
               
            }

            untisToBuild += cityBuildings;

            if (cityBuildings < cityBuildingsMax) {
           

                if (untisToBuild > 50) {
                   
                    AddBuildings();
                    cityBuildings++;

                    untisToBuild = 0;
                    }

                buildingsCount.text = "" + cityBuildings;

                buildingsCountChange.text = "+" + cityBuildings / 50;
                
            }

            unitsCount.text = "" + cityUnits;
            unitsCountChange.text = "+" + cityBuildings;

            currentTime = 0;
        }
    }

    public void AddBuildings()
    {
        GameObject newBuild = Instantiate(build, spawnPoints[cityBuildings]);
        buildings.Add(newBuild);
    }
}
