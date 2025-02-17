using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    public BattleSceneManager[] battleScenes;
    public List<MapBattleController> mapBattleControllers;

    public List<BattleSceneManager> activeBattleScenes;

    [SerializeField] private GameObject battleInfo;

    [SerializeField] private MapSceneManager mapSceneManager;

  

    public float battleSceneDelay;
    public float mapSceneDelay;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private IEnumerator EnterMapAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        mapSceneManager.EnterMapScene();
    }

    private IEnumerator EnterBattleAfterDelay(float delay,int sceneIndex)
    {
        yield return new WaitForSeconds(delay);

        Instance.activeBattleScenes[sceneIndex].EnterBattleScene();
        mapSceneManager.ExitMapScene();
    }

        public void LoadBattleScene(int index, ArmyController army, ArmyController enemyArmy)
    {
        StartCoroutine(LoadBattleSceneAsync(index, army, enemyArmy));
    }

    private IEnumerator LoadBattleSceneAsync(int index, ArmyController army, ArmyController enemyArmy)
    {

        BattleSceneManager newbattle = Instantiate(battleScenes[0], new Vector3(activeBattleScenes.Count * 500, 0, 0), battleScenes[0].transform.rotation);

        Instance.activeBattleScenes[index] = newbattle;
        newbattle.sceneIndex = index;

        newbattle.playerArmy = army;
        newbattle.enemyArmy = enemyArmy;
        newbattle.mapBattleController = mapBattleControllers[index];

        yield return new WaitForSeconds(0.5f);

        newbattle.SpawnFormationPoints();



        // Дожидаемся появления сцены
        yield return new WaitForSeconds(0.5f);

        // Найти точки спавна
        Transform[] spawnPoints = newbattle.playerSpawnPoints;
        Transform[] spawnEnemyPoints = newbattle.enemySpawnPoints;

        // Разместить армию на найденных точках
        PlaceArmyOnBattlefield(army, spawnPoints);

        PlaceArmyOnBattlefield(enemyArmy, spawnEnemyPoints);


        yield return new WaitForSeconds(0.5f);


        newbattle.InitScene();







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

    public void ExitBattle(int sceneIndex)
    {
       
          StartCoroutine(EnterMapAfterDelay(mapSceneDelay));
    }

    public void BattleWin(int sceneIndex)
    {

        BattleSceneManager deleteScene = activeBattleScenes[sceneIndex];

        mapBattleControllers[sceneIndex].BattleWin();


        activeBattleScenes.Remove(deleteScene);
        mapBattleControllers.RemoveAt(sceneIndex);

        deleteScene.playerArmy.ResetAfterBattle();

        Destroy(deleteScene.enemyArmy.gameObject);
        Destroy(deleteScene.gameObject);
    }
    public void BattleLose(int sceneIndex)
    {

        BattleSceneManager deleteScene = activeBattleScenes[sceneIndex];

        mapBattleControllers[sceneIndex].BattleLose();

        activeBattleScenes.Remove(deleteScene);
        mapBattleControllers.RemoveAt(sceneIndex);

        deleteScene.enemyArmy.ResetAfterBattle();

        Destroy(deleteScene.playerArmy.gameObject);
        Destroy(deleteScene.gameObject);
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

                mapSceneManager.ExitMapSceneAnimation();
                StartCoroutine(EnterBattleAfterDelay(battleSceneDelay, sceneIndex));



            }
            else
            {
                Instance.LoadBattleScene(sceneIndex, army, enemyArmy);

                if (Instance.activeBattleScenes[sceneIndex] != null)
                {
                    mapSceneManager.ExitMapSceneAnimation();
                    StartCoroutine(EnterBattleAfterDelay(battleSceneDelay, sceneIndex));
                }
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
        BattleInfo newBattleInfo = objectInfo.GetComponent<BattleInfo>();

        MapBattleController battleController = objectInfo.GetComponent<MapBattleController>();

        mapBattleControllers.Add(battleController);




        newBattleInfo.playerArmy = army;
        newBattleInfo.enemyArmy = enemyArmy;

        battleController.playerArmy = army;
        battleController.enemyArmy = enemyArmy;

        //newBattleInfo.sceneIndex = Instance.activeBattleScenes.Count-1;
        Instance.activeBattleScenes.Add(null);
        newBattleInfo.sceneIndex = Instance.activeBattleScenes.Count - 1;
        

        newBattleInfo.toBattleButton.onClick.AddListener(() => Instance.EnterBattle( newBattleInfo.sceneIndex, battleController.playerArmy, battleController.enemyArmy)) ;
        newBattleInfo.toBattleButton.onClick.AddListener(() => battleController.StartBattle()) ;

        newBattleInfo.UpdateArmyInfo();

        if (army.isSelected)
        {
            newBattleInfo.SelectBattle();
        }
    }
}
