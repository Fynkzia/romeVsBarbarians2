using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCityMapController : MonoBehaviour
{
    public bool isPlayer = true;

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


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
