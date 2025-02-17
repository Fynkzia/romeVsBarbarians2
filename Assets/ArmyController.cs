using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AI;

public class ArmyController : MonoBehaviour
{
    public bool isPlayer = true;

    public string armyName;
    public int armyUnits;
    public int armySqaudCount;

    public int armyPower;

    public bool isSelected;

    public int armySpeed;
    public bool isMoved;
    public Transform targetObject;
    [SerializeField] private NavMeshAgent agent;
    public bool goToJoint;

    public ArmyController jointArmy;

    public float formationTime;
    float currentFormationTime;
    public bool isFormated;
    int formationIndex;
    public Transform[] formationPoints;

    public GameObject[] SquadMapPrefabs;
    public Transform targetObjectPrefab;

    public int armyDefence;

    public bool inBattle;



    public List<SquadController> squadStartList;

    public List<SquadController> squadList;

    public List<SquadController> selectedSquadList;

    public GameObject armyInfoPanel;
    [SerializeField] public TextMeshProUGUI armyNameText;
    [SerializeField] public TextMeshProUGUI armySquadCountText;
    [SerializeField] public TextMeshProUGUI armyUnitsCountText;


    [SerializeField] public LineRenderer lineRendererPrefab;
    [SerializeField] public LineRenderer lineRenderer;

    public ArmyUIPanel armyUIPanel;
    

    public List<Transform> followers;  // Остальные отряды просто меняют Transform
    public float followDistance = 2f;  // Расстояние между юнитами
    public float followSpeed = 5f;  // Скорость передвижения следователей

    private Queue<Vector3> leaderPositions = new Queue<Vector3>();

    [SerializeField] MapControlManager mapControlManager;
    public BoxCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        lineRenderer = Instantiate(lineRendererPrefab);
        mapControlManager = GameObject.Find("MapControlManager").GetComponent<MapControlManager>();
        collider = GetComponent<BoxCollider>();

        if (targetObject == null)
        {
            Transform newTarget = Instantiate(targetObjectPrefab, transform.position, transform.rotation);
            targetObject = newTarget;
        }

        for (int i = 0; i < squadStartList.Count; i++)
        {
            SquadController newSquadController = Instantiate(squadStartList[i], transform.position, transform.rotation);
            newSquadController.gameObject.SetActive(false);
            newSquadController.transform.parent = transform;
            squadList.Add(newSquadController);

            GameObject newSquad = Instantiate(SquadMapPrefabs[0], transform.position,transform.rotation);
            followers.Add(newSquad.transform);
        }
    }
    public void SetMoving( bool move)
    {
        isMoved = move;

        if (!isMoved)
        {
            isFormated = true;

            currentFormationTime = 0;
            formationIndex = 0;
        }
    }

    public void RedyToBattle()
    {
        inBattle = true;
        isMoved = false;

        targetObject.position = transform.position;
        agent.Stop();

        collider.enabled = false;
        armyInfoPanel.SetActive(false);
        targetObject.gameObject.SetActive(false);
    }

    public void ResetAfterBattle()
    {
        inBattle = false;
        isMoved = false;

        targetObject.position = transform.position;
        agent.Stop();

        collider.enabled = true;
        armyInfoPanel.SetActive(true);
        targetObject.gameObject.SetActive(true);
    }


            public int UnitsCountUpdate()
    {
        int count = 0;
        for (int i = 0; i < squadList.Count; i++)
        {
            count += (int)squadList[i].currentAmountUnits;
        }

        armySqaudCount = squadList.Count;
        armyUnits = count;

        return count;
    }

    public int ArmyPowerUpdate()
    {
        int power = 0;
        for (int i = 0; i < squadList.Count; i++)
        {
            power += (int)GameOptions.PowerCalculate(squadList[i]); 
        }

        armyPower = power;

        return power;
    }

    public void GetDamage(ArmyController enAmry)
    {
        int enPower = enAmry.armyPower;
        ArmyPowerUpdate();

        if (enAmry.squadList.Count == 0)
        {
            return;
        }

        Debug.Log("GetDamage enPower - " + enPower);
        Debug.Log("GetDamage plPower - " + ArmyPowerUpdate());

        float powerResult =  enPower/ enAmry.squadList.Count;
        Debug.Log("GetDamage powerResult - " + powerResult);

        int damageCount = enAmry.squadList.Count;
        Debug.Log("GetDamage damageCount - " + damageCount);
        for (int i = 0; i < damageCount; i++)
        {
            SquadController randomSquad = squadList[Random.Range(0, squadList.Count)];
            int multiplayerCount = 1;

            if (randomSquad.AiPowerCalculate() < powerResult)
            {
                multiplayerCount++;
            }

            randomSquad.AutoBattle_GetUnitDie(multiplayerCount);
        }
        SqaudsUpdate();
        ArmyPowerUpdate();
        UnitsCountUpdate();

    }


    public void SqaudsUpdate()
    {



        for (int i = 0; i < squadList.Count; i++)
        {
            SquadController squad = squadList[i];

            if (squad.squadDie){
                RemoveSquad(squad);

                Destroy(squad.gameObject);
            }
        }

    }


    public void AddSquad(SquadController squad,int index)
    {
        SquadController newSquadController = Instantiate(squad, transform.position, transform.rotation);
        newSquadController.gameObject.SetActive(false);
        newSquadController.transform.parent = transform;
        squadList.Add(newSquadController);

        GameObject newSquad = Instantiate(SquadMapPrefabs[0], transform.position, transform.rotation);
        followers.Add(newSquad.transform);



        if (isSelected)
        {
            UpdateSquadsList();
        }

        


    }

    public void RemoveSquad(SquadController squad)
    {
        squadList.Remove(squad);
        
    }

    public void SquadSelect(int index)
    {
       

        if (!selectedSquadList.Contains(squadList[index]))
        {
            selectedSquadList.Add(squadList[index]);
        }
        else
        {
            selectedSquadList.Remove(squadList[index]);
        }
    }

    public void UpdateSquadsList()
    {

        if(mapControlManager == null)
        {
            mapControlManager = GameObject.Find("MapControlManager").GetComponent<MapControlManager>();
        }

        if(mapControlManager.armyUIPanel.active == false)
        {
            mapControlManager.armyUIPanel.SetActive(true);
        }
        armyUIPanel = mapControlManager.armyUIPanel.GetComponent<ArmyUIPanel>();
        armyUIPanel.Clear();


        for (int i = 0; i < squadList.Count; i++)
        {
            SquadMapUIPanel newSquadPanel = armyUIPanel.AddSquad(squadList[i].mapUIPanel);

            int index = i;
            newSquadPanel.GetComponent<Button>().onClick.AddListener(() => SquadSelect(index));
        }
        
    }

    public void ArmySelected()
    {
        isSelected = true;
        UpdateSquadsList();
    }

    public void ArmyDeselect()
    {
        isSelected = false;
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((tag == "Player" && other.gameObject.tag == "Enemy"))//|| (tag == "Enemy" && other.gameObject.tag == "Player")
        {
            ArmyController enemy = other.GetComponent<ArmyController>();

            SceneLoader.Instance.CreateBattleInfo(this, enemy);


            RedyToBattle();

            enemy.RedyToBattle();
           // enemy.collider.enabled = false;

            // enemy.armyInfoPanel.SetActive(false);
            // enemy.targetObject.gameObject.SetActive(false);


        }

        Debug.Log("OnTriggerEnter");
        if (goToJoint)
        {
           
            if (collider.tag == tag)
            {
                if(other.GetComponent<BoxCollider>() == jointArmy.collider)
                {
                    for (int i = 0; i < squadList.Count; i++)
                    {
                        jointArmy.AddSquad(squadList[i], i);

                            }

                        goToJoint = false;

                    for (int i = 0; i < followers.Count; i++)
                    {
                        

                        Destroy(followers[i].gameObject);

                    }

                    Destroy(targetObject.gameObject);

                    Destroy(lineRenderer.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }



    // Update is called once per frame
    void FixedUpdate()
    {
        if (isMoved)
        {
            ArmyMovement();

        }
        else
        {
            if (isFormated)
            {
                currentFormationTime += Time.deltaTime;

                if(currentFormationTime > formationTime)
                {

                    followers[formationIndex].transform.position = formationPoints[formationIndex].position;
                    followers[formationIndex].transform.rotation = formationPoints[formationIndex].rotation;
                    formationIndex++;

                    currentFormationTime = 0;

                    if(formationIndex >= followers.Count)
                    {
                        isFormated = false;
                        currentFormationTime = 0;
                        formationIndex = 0;
                    }
                }
            }
        }
    }

    void ArmyMovement()
    {
        //Vector3 targetPos = Vector3.MoveTowards(transform.position, targetObject.position, armySpeed * Time.fixedDeltaTime);
        agent.SetDestination(targetObject.position);
        leaderPositions.Enqueue(transform.position);

        int maxQueueSize = Mathf.CeilToInt(followers.Count * followDistance);
        while (leaderPositions.Count > maxQueueSize)
        {
            leaderPositions.Dequeue();
        }

        // Двигаем последователей
        int step = Mathf.CeilToInt(followDistance);
        int index = leaderPositions.Count - step;

        for (int i = 0; i < followers.Count; i++)
        {
            if (index >= 0 && index < leaderPositions.Count)
            {
                Vector3 targetPos = leaderPositions.ToArray()[index];
                targetPos.y = followers[i].position.y;  // Не меняем высоту
                followers[i].position = Vector3.Lerp(followers[i].position, targetPos, followSpeed * Time.deltaTime);

                Vector3 direction = targetPos - followers[i].position;
                if (direction.magnitude > 0.1f)  // Проверка, чтобы избежать нулевого направления
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    followers[i].rotation = Quaternion.Slerp(followers[i].rotation, targetRotation, 10f * Time.deltaTime);
                }
            }
            index -= step;

            

        }

        if (Vector3.Distance(followers[followers.Count - 1].position,transform.position)<0.2f)
        {
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f) // Убедимся, что он реально стоит
                {
                    SetMoving(false);
                }
            }
        }
        

        lineRenderer.SetPosition(0,transform.position);
        lineRenderer.SetPosition(1, targetObject.transform.position);


    }

}
