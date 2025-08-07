using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public BattleSceneManager[] battleScenes;

    public BattleSceneManager[] citiesScenes;

    

    public List<MapBattleController> mapBattleControllers;

    public List<BattleSceneManager> activeBattleScenes;

    public int activeScene = -1; // -1 - map 0-2 - battleScene


    [SerializeField] private GameObject battleInfo;

    [SerializeField] private MapSceneManager mapSceneManager;
    [SerializeField] private ResourceManager resourceManager;


    public float fadeDelay;
    public float battleSceneDelay;
    public float mapSceneDelay;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        UpdateBattleButtons();

    }

    //ui
    public void UpdateBattleButtons()
    {
        for (int i = 0; i < mapSceneManager.uIManager.battleButtons.Count; i++)
        {
            mapSceneManager.uIManager.battleButtons[i].gameObject.SetActive(false);

        }

        for (int i = 0; i < activeBattleScenes.Count; i++)
        {
            mapSceneManager.uIManager.battleButtons[i].gameObject.SetActive(true);

         }
    }

    public void LoadBattleSceneButton(int sceneIndex)
    {
       if(sceneIndex > activeBattleScenes.Count) { return; }

        if(sceneIndex == -1)
        {
            if(activeScene >= 0) // игрок выходит с боя на карту
            {
                

                StartCoroutine(EnterMapAfterDelay(activeScene));

                activeScene = sceneIndex;
            }
        }
        else
        {
            

            if(sceneIndex != activeScene)
            {

                if (activeScene == -1)// игрок переходит в бой с карты
                {
                    StartCoroutine(EnterBattleFromMap( sceneIndex));
                    activeScene = sceneIndex;
                }
                else// игрок переключается между боями 
                {
                  

                    StartCoroutine(EnterBattleFromBattle(activeScene, sceneIndex));

                    activeScene = sceneIndex;
                }

               

            }

        }


       
    }


    private IEnumerator EnterMapAfterDelay(int sceneIndex)
    {
        mapSceneManager.uIManager.TransitionAnimation(true);
        activeBattleScenes[sceneIndex].cameraController.CameraMoveToExit();
        

        yield return new WaitForSeconds(mapSceneDelay);
        // mapSceneManager.EnterMapSceneAnimation();

        activeBattleScenes[sceneIndex].ExitBattleScene();
        mapSceneManager.EnterMapScene();
        mapSceneManager.uIManager.TransitionAnimation(false);
    }

    private IEnumerator LoadBattleSceneAndEnter(int sceneIndex, ArmyController army, ArmyController enemyArmy)
    {
        mapSceneManager.ExitMapSceneAnimation();

        yield return new WaitForSeconds(0.5f);
        Instance.LoadBattleScene(sceneIndex, army, enemyArmy);

       

        for (int i = 0; i < 5; i++)
        {
            yield return new WaitForSeconds(0.5f);

            if (Instance.activeBattleScenes[sceneIndex].isInit)
            {

                Instance.activeBattleScenes[sceneIndex].EnterBattleScene();
                mapSceneManager.uIManager.TransitionAnimation(false);
               mapSceneManager.ExitMapScene();

                activeScene = sceneIndex;
                i = 5;

            }
        }
            

       
    }

    private IEnumerator EnterBattleFromBattle(int exitSceneIndex, int sceneIndex)
    {

        // mapSceneManager.ExitMapSceneAnimation();
        mapSceneManager.uIManager.TransitionAnimation(false);//UI

        Instance.activeBattleScenes[exitSceneIndex].ExitBattleScene();

        yield return new WaitForSeconds(battleSceneDelay);

        Instance.activeBattleScenes[sceneIndex].EnterBattleScene();
        //mapSceneManager.ExitMapScene();
    }

    private IEnumerator EnterBattleFromMap( int sceneIndex)
    {

         mapSceneManager.ExitMapSceneAnimation();

        yield return new WaitForSeconds(battleSceneDelay);

        Instance.activeBattleScenes[sceneIndex].EnterBattleScene();
        mapSceneManager.uIManager.TransitionAnimation(false);
        mapSceneManager.ExitMapScene();

        activeScene = sceneIndex;
    }

    public void LoadBattleScene(int index, ArmyController army, ArmyController enemyArmy)
    {
        StartCoroutine(LoadBattleSceneAsync(index, army, enemyArmy));
    }

    public void LoadNewSqads(int index,SquadController[] squads)
    {
        StartCoroutine(LoadNewSqadsSequence(index, squads));
    }

    private IEnumerator LoadNewSqadsSequence(int index, SquadController[] squads)
    {

         BattleSceneManager scene = activeBattleScenes[index];

        for (int i = 0; i < squads.Length; i++)
        {

            GameObject squadGO = squads[i].gameObject;
            Transform spawn;

            if (squads[0].playerSquad)
            {
                spawn = scene.playerSpawnPoints[scene.playerSquads.Count-1]; // Циклически используем точки
               

                squadGO.transform.parent = scene.playerSquadParent;
                scene.playerSquads.Add(squadGO);
            }
            else
            {
                spawn = scene.enemySpawnPoints[scene.enemySquads.Count]; // Циклически используем точки
                

                squadGO.transform.parent = scene.playerSquadParent;
                scene.enemySquads.Add(squadGO);
            }

            squadGO.transform.position = spawn.position;
            squadGO.gameObject.SetActive(true);


            SquadController squad = squadGO.GetComponent<SquadController>();

            squad.battleSceneManager = activeBattleScenes[activeBattleScenes.Count - 1];

            yield return new WaitForSeconds(0.5f);

            squad.InitSquad();
        }
       
    }

        private IEnumerator LoadBattleSceneAsync(int index, ArmyController army, ArmyController enemyArmy)
    {
        if (Instance.activeBattleScenes[index] == null)
        {
            BattleSceneManager newbattle;

            if (army.inCity || enemyArmy.inCity)
            {

                newbattle = Instantiate(citiesScenes[0], new Vector3(activeBattleScenes.Count * 500, 0, 0), battleScenes[0].transform.rotation);
            }
            else
            {
                newbattle = Instantiate(battleScenes[0], new Vector3(activeBattleScenes.Count * 500, 0, 0), battleScenes[0].transform.rotation);
            }


            Instance.activeBattleScenes[index] = newbattle;


            newbattle.sceneIndex = index;

            newbattle.playerArmy = army;
            newbattle.enemyArmy = enemyArmy;
            newbattle.mapBattleController = mapBattleControllers[index];
            newbattle.resourceManager = resourceManager;

            if (army.inCity || enemyArmy.inCity)
            {
                newbattle.IsCity = true;

                if (army.inCity)
                {
                    newbattle.city = army.transform.parent.GetComponent<CityController>();
                }
                else
                {
                    newbattle.city = enemyArmy.transform.parent.GetComponent<CityController>();
                }
            }


            yield return new WaitForSeconds(0.3f);

            newbattle.SpawnFormationPoints();




            // Дожидаемся появления сцены
            yield return new WaitForSeconds(0.3f);

            // Найти точки спавна
            Transform[] spawnPoints = newbattle.playerSpawnPoints;
            Transform[] spawnEnemyPoints = newbattle.enemySpawnPoints;



            // Разместить армию на найденных точках
            PlaceArmyOnBattlefield(army, spawnPoints);

            PlaceArmyOnBattlefield(enemyArmy, spawnEnemyPoints);


            yield return new WaitForSeconds(0.3f);


            newbattle.InitScene();

           


            UpdateBattleButtons();

        }


    }




    
    

    private void PlaceArmyOnBattlefield(ArmyController army, Transform[] spawnPoints)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Нет точек спавна на этой сцене!");
            return;
        }

        for (int i = 0; i < army.squadList.Count; i++)
        {
            Transform spawn = spawnPoints[i % spawnPoints.Length]; // Циклически используем точки
            GameObject squadGO = army.squadList[i].gameObject;

            if (army.isPlayer)
            {
                squadGO.transform.parent = activeBattleScenes[activeBattleScenes.Count - 1].playerSquadParent;
                activeBattleScenes[activeBattleScenes.Count - 1].playerSquads.Add(squadGO);
            }
            else
            {
                squadGO.transform.parent = activeBattleScenes[activeBattleScenes.Count - 1].enemySquadsParent;
                activeBattleScenes[activeBattleScenes.Count - 1].enemySquads.Add(squadGO);
            }

            squadGO.transform.position = spawn.position;
            squadGO.gameObject.SetActive(true);

            squadGO.GetComponent<SquadController>().battleSceneManager = activeBattleScenes[activeBattleScenes.Count - 1];


        }
    }

    private void PlaceSqaudOnBattlefield(SquadController squad, Transform[] spawnPoints)
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogError("Нет точек спавна на этой сцене!");
            return;
        }

        
            


        
    }


    

    public void BattleWin(int sceneIndex)
    {

        BattleSceneManager deleteScene = activeBattleScenes[sceneIndex];

        mapBattleControllers[sceneIndex].BattleWin();


        activeBattleScenes.Remove(deleteScene);
        mapBattleControllers.RemoveAt(sceneIndex);

       

        
        Destroy(deleteScene.gameObject);
    }
    public void BattleLose(int sceneIndex)
    {
        if(activeBattleScenes[sceneIndex] != null)
        {
            BattleSceneManager deleteScene = activeBattleScenes[sceneIndex];
            Destroy(deleteScene.gameObject);
            activeBattleScenes.RemoveAt(sceneIndex);
        }
        else
        {
            activeBattleScenes.RemoveAt(sceneIndex);
        }
       

        mapBattleControllers[sceneIndex].BattleLose();

       
        mapBattleControllers.RemoveAt(sceneIndex);

       

        
       
    }

    public void ArmyRetreat(int sceneIndex)
    {

        if (activeBattleScenes[sceneIndex] != null)
        {
            BattleSceneManager deleteScene = activeBattleScenes[sceneIndex];
            Destroy(deleteScene.gameObject);
            activeBattleScenes.RemoveAt(sceneIndex);
        }
        else
        {
            activeBattleScenes.RemoveAt(sceneIndex);
        }


        mapBattleControllers[sceneIndex].PlayerRetreat();

       
        mapBattleControllers.RemoveAt(sceneIndex);



        
        
    }


    public void EnterBattle( int sceneIndex, ArmyController army, ArmyController enemyArmy)
    {
        Debug.Log("button - EnterBattle");

        Debug.Log("sceneIndex - " + sceneIndex);
        Debug.Log("battleScenes - " + activeBattleScenes.Count);


       
        if(sceneIndex < Instance.activeBattleScenes.Count)
        {
            if (Instance.activeBattleScenes[sceneIndex] != null)
            {

               
                StartCoroutine(EnterBattleFromMap(sceneIndex));



            }
            else
            {
                

                StartCoroutine(LoadBattleSceneAndEnter(sceneIndex, army, enemyArmy));

                
            }
        }
        else
        {
           

           
        }

           


       




        
    }

    public void CreateBattleInfo(ArmyController army, ArmyController enemyArmy)
    {
        Vector3 positionPlayerArmy = (army.transform.position + enemyArmy.transform.position)/2;

        GameObject objectInfo = Instantiate(battleInfo, positionPlayerArmy, battleInfo.transform.rotation);
        MapBattleController newBattleController = objectInfo.GetComponent<MapBattleController>();

     

        mapBattleControllers.Add(newBattleController);




        newBattleController.playerArmy = army;
        newBattleController.enemyArmy = enemyArmy;


        newBattleController.resourceManager = resourceManager;

        //newBattleController.sceneIndex = Instance.activeBattleScenes.Count-1;
        Instance.activeBattleScenes.Add(null);

        newBattleController.sceneIndex = Instance.activeBattleScenes.Count - 1;
        

       

       

      

        if (army.isSelected)
        {
            newBattleController.SelectThisBattle();
        }
    }
}
