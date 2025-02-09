using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CityController : MonoBehaviour
{
    public string cityName;

    public float cityTickTime;

    public int cityBuildings;
    public int cityUnits;

    public int cityBuildingsMax;
    public int cityUnitsMax;

    public GameObject build;
    public Transform[] spawnPoints;
    public List<GameObject> buildings;



    private float currentTime;
    private int lastBuildCoint;

    [SerializeField] public TextMeshProUGUI cityNameText;
    [SerializeField] public TextMeshProUGUI buildingsCount;
    [SerializeField] public TextMeshProUGUI buildingsCountChange;
    [SerializeField] public TextMeshProUGUI unitsCount;
    [SerializeField] public TextMeshProUGUI unitsCountChange;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < cityBuildings; i++)
        {
            AddBuildings();
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;

        if (currentTime > cityTickTime)
        {
            if (cityBuildings < cityBuildingsMax)
            {
                cityUnits += cityBuildings;
            }

            if (cityBuildings < cityBuildingsMax) {
                cityBuildings += cityUnits / 50;

                if (lastBuildCoint + 1 < cityBuildings) {
                    lastBuildCoint = cityBuildings;
                    AddBuildings();
                    }

                buildingsCount.text = "" + lastBuildCoint;

                buildingsCountChange.text = "+" + cityUnits / 50;
                
            }

            unitsCount.text = "" + cityUnits;
            unitsCountChange.text = "+" + cityBuildings;

            currentTime = 0;
        }
    }

    public void AddBuildings()
    {
        GameObject newBuild = Instantiate(build, spawnPoints[lastBuildCoint]);
        buildings.Add(newBuild);
    }
}
