using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class BattleSceneManager : MonoBehaviour
{
    public int sceneIndex;
    public ArmyController playerArmy;
    public ArmyController enemyArmy;

    public CityController city;

    public MapBattleController mapBattleController;

   

    public SquadControlManager controlController;
    public OfficerSystem officerSystem;
    public AIController aIController;
    public WinLoseManager winLoseManager;

     public Camera gameCamera;
    public CameraMovement cameraController;
    public GameObject cinemachineGameObject;
    public GameObject lightGameObject;

    
    

    public Transform[] playerSpawnPoints;
    public Transform[] enemySpawnPoints;

    public Transform[] CitySpawnPoints;


    public GameObject spawnPointPrefab;
    public GameObject formationPointPrefab;

    public List<GameObject> playerSquads;
    public List<GameObject> enemySquads;

    public Transform playerSquadParent;
    public Transform enemySquadsParent;

    public bool isInit;
    public bool IsCity;

    public bool IsWin;
    public bool IsDraft;

    public float delayFade;
    public float afterBattleDelay;

    public Transform playerHousesParent;
    public Transform enemyHousesParent;
    

    public Transform fancesParent;
    public Transform towersParent;

    public ResourceManager resourceManager;
    

    public event Action<bool> NotAnimate;

    public Terrain terrain;


    public void InitScene()
    {



        Debug.Log("InitScene - playerSquads - " + playerSquads.Count);
        for (int i = 0; i < playerSquads.Count; i++)
        {
            playerSquads[i].GetComponent<SquadController>().InitSquad();

        }

        for (int i = 0; i < enemySquads.Count; i++)
        {
            enemySquads[i].GetComponent<SquadController>().InitSquad();

        }

        winLoseManager.Init();
        officerSystem.Init();
        cameraController.Init();
        

        cameraController.CamToPoint(playerSquads[0].transform.position);

        
        isInit = true;
        InitMap();
    }


    public void InitMap()
    {
        if (IsCity)
        {
            if (city.isPlayer) { /// тупа для оптимизашки, шоб меньше щелкало ебалом

               
                playerHousesParent.gameObject.SetActive(true);

                for (int i = 0; i < playerHousesParent.childCount; i++)
                {
                    playerHousesParent.GetChild(i).gameObject.SetActive(false);
                }
            }
            else
            {
                enemyHousesParent.gameObject.SetActive(true);

                for (int i = 0; i < enemyHousesParent.childCount; i++)
                {
                    enemyHousesParent.GetChild(i).gameObject.SetActive(false);
                }
            }
           
        }
        else /// потом переделать на узнование какая область игрока или врага
        {
            enemyHousesParent.gameObject.SetActive(true);

            for (int i = 0; i < enemyHousesParent.childCount; i++)
            {
                enemyHousesParent.GetChild(i).gameObject.SetActive(false);
            }
        }


        if (IsCity)
        {
            int housesCount = city.cityBuildings/5;

            if (city.isPlayer)
            {
                for (int i = 0; i < housesCount; i++)
                {
                    playerHousesParent.GetChild(i).gameObject.SetActive(true);
                }

                for (int i = 0; i < playerHousesParent.childCount; i++)
                {
                    if (playerHousesParent.GetChild(i).gameObject.activeSelf)
                    {

                        if (Random.Range(1, 100) > 70)
                        {
                            playerHousesParent.GetChild(i).GetComponent<SquadSpawnerController>().Init();
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < housesCount; i++)
                {
                    enemyHousesParent.GetChild(i).gameObject.SetActive(true);
                }

                for (int i = 0; i < enemyHousesParent.childCount; i++)
                {
                    if (enemyHousesParent.GetChild(i).gameObject.activeSelf)
                    {

                        if (Random.Range(1, 100) > 70)
                        {
                            enemyHousesParent.GetChild(i).GetComponent<SquadSpawnerController>().Init();
                        }
                    }
                }
            }


            

        }
        else
        {
            for (int i = 0; i < enemyHousesParent.childCount; i++)
            {
                if (Random.Range(1, 100) > 50)
                {
                    enemyHousesParent.GetChild(i).gameObject.SetActive(true);
                }

                if (Random.Range(1, 100) > 50)
                {
                    enemyHousesParent.GetChild(i).GetComponent<SquadSpawnerController>().Init();
                }
            }

        }

    }


        public void DiePlayerSquad(SquadController squadController)
    {
        playerArmy.squadList.Remove(squadController);
        squadController.gameObject.SetActive(false);

        if (playerArmy.squadList.Count == 0 || enemyArmy.squadList.Count == 0)
        {
            StartCoroutine(EndBattleSiquence());
        }
    }
    public void DieEnemySquad(SquadController squadController)
    {
        enemyArmy.squadList.Remove(squadController);

        squadController.gameObject.SetActive(false);
        if (playerArmy.squadList.Count == 0 || enemyArmy.squadList.Count == 0)
        {
            StartCoroutine(EndBattleSiquence());
        }

        resourceManager.ChangeAmountOfCoins(squadController.coinsFromDie);


    }

    private IEnumerator EndBattleSiquence() {

        playerArmy.squadList.Clear();
        Debug.Log("EndBattleSiquence " + playerSquadParent.childCount);  
        for (int i = 0; i < playerSquadParent.childCount; i++)
        {
            if (playerSquadParent.GetChild(i).gameObject.activeSelf) {
                SquadController squad = playerSquadParent.GetChild(i).GetComponent<SquadController>();


                if (!squad.squadDie && !squad.helperSquad)
                {

                    playerArmy.squadList.Add(squad);


                }
                
            }
        }

        for (int i = 0; i < playerArmy.squadList.Count; i++)
        {
            playerArmy.squadList[i].Reset();
            playerArmy.squadList[i].transform.parent = playerArmy.transform;
            playerArmy.squadList[i].gameObject.SetActive(false);
        }

        enemyArmy.squadList.Clear();

        for (int i = 0; i < enemySquadsParent.childCount; i++)
        {
            if (enemySquadsParent.GetChild(i).gameObject.activeSelf)
            {
                SquadController squad = enemySquadsParent.GetChild(i).GetComponent<SquadController>();



                if (!squad.squadDie && !squad.helperSquad)
                {
                    enemyArmy.squadList.Add(squad);
                }
            }
        }

        for (int i = 0; i < enemyArmy.squadList.Count; i++)
        {
            enemyArmy.squadList[i].Reset();
            enemyArmy.squadList[i].transform.parent = enemyArmy.transform;
            enemyArmy.squadList[i].gameObject.SetActive(false);
        }


        if (playerArmy.squadList.Count == 0) {

            IsWin = false;
            

        } else if (enemyArmy.squadList.Count == 0) {
           

          
            IsWin = true;


        }
        else
        {
            IsDraft = true;
        }

        yield return new WaitForSeconds(afterBattleDelay);
        ExitBattleSiquence();

        yield return new WaitForSeconds(delayFade + 1f);
        if (IsWin)
        {
            SceneLoader.Instance.BattleWin(sceneIndex);
        }
        else
        {
            SceneLoader.Instance.BattleLose(sceneIndex);
        }

    }



    public void SpawnFormationPoints()
    {

        Vector3 offcetToCenter = new Vector3(200f, 15f, 200f);

        Vector3 dirPlNorm = Quaternion.Euler(0, 90, 0) * (new Vector3(playerArmy.transform.position.x, mapBattleController.transform.position.y, playerArmy.transform.position.z) - mapBattleController.transform.position).normalized;
        Vector3 dirEnNorm = Quaternion.Euler(0, 90, 0) * (new Vector3(enemyArmy.transform.position.x, mapBattleController.transform.position.y, enemyArmy.transform.position.z) - mapBattleController.transform.position).normalized  ;


        Debug.Log("dirPlNorm - " + dirPlNorm);

        Debug.Log("dirEnNorm - " + dirEnNorm);

        Vector3 positionPlayerArmy = (transform.position + offcetToCenter) + dirPlNorm * 150f;
        Vector3 positionEnemyArmy = (transform.position + offcetToCenter) + dirEnNorm * 150f;


        if (!playerArmy.inCity)
        {
            GameObject newFormation = Instantiate(formationPointPrefab.gameObject, positionPlayerArmy, Quaternion.identity, transform);

            newFormation.transform.rotation = Quaternion.LookRotation(dirPlNorm);

            playerSpawnPoints = new Transform[newFormation.transform.childCount];

            for (int i = 0; i < newFormation.transform.childCount; i++)
            {
                newFormation.transform.GetChild(i).GetComponent<SpawnPoint>().isPlayer = true;
                playerSpawnPoints[i] = newFormation.transform.GetChild(i);
            }

        }
        else
        {
            playerSpawnPoints = CitySpawnPoints;
        }

        if (!enemyArmy.inCity)
        {

            GameObject newEnFormation = Instantiate(formationPointPrefab.gameObject, positionEnemyArmy, Quaternion.identity, transform);

            newEnFormation.transform.rotation = Quaternion.LookRotation(dirEnNorm);

            enemySpawnPoints = new Transform[newEnFormation.transform.childCount];
            for (int i = 0; i < newEnFormation.transform.childCount; i++)
            {
                newEnFormation.transform.GetChild(i).GetComponent<SpawnPoint>().isPlayer = false;
                enemySpawnPoints[i] = newEnFormation.transform.GetChild(i);
            }
        }
        else
        {
            enemySpawnPoints = CitySpawnPoints;
        }

    }

      


    public void EnterBattleScene()
    {
        gameCamera.gameObject.SetActive(true);
        lightGameObject.gameObject.SetActive(true);
        cinemachineGameObject.gameObject.SetActive(true);
        cameraController.gameObject.SetActive(true);
        controlController.gameObject.SetActive(true);



        NotAnimate?.Invoke(false);

       
        EnterBattleSceneAnimation();

        
    }


    public void ExitBattleScene()
    {
        gameCamera.gameObject.SetActive(false);
        lightGameObject.gameObject.SetActive(false);

        cinemachineGameObject.gameObject.SetActive(false);
        cameraController.gameObject.SetActive(false);
        controlController.gameObject.SetActive(false);




        NotAnimate?.Invoke(true);
    }

    public void ExitBattleSiquence()
    {
        
        cameraController.CameraMoveToExit();
        StartCoroutine(ExitSiquence());
       
    }

    public void EnterBattleSceneAnimation()
    {
        StartCoroutine(EnterSiquence());
    }

    private IEnumerator ExitSiquence()
    {
       

        
        yield return new WaitForSeconds(delayFade);

        ExitBattleScene();
    }

    private IEnumerator EnterSiquence()
    {

        cameraController.CameraSetEnterPoint();
    
        
        

        yield return new WaitForSeconds(0.2f);

        cameraController.CameraMoveEnter();


    }
}
