using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
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

    public int newBattleSceneIndex;

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
            if (activeBattleScenes[i] != null)
            {
                mapSceneManager.uIManager.battleButtons[i].gameObject.SetActive(true);
            }

         }

        if (activeScene >= newBattleSceneIndex || mapBattleControllers.Count <= newBattleSceneIndex)
        {
            mapSceneManager.uIManager.NewBattleButtonActivate(0, false);
            newBattleSceneIndex = -2;
        }
       
    }

    public void NewBattleButton(int sceneIndex)
    {
        if(activeScene == -1)
        {
            mapSceneManager.controlController.SelectBattle(mapBattleControllers[sceneIndex]);
            //mapSceneManager.cameraMapMovement.CamToPoint.transform.position,null);
        }
        else if(activeScene >= 0 )
        {
            StartCoroutine(EnterMapAndSelectBattle(activeScene, sceneIndex));
            activeScene = -1;
          
            //mapSceneManager.controlController.SelectBattle(mapBattleControllers[sceneIndex]);
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

        mapSceneManager.uIManager.UpdateActiveSceneSpritePosition(activeScene);

    }

    private IEnumerator EnterMapAndSelectBattle(int sceneIndex,int newBattleIndex)
    {
        mapSceneManager.uIManager.TransitionAnimation(true);
        activeBattleScenes[sceneIndex].cameraController.CameraMoveToExit();


        yield return new WaitForSeconds(mapSceneDelay);
        // mapSceneManager.EnterMapSceneAnimation();

        activeBattleScenes[sceneIndex].ExitBattleScene();
        mapSceneManager.EnterMapScene();
        mapSceneManager.uIManager.TransitionAnimation(false);

        yield return new WaitForSeconds(mapSceneDelay);

        mapSceneManager.controlController.SelectBattle(mapBattleControllers[newBattleIndex]);
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

            if (Instance.activeBattleScenes.Count >= sceneIndex + 1)
            {
                if (Instance.activeBattleScenes[sceneIndex].isInit)
                {

                    Instance.activeBattleScenes[sceneIndex].EnterBattleScene();
                    mapSceneManager.uIManager.TransitionAnimation(false);
                    mapSceneManager.ExitMapScene();

                    activeScene = sceneIndex;

                    UpdateBattleButtons();
                    mapSceneManager.uIManager.UpdateActiveSceneSpritePosition(activeScene);

                    i = 5;

                }
            }
        }


        

    }

    private IEnumerator EnterBattleFromBattle(int exitSceneIndex, int sceneIndex)
    {

        // mapSceneManager.ExitMapSceneAnimation();
        mapSceneManager.uIManager.TransitionAnimation(true);//UI

        Instance.activeBattleScenes[exitSceneIndex].ExitBattleSiquence();

        yield return new WaitForSeconds(battleSceneDelay);

        Instance.activeBattleScenes[sceneIndex].EnterBattleScene();
        //mapSceneManager.ExitMapScene();
        mapSceneManager.uIManager.TransitionAnimation(false);//UI
    }

    private IEnumerator EnterBattleFromMap( int sceneIndex)
    {

         mapSceneManager.ExitMapSceneAnimation();

        yield return new WaitForSeconds(battleSceneDelay);

        Instance.activeBattleScenes[sceneIndex].EnterBattleScene();
        mapSceneManager.uIManager.TransitionAnimation(false);
        mapSceneManager.ExitMapScene();

        activeScene = sceneIndex;
        mapSceneManager.uIManager.UpdateActiveSceneSpritePosition(activeScene);

    }

    public void LoadBattleScene(int index, ArmyController army, ArmyController enemyArmy)
    {
        StartCoroutine(LoadBattleSceneAsync(index, army, enemyArmy));
    }

    public void LoadNewSqads(int index,SquadController[] squads, ArmyController army)
    {
        StartCoroutine(LoadNewSqadsSequence(index, squads,  army));
    }

    private IEnumerator LoadNewSqadsSequence(int index, SquadController[] squads, ArmyController army)
    {
        //Debug.LogError("LoadNewSqadsSequence " + index);

        BattleSceneManager scene = activeBattleScenes[index];

        scene.SpawnNewFormationPoints(army);

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
                

                squadGO.transform.parent = scene.enemySquadsParent;
                scene.enemySquads.Add(squadGO);
            }

            squadGO.transform.position = spawn.position;
            squadGO.gameObject.SetActive(true);


            SquadController squad = squadGO.GetComponent<SquadController>();

            squad.battleSceneManager = scene;

            yield return new WaitForSeconds(0.5f);

            squad.InitSquad();
        }
       
    }

        private IEnumerator LoadBattleSceneAsync(int index, ArmyController army, ArmyController enemyArmy)
    {
        if (Instance.activeBattleScenes[index] == null)
        {
            BattleSceneManager newbattle;

            if (army.inCity )
            {
                
                if (army.city.isSmallCity)
                {

                    newbattle = Instantiate(battleScenes[Random.RandomRange(0, battleScenes.Length)], new Vector3(index * 800, 0, 0), battleScenes[0].transform.rotation);

                }
                else
                {
                   
                    newbattle = Instantiate(citiesScenes[Random.RandomRange(0, citiesScenes.Length)], new Vector3(index * 800, 0, 0), battleScenes[0].transform.rotation);
                }
            }
            else if(enemyArmy.inCity)
            {
                if (enemyArmy.city.isSmallCity)
                {

                    newbattle = Instantiate(battleScenes[Random.RandomRange(0, battleScenes.Length)], new Vector3(index * 800, 0, 0), battleScenes[0].transform.rotation);

                }
                else
                {

                    newbattle = Instantiate(citiesScenes[Random.RandomRange(0, citiesScenes.Length)], new Vector3(index * 800, 0, 0), battleScenes[0].transform.rotation);
                }



            }
            else
            {
                newbattle = Instantiate(battleScenes[Random.RandomRange(0, battleScenes.Length)], new Vector3(index * 800, 0, 0), battleScenes[0].transform.rotation);
            }


            Instance.activeBattleScenes[index] = newbattle;


            newbattle.sceneIndex = index;

            newbattle.playerArmy = army;
            newbattle.enemyArmy = enemyArmy;
            newbattle.mapBattleController = mapBattleControllers[index];
            newbattle.resourceManager = resourceManager;

            
            if (army.inCity || enemyArmy.inCity)
            {
                    CityController c;
               

                if (army.inCity)
                {
                        c = army.city;


                }
                else
                {
                        c = enemyArmy.city;
                }

                    newbattle.city = c;

                newbattle.IsCity = true;
                if (c.isSmallCity) {
                    
                
                
                    newbattle.isSmallCity = true;
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
            PlaceArmyOnBattlefield(index,army, spawnPoints);

            PlaceArmyOnBattlefield(index,enemyArmy, spawnEnemyPoints);


            yield return new WaitForSeconds(0.3f);


            newbattle.InitScene();

           


          

        }


    }




    
    

    private void PlaceArmyOnBattlefield(int index,ArmyController army, Transform[] spawnPoints)
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
                squadGO.transform.parent = activeBattleScenes[index].playerSquadParent;
                activeBattleScenes[index].playerSquads.Add(squadGO);
            }
            else
            {
                squadGO.transform.parent = activeBattleScenes[index].enemySquadsParent;
                activeBattleScenes[index].enemySquads.Add(squadGO);
            }

            squadGO.transform.position = spawn.position;
            squadGO.gameObject.SetActive(true);

            squadGO.GetComponent<SquadController>().battleSceneManager = activeBattleScenes[index];


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

        mapSceneManager.EnterMapScene();
        mapSceneManager.uIManager.TransitionAnimation(false);

        activeScene = -1;

        UpdateBattleButtons();
        mapSceneManager.uIManager.UpdateActiveSceneSpritePosition(activeScene);


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

        activeScene = -1;

        UpdateBattleButtons();
        mapSceneManager.uIManager.UpdateActiveSceneSpritePosition(activeScene);

        mapSceneManager.EnterMapScene();
        mapSceneManager.uIManager.TransitionAnimation(false);



    }

    public void ArmyRetreat(int sceneIndex)
    {

        int scene = FindSceneByIndex(sceneIndex);


        if (scene != -1)
        {
            if (activeBattleScenes[scene] != null)
            {
                BattleSceneManager deleteScene = activeBattleScenes[scene];

                deleteScene.PlayerRetreat();

               
                activeBattleScenes.RemoveAt(scene);

                Destroy(deleteScene.gameObject);
            }
            else
            {
                activeBattleScenes.RemoveAt(scene);
            }

            mapBattleControllers.RemoveAt(scene);
        }


       // 

       
       

        IndexesUpdate();

        UpdateBattleButtons();



    }

    public int FindSceneByIndex(int sceneIndex)
    {
        for (int i = 0; i < mapBattleControllers.Count; i++)
        {
            if(mapBattleControllers[i].sceneIndex == sceneIndex)
            {

                return i;
            }
        }

        return -1;
    }

    public void IndexesUpdate()
    {
        for (int i = 0; i < mapBattleControllers.Count; i++)
        {
            mapBattleControllers[i].sceneIndex = i;
        }

        for (int i = 0; i < activeBattleScenes.Count; i++)
        {
            activeBattleScenes[i].sceneIndex = i;
        }

        
    }

    public void StartAutoBattle(int sceneIndex)
    {
        int scene = FindSceneByIndex(sceneIndex);



        if (scene != -1)
        {
            mapBattleControllers.RemoveAt(scene);





            activeBattleScenes.RemoveAt(scene);
        }

        IndexesUpdate();
        UpdateBattleButtons();
    }


        public void EnterBattle( int sceneIndex, ArmyController army, ArmyController enemyArmy)
    {
        Debug.Log("button - EnterBattle");

        Debug.Log("sceneIndex - " + sceneIndex);
        Debug.Log("battleScenes - " + activeBattleScenes.Count);

         //int scene = FindSceneByIndex(sceneIndex);
       
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

        

        if (army.inCity)
        {
            newBattleController.city = army.city;
            newBattleController.cityBattle = true;
        }
        if (enemyArmy.inCity)
        {
            newBattleController.city = enemyArmy.city;
            newBattleController.cityBattle = true;
        }

        newBattleController.ArmyCheck();

        newBattleController.resourceManager = resourceManager;

        //newBattleController.sceneIndex = Instance.activeBattleScenes.Count-1;
        Instance.activeBattleScenes.Add(null);

        newBattleController.sceneIndex = Instance.activeBattleScenes.Count - 1;


        newBattleSceneIndex = newBattleController.sceneIndex;
        mapSceneManager.uIManager.newBattleButton.onClick.RemoveAllListeners();

        mapSceneManager.uIManager.newBattleButton.onClick.AddListener(() => NewBattleButton(newBattleSceneIndex));
        

        mapSceneManager.uIManager.NewBattleButtonActivate(newBattleSceneIndex, true);


        if (army.isSelected)
        {
            newBattleController.SelectThisBattle();
        }
    }



    public void MapAttackAlert(int index)
    {
        if (activeScene != index)
        {
            mapSceneManager.uIManager.alertObject[index].SetActive(true);
        }
    }

}
