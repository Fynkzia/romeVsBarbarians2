using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class SquadSpawnerController : MonoBehaviour
{
    [SerializeField] public bool isPlayer;
    [SerializeField] private bool isSpawner;
    [SerializeField] private bool isHealer;
    [SerializeField] private bool isInit;


   

    [SerializeField] public SquadController squadSpawnPrefab;

    [SerializeField] private List<SquadController> spawnedSuadsList;

    [SerializeField] private float timeToSpawn;
    [SerializeField] private float timeToHeal;
    [SerializeField] private int squadLimit;


    [SerializeField] public SquadController playerDefaultSquadHelper;
    [SerializeField] public SquadController enemyDefaultSquadHelper;



    private float curretntTimeToSpawn;
    private float curretntTimeToHeal;
    private float curretntTimeToUpdateUi;

    [SerializeField] public List<SquadController> healedSquad;

    [SerializeField] private GameObject allVisual;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject spawnerInfoObject;
    [SerializeField] private Image spawnTimerImage;
    [SerializeField] private TextMeshProUGUI spawnTimerText;

    [SerializeField] private Transform squadInfoPanel;
    [SerializeField] private SquadMapUIPanel newSquadPanel;
    [SerializeField] private float timeToUpdateUi = 1;

    [SerializeField] private GameObject healerInfoObject;
    [SerializeField] private GameObject cencelInfoObject;


    public BattleSceneManager battleSceneManager;

    // Start is called before the first frame update
    public void Init()
    {
        battleSceneManager = transform.parent.parent.GetComponent<BattleSceneManager>();
        isInit = true;

        SetSpawner(true);

      

        allVisual.SetActive(true);


       
    }


    // Update is called once per frame
    void Update()
    {
        if (isInit) {

            if (isSpawner)
            {
                curretntTimeToSpawn += Time.deltaTime;
                curretntTimeToUpdateUi += Time.deltaTime;

                if (curretntTimeToUpdateUi > timeToUpdateUi)
                {
                    UpdateSpawnUi();
                }

                    if (curretntTimeToSpawn > timeToSpawn)
                {
                    if(spawnedSuadsList.Count < squadLimit)
                    {
                        SpawnSquad();
                    }
                    else
                    {
                       
                        SetSpawner(false);
                    }



                    curretntTimeToSpawn = 0;
                }
            }
            else if(!isHealer)
            {
                curretntTimeToSpawn += Time.deltaTime;

                if (curretntTimeToSpawn > timeToSpawn)
                {
                    SpawnedListUpdate();

                    if (spawnedSuadsList.Count < squadLimit)
                    {
                        SetSpawner(true);
                    }

                        curretntTimeToSpawn = 0;
                }
             }

            if (isHealer)
            {
                if (healedSquad.Count == 0)
                {
                    SetHealer(false);
                }

                curretntTimeToHeal += Time.deltaTime;

                if (curretntTimeToHeal > timeToHeal)
                {


                    SquadHeal();
                    curretntTimeToHeal = 0;
                }
            }


        }


    }
    public void SquadHeal()
    {
        if (healedSquad.Count == 0)
        {
            return;
        }
        else
        {
            healedSquad[0].AddUnit(1);

            if(healedSquad[0].currentAmountUnits == healedSquad[0].amountUnits)
            {
                healedSquad.Remove(healedSquad[0]);
            }
        }


    }
   

    public void SpawnSquad()
    {
        Transform parent = null;

        if (isPlayer)
        {
            parent = battleSceneManager.playerSquadParent;
        }
        else
        {
            parent = battleSceneManager.enemySquadsParent;
        }

        SquadController squad = Instantiate(squadSpawnPrefab, spawnPoint.position + new Vector3(Random.Range(5.5f,-5.5f),0, Random.Range(5.5f, -5.5f)), squadSpawnPrefab.transform.rotation, parent);

        spawnedSuadsList.Add(squad);


        squad.battleSceneManager = battleSceneManager;

        squad.InitSquad();

        
        

        if (spawnedSuadsList.Count == squadLimit)
        {
            SetSpawner(false);
        }
    }

    public void SetSpawner(bool spawn)
    {

        isSpawner = spawn;

        if (isSpawner)
        {
            isHealer = false;
            spawnerInfoObject.gameObject.SetActive(true);
            healerInfoObject.gameObject.SetActive(false);
        }
        else
        {
            spawnerInfoObject.gameObject.SetActive(false);
        }

        

    }

    public void SetHealer(bool heal)
    {

        isHealer = heal;

        if (isHealer)
        {
            isSpawner = false;
            healerInfoObject.gameObject.SetActive(true);
            spawnerInfoObject.gameObject.SetActive(false);
        }
        else
        {
            spawnerInfoObject.gameObject.SetActive(true);
            healerInfoObject.gameObject.SetActive(false);
        }

    }

    

    public void SpawnedListUpdate()
    {


        for (int i = 0; i < spawnedSuadsList.Count; i++)
        {
            if(spawnedSuadsList[i] == null)
            {
                spawnedSuadsList.RemoveAt(i);
            }

        }
        
    }

    public void UpdateSpawnUi()
    {
        if (newSquadPanel == null && squadSpawnPrefab != null)
        {
            SquadMapUIPanel newPanel = Instantiate(squadSpawnPrefab.mapUIPanel, squadInfoPanel.position, squadInfoPanel.rotation, squadInfoPanel);

            newSquadPanel = newPanel;
        }
        spawnTimerText.text = "" + (int)(timeToSpawn - curretntTimeToSpawn) + " sec";
    }

    public void ClearSpawnUi()
    {
        if(newSquadPanel != null)
        {
            Destroy(newSquadPanel.gameObject);
        }

    }
}
