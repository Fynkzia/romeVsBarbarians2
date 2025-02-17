using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class BattleSceneManager : MonoBehaviour
{
    public int sceneIndex;
    public ArmyController playerArmy;
    public ArmyController enemyArmy;

    public MapBattleController mapBattleController;

    public Button toGlobalMapButton;

    public SquadControlManager controlController;
    public OfficerSystem officerSystem;
    public AIController aIController;
    public WinLoseManager winLoseManager;

     public Camera gameCamera;
    public CameraMovement cameraController;
    public GameObject cinemachineGameObject;
    public GameObject lightGameObject;

    public GameObject UIGameObject;
    public Animator transitionAnimation;
    public Animator WinEffect;
    public Animator LoseEffect;

    public Transform[] playerSpawnPoints;
    public Transform[] enemySpawnPoints;

    public GameObject spawnPointPrefab;
    public GameObject formationPointPrefab;

    public List<GameObject> playerSquads;
    public List<GameObject> enemySquads;

    public Transform playerSquadParent;
    public Transform enemySquadsParent;

    public bool isInit;

    public bool IsWin;

    public float delayFade;
    public float afterBattleDelay;


    public void DiePlayerSquad(SquadController squadController)
    {
        playerArmy.squadList.Remove(squadController);

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

        


    }

    private IEnumerator EndBattleSiquence() {

        playerArmy.squadList.Clear();
        Debug.Log("EndBattleSiquence " + playerSquadParent.childCount);  
        for (int i = 0; i < playerSquadParent.childCount; i++)
        {
            if (playerSquadParent.GetChild(i).gameObject.activeSelf) {
                SquadController squad = playerSquadParent.GetChild(i).GetComponent<SquadController>();

               
               

                 playerArmy.squadList.Add(squad);

                

                
            }
        }

        for (int i = 0; i < playerArmy.squadList.Count; i++)
        {
            playerArmy.squadList[i].transform.parent = playerArmy.transform;
            playerArmy.squadList[i].gameObject.SetActive(false);
        }

        enemyArmy.squadList.Clear();

        for (int i = 0; i < enemySquadsParent.childCount; i++)
        {
            if (enemySquadsParent.GetChild(i).gameObject.activeSelf)
            {
                SquadController squad = enemySquadsParent.GetChild(i).GetComponent<SquadController>();

               
               

                enemyArmy.squadList.Add(squad);
            }
        }

        for (int i = 0; i < enemyArmy.squadList.Count; i++)
        {
            enemyArmy.squadList[i].transform.parent = enemyArmy.transform;
            enemyArmy.squadList[i].gameObject.SetActive(false);
        }


        if (playerArmy.squadList.Count == 0) {

            IsWin = false;
            WinEffect.gameObject.SetActive(true);

        } else if (enemyArmy.squadList.Count == 0) {
            LoseEffect.gameObject.SetActive(true);
            IsWin = true;

           
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

        Vector3 offcetToCenter = new Vector3(200f, 12f, 200f);

        Vector3 dirPlNorm = Quaternion.Euler(0, 90, 0) * (playerArmy.transform.position - mapBattleController.transform.position).normalized;
        Vector3 dirEnNorm = Quaternion.Euler(0, 90, 0) * (enemyArmy.transform.position - mapBattleController.transform.position).normalized  ;


        Debug.Log("dirPlNorm - " + dirPlNorm);

        Debug.Log("dirEnNorm - " + dirEnNorm);

        Vector3 positionPlayerArmy = (transform.position + offcetToCenter) + dirPlNorm * 120f;
        Vector3 positionEnemyArmy = (transform.position + offcetToCenter) + dirEnNorm * 120f;

        GameObject newFormation = Instantiate(formationPointPrefab.gameObject, positionPlayerArmy, Quaternion.identity, transform);

        newFormation.transform.rotation = Quaternion.LookRotation(dirPlNorm);

        playerSpawnPoints = new Transform[newFormation.transform.childCount];

        for (int i = 0; i < newFormation.transform.childCount; i++)
        {
            newFormation.transform.GetChild(i).GetComponent<SpawnPoint>().isPlayer = true;
            playerSpawnPoints[i] = newFormation.transform.GetChild(i);
        }

        GameObject newEnFormation = Instantiate(formationPointPrefab.gameObject, positionEnemyArmy, Quaternion.identity, transform);

        newEnFormation.transform.rotation = Quaternion.LookRotation(dirEnNorm);

        enemySpawnPoints = new Transform[newEnFormation.transform.childCount];
        for (int i = 0; i < newEnFormation.transform.childCount; i++)
        {
            newEnFormation.transform.GetChild(i).GetComponent<SpawnPoint>().isPlayer = false;
            enemySpawnPoints[i] = newEnFormation.transform.GetChild(i);
        }

    }

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

        toGlobalMapButton.onClick.AddListener(() => ExitBattleSiquence());
        isInit = true;

    }


    public void EnterBattleScene()
    {
        gameCamera.gameObject.SetActive(true);
        lightGameObject.gameObject.SetActive(true);
        cinemachineGameObject.gameObject.SetActive(true);
        cameraController.gameObject.SetActive(true);
        controlController.gameObject.SetActive(true);
        UIGameObject.gameObject.SetActive(true);

       
        EnterBattleSceneAnimation();
    }


    public void ExitBattleScene()
    {
        gameCamera.gameObject.SetActive(false);
        lightGameObject.gameObject.SetActive(false);

        cinemachineGameObject.gameObject.SetActive(false);
        cameraController.gameObject.SetActive(false);
        controlController.gameObject.SetActive(false);
        UIGameObject.gameObject.SetActive(false);

        SceneLoader.Instance.ExitBattle(sceneIndex);

        
    }

    public void ExitBattleSiquence()
    {
        transitionAnimation.SetTrigger("In");
        cameraController.CameraMoveToExit();
        StartCoroutine(ExitSiquence());
       
    }

    public void EnterBattleSceneAnimation()
    {
        transitionAnimation.SetTrigger("Out");
        cameraController.CameraMoveEnter();
    }

    private IEnumerator ExitSiquence()
    {
       

        
        yield return new WaitForSeconds(delayFade);

        ExitBattleScene();
    }
}
