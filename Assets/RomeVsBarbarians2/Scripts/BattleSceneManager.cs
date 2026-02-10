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


    public Camera gameCamera;
    public CameraMovement cameraController;
    public GameObject cinemachineGameObject;
    public GameObject lightGameObject;




    public Transform[] playerSpawnPoints;
    public Transform[] enemySpawnPoints;

    public Transform[] CitySpawnPoints;


    public GameObject spawnPointPrefab;
    public GameObject formationPointPrefab;

    public ReinforcementEffect reinforcementEffectPrefab;

    public List<ReinforcementEffect> reinforcementList;

    public List<GameObject> playerSquads;
    public List<GameObject> enemySquads;

    public Transform playerSquadParent;
    public Transform enemySquadsParent;

    public bool isInit;
    public bool IsCity;
    public bool isSmallCity;

    public bool IsEnd;
    public bool IsWin;
    public bool IsDraft;

    public float delayFade;
    public float afterBattleDelay;
    public float timeToUptadeReinforcment;

    public float reinforcmentTime;

    public Transform playerHousesParent;
    public Transform enemyHousesParent;

    public List<SmallCityController> smallCities;




    public Transform fancesParent;
    public Transform towersParent;

    public ResourceManager resourceManager;


    public event Action<bool> NotAnimate;

    public Terrain terrain;



    int loses = 0;
    int kills = 0;
    int xp = 0;
    int coins = 0;

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


        //officerSystem.Init();
        cameraController.Init();
        // lightGameObject.gameObject.SetActive(false);


        cameraController.CamToPoint(playerSquads[0].transform.position);


        isInit = true;
        InitMap();
    }


    public void InitMap()
    {
        if (IsCity)
        {
            if (city.isPlayer)
            { /// тупа для оптимизашки, шоб меньше щелкало ебалом


                //playerHousesParent.gameObject.SetActive(true);

                //for (int i = 0; i < playerHousesParent.childCount; i++)
                //{
                //    playerHousesParent.GetChild(i).gameObject.SetActive(false);
                //}
            }
            else
            {
                //enemyHousesParent.gameObject.SetActive(true);

                //for (int i = 0; i < enemyHousesParent.childCount; i++)
                //{
                //    enemyHousesParent.GetChild(i).gameObject.SetActive(false);
                //}
            }

        }
        else if (isSmallCity) /// потом переделать на узнование какая область игрока или врага
        {

            if (!city.isPlayer)
            {
                //enemyHousesParent.gameObject.SetActive(true);

                //for (int i = 0; i < enemyHousesParent.childCount; i++)
                //{
                //    enemyHousesParent.GetChild(i).gameObject.SetActive(false);
                //}
            }
        }


        if (IsCity && !isSmallCity)
        {
            int housesCount = city.cityBuildings / 5;

            if (city.isPlayer)
            {
                //for (int i = 0; i < housesCount; i++)
                //{
                //    playerHousesParent.GetChild(i).gameObject.SetActive(true);
                //}

                for (int i = 0; i < fancesParent.childCount; i++)
                {
                    fancesParent.GetChild(i).GetComponent<BuildingManager>().SetOwner(true);
                }

                for (int i = 0; i < towersParent.childCount; i++)
                {
                    towersParent.GetChild(i).GetComponent<BuildingManager>().SetOwner(true);
                }


                smallCities[0].SetOwner(true);




                smallCities[0].unitCount = (int)city.cityUnits;
                smallCities[0].buildCount = (int)city.cityBuildings;

                smallCities[0].Init();
                smallCities[0].squadSpawnerController.Init();
            }
            else
            {
                //for (int i = 0; i < housesCount; i++)
                //{
                //    enemyHousesParent.GetChild(i).gameObject.SetActive(true);
                //}

                for (int i = 0; i < fancesParent.childCount; i++)
                {
                    fancesParent.GetChild(i).GetComponent<BuildingManager>().SetOwner(false);
                }
                for (int i = 0; i < towersParent.childCount; i++)
                {
                    towersParent.GetChild(i).GetComponent<BuildingManager>().SetOwner(false);
                }

                smallCities[0].SetOwner(false);



                smallCities[0].unitCount = (int)city.cityUnits;
                smallCities[0].buildCount = (int)city.cityBuildings;

                smallCities[0].Init();
                smallCities[0].squadSpawnerController.Init();
            }




        }
        else if (isSmallCity && IsCity)
        {
            SmallCityController smallCity = smallCities[Random.Range(0, smallCities.Count)];

            smallCity.gameObject.SetActive(true);

            if (city.isPlayer)
            {
                smallCity.SetOwner(true);
            }
            else
            {
                smallCity.SetOwner(false);

            }



            smallCity.unitCount = (int)city.cityUnits;
            smallCity.buildCount = (int)city.cityBuildings;

            smallCity.Init();
            smallCity.squadSpawnerController.Init();


            //int houses = (smallCity.buildCount / 5) + (smallCity.unitCount / 50);


            //for (int i = 0; i < houses; i++)
            //{
            //    if (i < enemyHousesParent.childCount)
            //    {
            //        if (Random.Range(1, 100) > 50)
            //        {


            //        }

            //        enemyHousesParent.GetChild(i).gameObject.SetActive(true);

            //    }
            //}

        }

    }

    public void Update()
    {
        if(reinforcementList.Count > 0)
        {
            reinforcmentTime += Time.deltaTime;

            if(reinforcmentTime > timeToUptadeReinforcment)
            {

                for (int i = 0; i < reinforcementList.Count; i++)
                {
                    reinforcementList[i].UpdateEffect();
                }


                    reinforcmentTime = 0;
            }
        }
    }


    public void DiePlayerSquad(SquadController squadController)
    {
        playerArmy.squadList.Remove(squadController);
        squadController.gameObject.SetActive(false);

       
       loses += (int)squadController.amountUnits;
        

        if (playerArmy.squadList.Count == 0 || enemyArmy.squadList.Count == 0)
        {
           
            EndBattle();
        }
        UpdateUIPowerBar(playerArmy.armyPower, enemyArmy.armyPower);
        
    }
    public void DieEnemySquad(SquadController squadController)
    {
        enemyArmy.squadList.Remove(squadController);

        kills += (int)squadController.amountUnits;
        coins += squadController.coinsFromDie;
        xp += 1;

        squadController.gameObject.SetActive(false);
        squadController.squadDie = true;


        if (playerArmy.squadList.Count == 0 || enemyArmy.squadList.Count == 0)
        {
        
            EndBattle();
        }

        resourceManager.ChangeAmountOfCoins(squadController.coinsFromDie);

        UpdateUIPowerBar(playerArmy.armyPower, enemyArmy.armyPower);
        
    }

    public void PlayerRetreat()
    {
        EndBattle();
    }



    private void EndBattle()
    {



        if (!IsEnd)
        {

            IsEnd = true;

            playerArmy.squadList.Clear();


            for (int i = 0; i < playerSquadParent.childCount; i++)
            {
                if (playerSquadParent.GetChild(i).gameObject.activeSelf)
                {
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
                    else if (squad.squadDie)
                    {

                    }
                }
            }

            for (int i = 0; i < enemyArmy.squadList.Count; i++)
            {
                enemyArmy.squadList[i].Reset();
                enemyArmy.squadList[i].transform.parent = enemyArmy.transform;
                enemyArmy.squadList[i].gameObject.SetActive(false);
            }




            if (playerArmy.squadList.Count == 0)
            {

                IsWin = false;


            }
            else if (enemyArmy.squadList.Count == 0)
            {



                IsWin = true;


            }
            else
            {
                IsDraft = true;
            }

        }

        if (sceneIndex == SceneLoader.Instance.activeScene)
        {

            mapBattleController.mapSceneManager.uIManager.BattleWinLoose(IsWin,kills,loses,xp,coins);

            mapBattleController.mapSceneManager.uIManager.winScreen.exitButton.onClick.RemoveAllListeners();
           mapBattleController.mapSceneManager.uIManager.winScreen.exitButton.onClick.AddListener(() => EndBattleButton());
        }
        else
        {
            if (IsWin)
            {
                SceneLoader.Instance.BattleWin(sceneIndex);
            }
            else
            {
                SceneLoader.Instance.BattleLose(sceneIndex);
            }
        }

        NotificationManager.Instance.RemoveOldNotification(sceneIndex);
    }

    public void EndBattleButton() {
        StartCoroutine(EndBattleSiquence());
    }


    private IEnumerator EndBattleSiquence()
    {

       

        //yield return new WaitForSeconds(afterBattleDelay);
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


    public void SpawnNewFormationPoints(ArmyController army)
    {
        ArmyController newEnemyArmy;

        if (army.isPlayer)
        {
            newEnemyArmy = enemyArmy;
        }
        else
        {
            newEnemyArmy = playerArmy;
        }

        Vector3 offcetToCenter = new Vector3(200f, 15f, 200f);

        Vector3 dirPlNorm = Quaternion.Euler(0, 90, 0) * (new Vector3(army.transform.position.x, mapBattleController.transform.position.y, army.transform.position.z) - mapBattleController.transform.position).normalized;
        Vector3 dirEnNorm = Quaternion.Euler(0, 90, 0) * (new Vector3(newEnemyArmy.transform.position.x, newEnemyArmy.transform.position.y, newEnemyArmy.transform.position.z) - mapBattleController.transform.position).normalized;

        Vector3 positionArmy = (transform.position + offcetToCenter) + dirPlNorm * 120f;
        //Vector3 positionEnemyArmy = (transform.position + offcetToCenter) + dirEnNorm * 120f;




        if (army.isPlayer)
        {
            GameObject newFormation = Instantiate(formationPointPrefab.gameObject, positionArmy, Quaternion.identity, transform);

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
            GameObject newEnFormation = Instantiate(formationPointPrefab.gameObject, positionArmy, Quaternion.identity, transform);

            newEnFormation.transform.rotation = Quaternion.LookRotation(dirEnNorm);

            enemySpawnPoints = new Transform[newEnFormation.transform.childCount];
            for (int i = 0; i < newEnFormation.transform.childCount; i++)
            {
                newEnFormation.transform.GetChild(i).GetComponent<SpawnPoint>().isPlayer = false;
                enemySpawnPoints[i] = newEnFormation.transform.GetChild(i);
            }
        }




    }

    public void SpawnFormationPoints()
    {

       // Vector3 offcetToCenter = transform.position;

        Vector3 dirPlNorm = Quaternion.Euler(0, 90, 0) * (new Vector3(playerArmy.transform.position.x, mapBattleController.transform.position.y, playerArmy.transform.position.z) - mapBattleController.transform.position).normalized;
        Vector3 dirEnNorm = Quaternion.Euler(0, 90, 0) * (new Vector3(enemyArmy.transform.position.x, mapBattleController.transform.position.y, enemyArmy.transform.position.z) - mapBattleController.transform.position).normalized;


        Debug.Log("dirPlNorm - " + dirPlNorm);

        Debug.Log("dirEnNorm - " + dirEnNorm);

        float offcet = 80f;

        if((playerArmy.inCity || enemyArmy.inCity) && !isSmallCity)
        {
            offcet = 200;
        }

        Vector3 positionPlayerArmy = transform.position + dirPlNorm * offcet;
        Vector3 positionEnemyArmy = transform.position  + dirEnNorm * offcet;


        if ((!playerArmy.inCity || isSmallCity))
        {
            GameObject newFormation = Instantiate(formationPointPrefab.gameObject, positionPlayerArmy, Quaternion.identity, transform);

            newFormation.transform.rotation = Quaternion.LookRotation(dirPlNorm);

            playerSpawnPoints = new Transform[newFormation.transform.childCount];

            for (int i = 0; i < newFormation.transform.childCount; i++)
            {
                newFormation.transform.GetChild(i).GetComponent<SpawnPoint>().isPlayer = true;
                playerSpawnPoints[i] = newFormation.transform.GetChild(i);

                playerSpawnPoints[i].transform.position = new Vector3(playerSpawnPoints[i].transform.position.x, terrain.SampleHeight(playerSpawnPoints[i].transform.position) + 5f, playerSpawnPoints[i].transform.position.z);

                   
            }

        }
        else if (playerArmy.inCity && !isSmallCity)

        {
            playerSpawnPoints = CitySpawnPoints;
        }

        if (!enemyArmy.inCity || isSmallCity)
        {

            GameObject newEnFormation = Instantiate(formationPointPrefab.gameObject, positionEnemyArmy, Quaternion.identity, transform);

            newEnFormation.transform.rotation = Quaternion.LookRotation(dirEnNorm);

            enemySpawnPoints = new Transform[newEnFormation.transform.childCount];
            for (int i = 0; i < newEnFormation.transform.childCount; i++)
            {
                newEnFormation.transform.GetChild(i).GetComponent<SpawnPoint>().isPlayer = false;
                enemySpawnPoints[i] = newEnFormation.transform.GetChild(i);

                enemySpawnPoints[i].transform.position = new Vector3(enemySpawnPoints[i].transform.position.x, terrain.SampleHeight(enemySpawnPoints[i].transform.position) + 5f, enemySpawnPoints[i].transform.position.z);
            }
        }
        else if (enemyArmy.inCity && !isSmallCity)
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

        Debug.Log("playerArmy.armyPower " + playerArmy.armyPower);
        Debug.Log("enemyArmy.armyPower " + enemyArmy.armyPower);



        UpdateUIPowerBar(playerArmy.armyPower, enemyArmy.armyPower);
        UpdateUIReinforcementsCount();

      
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

        if (IsEnd)
        {
            EndBattle();
        }


    }

    public void UpdateUIPowerBar(int player, int enemy)
    {
        float power = player +  enemy;
        power = player / power;

        Debug.Log("playerArmy.armyPower / (playerArmy.armyPower + enemyArmy.armyPower)" + power);


        mapBattleController.mapSceneManager.uIManager.UpdateBattlePowerBar(power);
    }

    public void UpdateUIReinforcementsCount()
    {

        int player = 0;
        int enemy = 0;

        for (int i = 0; i < reinforcementList.Count; i++)
        {
            if (reinforcementList[i].reinforcementArmy.isPlayer)
            {
                player++;
            }
            else
            {
                enemy++;
            }
        }

        mapBattleController.mapSceneManager.uIManager.UpdateBattleReinforcementsCount(player, enemy);
    }


    public void AttackAlert()
    {
        SceneLoader.Instance.MapAttackAlert(sceneIndex);
    }

    public void RemoveReinfrcementNotification(ArmyController army)
    {
        for (int i = 0; i < reinforcementList.Count; i++)
        {
            if (reinforcementList[i].reinforcementArmy == army)
            {

                Destroy(reinforcementList[i].gameObject);
                reinforcementList.RemoveAt(i);
                return;
            }
        }
    }

        public void ReinfrcementNotification(ArmyController army)
    {
        RemoveReinfrcementNotification(army);
             //SceneLoader.Instance.MapAttackAlert(sceneIndex);



             //Vector3 offcetToCenter = new Vector3(200f, 15f, 200f);
             //if(army == null)
             //{
             //    Debug.LogError("hahah");
             //}
             //else if (mapBattleController == null)
             //{
             //    Debug.LogError("hihihi");
             //}

             Vector3 dirNorm = Quaternion.Euler(0, 90, 0) * (new Vector3(army.transform.position.x, mapBattleController.transform.position.y, army.transform.position.z) - mapBattleController.transform.position).normalized;


        Vector3 spawnPos = GetSpawnOnBorder(dirNorm);
        //Vector3 positionEnemyArmy = (transform.position + offcetToCenter) + dirEnNorm * 120f;





        GameObject newFormation = Instantiate(reinforcementEffectPrefab.gameObject, spawnPos, Quaternion.identity, transform);

        newFormation.transform.rotation = Quaternion.LookRotation(dirNorm);
        newFormation.transform.localPosition = spawnPos;

        //playerSpawnPoints = new Transform[newFormation.transform.childCount];

        ReinforcementEffect newReinforcementEffect = newFormation.GetComponent<ReinforcementEffect>();

        for (int i = 0; i < newReinforcementEffect.points.Length; i++)
        {
            newReinforcementEffect.points[i].transform.position = new Vector3(newReinforcementEffect.points[i].transform.position.x, terrain.SampleHeight(newReinforcementEffect.points[i].transform.position) + 5f, newReinforcementEffect.points[i].transform.position.z);
        }

            newReinforcementEffect.CreateEffect(army, this);

        reinforcementList.Add(newReinforcementEffect);


        if (army.isPlayer)
        {
            NotificationManager.Instance.ShowNotification(sceneIndex,0,0,
                "Allied reinforcements incoming",
                "" + newReinforcementEffect.reinforcementArmyDistance.text,
                newReinforcementEffect.transform);


        }
        else
        {
            NotificationManager.Instance.ShowNotification(sceneIndex, 0, 1,
               "Enemy reinforcements incoming",
               "" + newReinforcementEffect.reinforcementArmyDistance.text,
               newReinforcementEffect.transform);
        }
    }

    Vector3 GetSpawnOnBorder(Vector3 direction)
    {
        Vector3 center = new Vector3(0,0,0);
       // direction.Normalize();

        float halfSize = 220f; // половина стороны (если квадрат 400x400)
        float mapMin = -220; // 0
        float mapMax = 220; // 400

        float tMin = float.MaxValue;

        // вычисляем пересечение по каждой оси
        if (Mathf.Abs(direction.x) > 0.0001f)
        {
            float tX1 = (mapMin - center.x) / direction.x;
            float tX2 = (mapMax - center.x) / direction.x;

            // берём положительное минимальное значение t (то есть вперёд по лучу)
            if (tX1 > 0) tMin = Mathf.Min(tMin, tX1);
            if (tX2 > 0) tMin = Mathf.Min(tMin, tX2);
        }

        if (Mathf.Abs(direction.z) > 0.0001f)
        {
            float tZ1 = (mapMin - center.z) / direction.z;
            float tZ2 = (mapMax - center.z) / direction.z;

            if (tZ1 > 0) tMin = Mathf.Min(tMin, tZ1);
            if (tZ2 > 0) tMin = Mathf.Min(tMin, tZ2);
        }

        // Получаем точку пересечения
        Vector3 spawnPos = center + direction * tMin;

        // ограничиваем координаты чтобы точно не выйти за пределы
        spawnPos.x = Mathf.Clamp(spawnPos.x, mapMin, mapMax);
        spawnPos.z = Mathf.Clamp(spawnPos.z, mapMin, mapMax);

        //spawnPos.y = Terrain.activeTerrain.SampleHeight(spawnPos);
        return spawnPos;
    }

        public void Debug_BattleWin()
    {

        for (int i = 0; i < enemySquadsParent.childCount; i++)
        {
            if (enemySquadsParent.GetChild(i).gameObject.activeSelf)
            {
                SquadController squad = enemySquadsParent.GetChild(i).GetComponent<SquadController>();



                DieEnemySquad(squad);
            }
        }

       

            //enemyArmy.squadList.Clear();

            //StartCoroutine(EndBattleSiquence());

            // EndBattle();




        }

    public void Debug_BattleLose()
    {

        for (int i = 0; i < playerSquadParent.childCount; i++)
        {
            if (playerSquadParent.GetChild(i).gameObject.activeSelf)
            {
                SquadController squad = playerSquadParent.GetChild(i).GetComponent<SquadController>();



                DiePlayerSquad(squad);
            }
        }


    }


}
