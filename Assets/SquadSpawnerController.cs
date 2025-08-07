using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class SquadSpawnerController : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    [SerializeField] private bool isSpawner;
    [SerializeField] private bool isHealer;
    [SerializeField] private bool isInit;


    [SerializeField] private List<BuildingManager> buildInGroup;

    [SerializeField] private SquadController squadSpawnPrefab;

    [SerializeField] private List<SquadController> spawnedSuadsList;

    [SerializeField] private float timeToSpawn;
    [SerializeField] private float timeToHeal;
    [SerializeField] private int squadLimit;

    [SerializeField] private SphereCollider collider;
    [SerializeField] private LayerMask sduadMask;

    private float curretntTimeToSpawn;
    private float curretntTimeToHeal;

    [SerializeField] private List<SquadController> healedSquad;

    [SerializeField] private GameObject allVisual;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private GameObject spawnerInfoObject;
    [SerializeField] private Image spawnTimerImage;
    [SerializeField] private TextMeshProUGUI spawnTimerText;

    [SerializeField] private GameObject healerInfoObject;
    [SerializeField] private GameObject cencelInfoObject;


    public BattleSceneManager battleSceneManager;

    // Start is called before the first frame update
    public void Init()
    {
        
        isInit = true;

        SetSpawner(true);

        collider = GetComponent<SphereCollider>();

        allVisual.SetActive(true);
        collider.enabled = true;
    }


    // Update is called once per frame
    void Update()
    {
        if (isInit) {

            if (isSpawner)
            {
                curretntTimeToSpawn += Time.deltaTime;

                if (spawnedSuadsList.Count < squadLimit)
                {
                    spawnTimerText.text = "" + (int)(timeToSpawn - curretntTimeToSpawn) + " sec";
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



                    curretntTimeToHeal = 0;
                }
            }


        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if ((tag == "Player" && other.gameObject.tag == "Enemy") || (tag == "Enemy" && other.gameObject.tag == "Player"))
        {
            SetSpawner(false);
            SetHealer(false);
        }
        else if ((tag == "Enemy" && other.gameObject.tag == "Enemy") || (tag == "Player" && other.gameObject.tag == "Player")) {

            if (other.gameObject.layer == 12) // внутренний коллайдер отряда
            {
                

                SquadController squad = other.GetComponent<SquadController>();

                if (squad.currentAmountUnits < squad.amountUnits)
                {
                    healedSquad.Add(squad);

                    if (healedSquad.Count == 0)
                    {
                        SetSpawner(true);



                    }
                    else
                    {
                        SetHealer(true);
                    }
                }

            }
        }
     }

    private void OnTriggerExit(Collider other)
    {
        if ((tag == "Player" && other.gameObject.tag == "Enemy") || (tag == "Enemy" && other.gameObject.tag == "Player"))
        {
            if (!EnemySquadCheck()) {


                if (healedSquad.Count == 0)
                {
                    SetSpawner(true);



                }
                else
                {
                    SetHealer(true);
                }
                

            }
        }
        else if ((tag == "Enemy" && other.gameObject.tag == "Enemy") || (tag == "Player" && other.gameObject.tag == "Player"))
        {

            if (other.gameObject.layer == 12) // внутренний коллайдер отряда
            {
                SquadController squad = other.GetComponent<SquadController>();

                if (healedSquad.Contains(squad))
                {
                    healedSquad.Remove(squad);
                }

                if (healedSquad.Count == 0)
                {
                    SetSpawner(true);



                }
                else
                {
                    SetHealer(true);
                }

                
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

    public bool EnemySquadCheck()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, collider.radius, sduadMask);

        foreach (var hitCollider in hitColliders)
        {

            if ((tag == "Player" && hitCollider.gameObject.tag == "Enemy") || (tag == "Enemy" && hitCollider.gameObject.tag == "Player"))
            {
                cencelInfoObject.gameObject.SetActive(true);
                return true;
            }

       }
        cencelInfoObject.gameObject.SetActive(false);
        return false;
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

    public void BuildDestroy(BuildingManager build)
    {

        buildInGroup.Remove(build);

        if(buildInGroup.Count == 0)
        {
            Destroy(gameObject);
        }

    }
}
