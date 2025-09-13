using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class SmallCityController : MonoBehaviour
{

    [SerializeField] private bool isInit;
    [SerializeField] public bool isPlayer;


    [SerializeField] public int buildCount;

    [SerializeField] public int unitCount;

    [SerializeField] private float timeToBuild;
    [SerializeField] private float timeToAddUnit;


    [SerializeField] private List<BuildingManager> buildInGroup;

    [SerializeField] private List<BuildingManager> playerBuilds;
    [SerializeField] private List<BuildingManager> enemyBuilds;

    public BattleSceneManager battleSceneManager;
    public SquadSpawnerController squadSpawnerController;


    private float currentTimeToAddUnit;
    private float currentTimeToBuild;

    [SerializeField] private TextMeshProUGUI buildCountText;
    [SerializeField] private TextMeshProUGUI unitsCountText;


    [SerializeField] private BoxCollider checkCollider;
    [SerializeField] private LayerMask sduadMask;


    // Start is called before the first frame update
    public void Init()
    {

        isInit = true;



       

        if (isPlayer)
        {
            buildInGroup = playerBuilds;
        }
        else
        {
            buildInGroup = enemyBuilds;
        }

        for (int i = 0; i < buildCount; i++)
        {
            AddBuild();



        }

    }

    // Update is called once per frame
    void Update()
    {
        currentTimeToAddUnit += Time.deltaTime;
        currentTimeToBuild += Time.deltaTime;

        if (currentTimeToAddUnit > timeToAddUnit)
        {
            if(unitCount < buildCount * 5 )
            unitCount++;

            currentTimeToAddUnit = 0;

            UpdateInfoPanel();

        }

        if (currentTimeToBuild > timeToBuild)
        {
            if (buildCount < buildInGroup.Count)
            {
                if (EnemySquadCheck() == false)
                {

                    buildCount++;

                    AddBuild();

                    UpdateInfoPanel();


                }


            }
            currentTimeToBuild = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((tag == "Player" && other.gameObject.tag == "Enemy") || (tag == "Enemy" && other.gameObject.tag == "Squad"))
        {
            squadSpawnerController.SetSpawner(false);
            squadSpawnerController.SetHealer(false);
        }
        else if ((tag == "Enemy" && other.gameObject.tag == "Enemy") || (tag == "Player" && other.gameObject.tag == "Squad"))
        {

            if (other.gameObject.layer == 12) // внутренний коллайдер отряда
            {


                SquadController squad = other.GetComponent<SquadController>();

                if (squad.currentAmountUnits < squad.amountUnits)
                {
                    squadSpawnerController.healedSquad.Add(squad);

                    if (squadSpawnerController.healedSquad.Count == 0)
                    {
                        squadSpawnerController.SetSpawner(true);



                    }
                    else
                    {
                        squadSpawnerController.SetHealer(true);
                    }
                }

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((tag == "Player" && other.gameObject.tag == "Enemy") || (tag == "Enemy" && other.gameObject.tag == "Player"))
        {
            if (!EnemySquadCheck())
            {


                if (squadSpawnerController.healedSquad.Count == 0)
                {
                    squadSpawnerController.SetSpawner(true);



                }
                else
                {
                    squadSpawnerController.SetHealer(true);
                }


            }
        }
        else if ((tag == "Enemy" && other.gameObject.tag == "Enemy") || (tag == "Player" && other.gameObject.tag == "Squad"))
        {

            if (other.gameObject.layer == 12) // внутренний коллайдер отряда
            {
                SquadController squad = other.GetComponent<SquadController>();

                if (squadSpawnerController.healedSquad.Contains(squad))
                {
                    squadSpawnerController.healedSquad.Remove(squad);
                }

                if (squadSpawnerController.healedSquad.Count == 0)
                {
                    squadSpawnerController.SetSpawner(true);



                }
                else
                {
                    squadSpawnerController.SetHealer(true);
                }


            }
        }
    }

    public bool EnemySquadCheck()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position + checkCollider.center, checkCollider.size,transform.rotation, sduadMask);

        

            

        foreach (var hitCollider in hitColliders)
        {
            Debug.Log("EnemySquadCheck " + hitCollider.gameObject.name);
            if ((tag == "Player" && hitCollider.gameObject.tag == "Enemy") || (tag == "Enemy" && hitCollider.gameObject.tag == "Squad"))
            {
                // cencelInfoObject.gameObject.SetActive(true);
               

                return true;
                
            }

        }
        //cencelInfoObject.gameObject.SetActive(false);

     
        return false;
    }

    public void AddBuild()
    {
        for (int i = 0; i < buildInGroup.Count; i++)
        {
            if (buildInGroup[i].isDestroed == true)
            {
                buildInGroup[i].Build();
                return;
            }

        }
    }

    public void SetOwner(bool player)
    {
        if (player)
        {
            isPlayer = true;
            gameObject.tag = "Player";
            squadSpawnerController.isPlayer = true;
            buildInGroup = playerBuilds;

            squadSpawnerController.squadSpawnPrefab = squadSpawnerController.playerDefaultSquadHelper;
        }
        else
        {
            isPlayer = false;
            gameObject.tag = "Enemy";
            squadSpawnerController.isPlayer = false;
            buildInGroup = enemyBuilds;
            squadSpawnerController.squadSpawnPrefab = squadSpawnerController.enemyDefaultSquadHelper;

        }

        squadSpawnerController.ClearSpawnUi();

    }

    public void Capture()
    {
        buildCount = 1;
        unitCount = 0;

        if (!isPlayer)
        {
            buildInGroup = playerBuilds;
            tag = "Player";
        }
        else
        {
            buildInGroup = enemyBuilds;
            tag = "Enemy";
        }


        for (int i = 0; i < buildInGroup.Count; i++)
        {
            buildInGroup[i].isDestroed = true;

            buildInGroup[i].gameObject.SetActive(false);



        }

        buildInGroup[0].gameObject.SetActive(true);
        buildInGroup[1].isDestroed = false;

        if (!isPlayer)
        {
            isPlayer = true;
            squadSpawnerController.isPlayer = true;

            squadSpawnerController.squadSpawnPrefab = squadSpawnerController.playerDefaultSquadHelper;
        }
        else
        {
            isPlayer = false;
            squadSpawnerController.isPlayer = false;
            squadSpawnerController.squadSpawnPrefab = squadSpawnerController.enemyDefaultSquadHelper;

            
        }
        squadSpawnerController.ClearSpawnUi();

    }


    public void BuildDestroy(BuildingManager build)
    {
        buildCount--;

       if(buildCount == 0)
        {
            Capture();
        }
        UpdateInfoPanel();



    }

    public void UpdateInfoPanel()
    {
        buildCountText.text = "" + buildCount ;
        unitsCountText.text = "" + unitCount;
    }

}
