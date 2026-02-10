using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class AIGlobalController : MonoBehaviour
{
    [Space(10)]
    [SerializeField] public AiGlobalaAnalysis analysis;
    [SerializeField] public FormationBuilder formationBuilder;
    [Space(10)]

    [SerializeField] public float timeToGetActions_macro;
    [SerializeField] public float timeToGetActions_micro;
    [Space(10)]
    [SerializeField] public float timeAction_macro;
    [SerializeField] public float timeAction_micro;
    [Space(10)]
    [Header("Distance Settings")]
    [SerializeField] public float groupDist;
    [SerializeField] public float battleCheckDist;
    [Space(10)]
    [SerializeField] public float[] actionPriority_macro;
    [SerializeField] public float[] actionPriority_micro;
    /// ------ макро контроль
    // 0 - атака
    // 1 - ведение боя
    // 2 - защитные действия
    // 3 - штурм города
    // 4 - оборона города
    // 5 - маневры 
    /// ------ микроконтроль
    // 0 - Аттака отряда
    // 1 - Отход отряда
    // 2 - Помошь отряду
    // 3 - 

    [SerializeField] public float[] attackPriority;
    // 0 - фронтовая атака
    // 1 - атака в слабое место
    // 2 - байт на слабое место
    // 3 - маневр всей группой на слабое местро
    // 4 - отправить в атаку стрелков
    // 5 - отправить в атаку кавалерию



    [SerializeField] public float[] battlePriority;
    // 0 - отвод группы в сражении
    // 1 - отправить подкреп группе (тег успех)
    // 2 - отход + регрупировка (тег помощ)


    [SerializeField] public float[] defencePriority;
    // 0 - выстроить линию 
    // 1 - выстроить оборону (небольшая линия + защита флангов)
    // 2 - отвод вглубь тех кого стреляют


    [SerializeField] public float[] cityAttackPriority;
    // есть группа возле города готовая атаковать и победить
    // 0 - мы сильно больше павера города - идем на пролом
    // 1 - мы на уровне павера или немного меньше - сначала ближайшкий забор + вышки. при контакте отходим
    // 2 - свободный отряд + рядом есть здание без защиты

    [SerializeField] public float[] cityDefencePriority;
    // 0 - расставить отряды у ворот
    // 1 - бой у ворот - прислать помошь
    // 2 - бют наш домик - отправить в атаку микро отряд
    // 3 - есть достаточный павер чтоб вывести отряды в контр атакич - ведем в контр атаку

    [SerializeField] public PriorityResult[] handlingPriority;
    // 0 - очень маленькая группа - соединяется с ближайшей
    // 1 - есть тег успех - отправим ближайшую группу, рядом с которой нет группы игрока
    // 2 - есть тег помощ - отправим ближайшую группу, рядом с которой нет группы игрока
    // 3 - средняя группа ничего не делает - идет атаковать самую малую группу игрока
    // 4 - средняя группа + есть заблокированя группа - группа идет чтоб атаковать с противоволожной стороны
    // 5 - большая группа ничего не делает - идет атаковать самую сильную (но не больше павера чем у него)
    // 6 - большая группа ничего не делает - идет блочить отряд которы нас немного больше
    // 7 - большие группы создают среднюю - если у игрока больше групп и есть маля група - формирование группы**
    // 8 - город без надлежашей защиты - отправить подходящие отряды/ или другие/без кавы - формирование отряда** (выдает тег защитники города)
    // 9 - город без надлежайшей защиты у игрока - подходим к городу(не залоканые группы)

    [SerializeField] public BattleGroup[] handlingGrop;
    [SerializeField] public List<Vector3> handlingTargets;

    public class BattleGroup
    {
        public float power;

       public List<SquadController> squadList;
        public FormationPointsProvider points;

        public Vector3 center;

        public int sw = 0;
        public int sp = 0;
        public int cv = 0;
        public int sh = 0;

        
        public bool inMove;
        public bool inBattle;
        public bool isBlocked;
        public bool isCityDefender;

        public bool needHelp;

        public void Recalculate()
        {
            power = 0;
            center = Vector3.zero;

            if (squadList.Count == 0)
                return;

            foreach (var squad in squadList)
            {
                center += squad.transform.position;
                power += squad.powerSquad;
            }

            center /= squadList.Count;
        }

        public void CheckSquads()
        {


            for (int j = 0; j < squadList.Count; j++)
            {
                if ((int)squadList[j].type == 0)
                {
                    sw++;
                }
                if ((int)squadList[j].type == 1)
                {
                    sp++;
                }
                if ((int)squadList[j].type == 2)
                {
                    cv++;
                }
            }
        }
    }

    [SerializeField]
    public struct PriorityResult
    {
        public float weight;                 // 0–10
        public BattleGroup sourceGroup;       // НАША группа
        public BattleGroup targetGroup;       // цель (если есть)
    }


    [SerializeField] public List<SquadController> allPlayerList;
    [SerializeField] public List<SquadController> allEnemiesList;
  
    [SerializeField] public List<SquadController> allEnemiesInBattleList;

    [SerializeField] public List<BattleGroup> playerGroups;
    [SerializeField] public List<BattleGroup> enemyGrops;
    [SerializeField]public List<BattleGroup> enemyGropsInBattle;


    [SerializeField] public float minPowerMultiplayer;
    [SerializeField] public float maxPowerMultiplayer;

    [SerializeField] public float distToAttack;
    [SerializeField] public float distToCover;

    

    [Header("NavMesh Settings")]
    [SerializeField] private NavMeshAgent agent;
    private NavMeshPath path;

    [Header("other Settings")]

    [SerializeField] public BattleSceneManager battleSceneManager;
    [SerializeField] public Transform enemyObject;
    [SerializeField] public Transform playerObject;

    [SerializeField] private LayerMask playerLayer;

    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private LayerMask squadLayer;

    List<SquadController> listToCreateGrops;


    void Start()
    {
        enemyObject = battleSceneManager.enemySquadsParent;
        playerObject = battleSceneManager.playerSquadParent;
    }

    // Update is called once per frame
    void Update()
    {
        timeAction_macro += Time.deltaTime;


        if (timeAction_macro > timeToGetActions_macro)
        {

            AiActionMarco();
        }
        if (timeAction_micro > timeToGetActions_micro)
        {

           // AiActionMarco();
        }
    }

    public void AiActionMarco()
    {
        for (int i = 0; i < actionPriority_macro.Length; i++) ///// очистка
        {
            actionPriority_macro[i] = 0;
        }

        SetData();

        actionPriority_macro[0] = GetPriority_Attack();
        actionPriority_macro[1] = 0;
        actionPriority_macro[2] = 0;
        actionPriority_macro[3] = 0;
        actionPriority_macro[4] = 0;
        actionPriority_macro[5] = GetPriority_Handling();





    }

  
    public float GetPriority_StrategycMoveGroup()
    {
        float score = 0;
        float maxScore = 0;

        // можем кого-то отослать угражать
        for (int i = 0; i < enemyGrops.Count; i++)
        {
            if(enemyGrops[i].power > playerGroups[i].power * minPowerMultiplayer) // группа может дать пизды, подходим побиже
            {
                score = 10;
            }

           

        }

        // можем кого-то отослать просто сдерживать
        if (score == 0)
        {

            for (int i = 0; i < enemyGrops.Count; i++)
            {
                if (enemyGrops[i].power < playerGroups[i].power / minPowerMultiplayer
                   && enemyGrops[i].power > playerGroups[i].power / maxPowerMultiplayer) // группа может только удерживать визави
                {
                    score = 7;
                }
            }
        }

        return maxScore;
    }

    public float GetPriority_Attack()
    {
        float score = 0;
        float maxScore = 0;

        //attackPriority[0] = GetPriority_FrontAttack();

        return maxScore;
    }
    // 5 - hadling
    public float GetPriority_Handling()
    {
        float score = 0;
        float maxScore = 0;

        handlingPriority[0] = GetPriority_HandlSmallGroup();
        handlingPriority[1] = GetPriority_HandlSuccess();
        handlingPriority[2] = GetPriority_HandlHelp();
        handlingPriority[3] = GetPriority_HandlPush();
        handlingPriority[4] = GetPriority_HandlFlanker();
        handlingPriority[5] = GetPriority_HandlCreateMediumGroup();
        handlingPriority[6] = GetPriority_HandlCityDefence();
        handlingPriority[7] = GetPriority_HandlCityAttck();

        return maxScore;
    }

    public PriorityResult GetPriority_HandlSmallGroup()
    {
        PriorityResult best = new PriorityResult
        {
            weight = 0,
            sourceGroup = null,
            targetGroup = null
        };

        for (int i = 0; i < enemyGrops.Count; i++)
        {
            if (!IsFreeGroup(enemyGrops[i])) { 
                continue;
            }

            if(enemyGrops[i].power *minPowerMultiplayer < analysis.avgEnemyGropPower)
            {

                best.targetGroup = FindNearestFriendlyGroup(enemyGrops[i]);

                if (best.targetGroup != null) // нет группы - нет приказу
                {
                    best.weight = 10;
                    best.sourceGroup = enemyGrops[i];
                    break;
                }

            }
        }


            return best;
    }
    public PriorityResult GetPriority_HandlSuccess()
    {
        PriorityResult best = new PriorityResult
        {
            weight = 0,
            sourceGroup = null,
            targetGroup = null
        };

        if(allEnemiesInBattleList.Count == 0 
            )
        {
            return best;
        }



        // ищем точку успеха у сражающихся


        int weight = 0;
        // цикл по всем сражающимся, сравниваем их с опонентами
        for (int i = 0; i < enemyGropsInBattle.Count; i++)
        {
            Collider[] nearEnColliders = Physics.OverlapSphere(enemyGropsInBattle[i].center, battleCheckDist, squadLayer);
            List<SquadController> sqInBattlePoint = new List<SquadController>();
            float plPower = 0;
            float enPower = 0;

            

            for (int j = 0; j < nearEnColliders.Length; j++)
            {
                SquadController sq = nearEnColliders[j].GetComponent<SquadController>();

                sqInBattlePoint.Add(sq);

            }

            for (int j = 0; j < sqInBattlePoint.Count; j++)
            {
                if (sqInBattlePoint[j].playerSquad)
                {
                    plPower += sqInBattlePoint[j].powerSquad;
                }
                else
                {
                    enPower += sqInBattlePoint[j].powerSquad;
                }

                

            }

            float powerRatio = enPower / (float)plPower;

            if (powerRatio >= 2-minPowerMultiplayer && // есть вариант помогать
                powerRatio <= maxPowerMultiplayer)// не больше приемущества в 1.5. если нет - приемкщество уже обеспечно
            {
                int plLowMorale = 0;
                int enLowMorale = 0;
                for (int j = 0; j < sqInBattlePoint.Count; j++)
                {
                    if (sqInBattlePoint[j].playerSquad)
                    {

                        if (sqInBattlePoint[j].currentMorale / sqInBattlePoint[j].maxMorale < 0.4f)
                        {
                            plLowMorale++;
                        }

                    }
                    else
                    {
                       
                    }
                    


                }

              if(plLowMorale > 5)
                {
                    weight = 10;
                }
                else if(plLowMorale >= 3)
                {
                    weight = 7;
                }
                else if (plLowMorale >= 1)
                {
                    weight = 5;
                }

              if(weight > best.weight)
                {
                    best.weight = weight;
                    best.targetGroup = enemyGropsInBattle[i];

                    best.sourceGroup =FindFreeNearestFriendlyGroup(best.targetGroup);
                }
            }

        }



        return best;
    }

    public PriorityResult GetPriority_HandlHelp()
    {
        PriorityResult best = new PriorityResult
        {
            weight = 0,
            sourceGroup = null,
            targetGroup = null
        };

        for (int i = 0; i < enemyGrops.Count; i++)
        {
            if (enemyGrops[i].needHelp)
            {
                BattleGroup nearfreeGroup = FindFreeNearestFriendlyGroup(enemyGrops[i]);
                if (nearfreeGroup != null)
                {
                    best.weight = 10;
                    best.sourceGroup = nearfreeGroup;
                    best.targetGroup = enemyGrops[i];

                    break;
                }
            }
        }


            return best;
    }

    public PriorityResult GetPriority_HandlPush()
    {
        PriorityResult best = new PriorityResult
        {
            weight = 0,
            sourceGroup = null,
            targetGroup = null
        };

        foreach (var e in enemyGrops)
        {
            if (!IsFreeGroup(e))
            {
                continue;
            }

            foreach (var p in playerGroups)
            {
                

                float weight = CalculatePushWeight(e, p);

                if (weight > best.weight)
                {
                    best.weight = weight;
                    best.sourceGroup = e;
                    best.targetGroup = p;
                }
            }
        }


        return best;
    }
    public float GetPriority_HandlMediumFlanker()
    {
        float score = 0;
        float maxScore = 0;



        return maxScore;
    }
    

    public float GetPriority_HandlCreateMediumGroup()
    {
        float score = 0;
        float maxScore = 0;



        return maxScore;
    }

    public float GetPriority_HandlCityDefence()
    {
        float score = 0;
        float maxScore = 0;



        return maxScore;
    }

    public float GetPriority_HandlCityAttck()
    {
        float score = 0;
        float maxScore = 0;



        return maxScore;
    }

    // 5 - hadling

  

    

    public void SetData()
    {
        SetAllPlayersList();
        SetAllEnemiesList();

        playerGroups = BuildGroups(allPlayerList, groupDist);
        enemyGrops = BuildGroups(allEnemiesList, groupDist);



        enemyGropsInBattle = BuildGroups(allEnemiesInBattleList, groupDist);

        analysis.UpdateAnalysis();
    }

    public void SetAllPlayersList()
    {

        allPlayerList = new List<SquadController>();

        for (int i = 0; i < playerObject.transform.childCount; i++)
        {
            if (playerObject.transform.GetChild(i).gameObject.activeSelf)
            {
                allPlayerList.Add(playerObject.transform.GetChild(i).GetComponent<SquadController>());
            }

        }


    }

    public void SetAllEnemiesList()
    {

        allEnemiesList = new List<SquadController>();

        allEnemiesInBattleList = new List<SquadController>();

        for (int i = 0; i < enemyObject.transform.childCount; i++)
        {
            if (enemyObject.transform.GetChild(i).gameObject.activeSelf)
            {
                SquadController squad = enemyObject.transform.GetChild(i).GetComponent<SquadController>();
                allEnemiesList.Add(squad);
                if (squad.inBattle)
                {
                    allEnemiesInBattleList.Add(squad);
                }
            }
        }


    }

    

    public List<BattleGroup> BuildGroups(
        List<SquadController> squads,
        float groupDistance)
    {
        List<BattleGroup> groups = new List<BattleGroup>();
        HashSet<SquadController> used = new HashSet<SquadController>();

        foreach (var startSquad in squads)
        {
            if (used.Contains(startSquad))
                continue;

            BattleGroup group = new BattleGroup();
            Queue<SquadController> queue = new Queue<SquadController>();

            queue.Enqueue(startSquad);
            used.Add(startSquad);

            while (queue.Count > 0)
            {
                SquadController current = queue.Dequeue();
                group.squadList.Add(current);

                foreach (var other in squads)
                {
                    if (used.Contains(other))
                        continue;

                    float dist = Vector3.Distance(
                        current.transform.position,
                        other.transform.position);

                    if (dist <= groupDistance)
                    {
                        used.Add(other);
                        queue.Enqueue(other);
                    }
                }
            }

            group.Recalculate();
            group.CheckSquads();
            groups.Add(group);
        }

        return groups;
    }


    public bool IsFreeGroup(BattleGroup group)
    {
        if(!group.inMove && !group.inBattle && !group.isCityDefender)
        {
            return true;
        }
        else
        {
            return false;
        }
        
    }
    public  BattleGroup FindNearestFriendlyGroup(
    BattleGroup source)
    {
        BattleGroup nearest = null;
        float minDistance = float.MaxValue;

        foreach (var group in enemyGrops)
        {
            if (group == source)
                continue;

            float dist = Vector3.Distance(source.center, group.center);

            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = group;
            }
        }

        return nearest;
    }
    public BattleGroup FindFreeNearestFriendlyGroup(
    BattleGroup source)
    {
        BattleGroup nearest = null;
        float minDistance = float.MaxValue;

        foreach (var group in enemyGrops)
        {
            if (group == source || !IsFreeGroup(group))
                continue;

            float dist = Vector3.Distance(source.center, group.center);

            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = group;
            }
        }

        return nearest;
    }


    public BattleGroup TryToCollectGroup(float targetPower, BattleGroup targetGroup)
    {
       
        float currPower = 0;
        BattleGroup result = new BattleGroup();

        // TODO коллект не из рандомов а выбирать по типам

        for (int i = 0; i < allEnemiesList.Count; i++)
        {
            result.squadList.Add(allEnemiesList[i]);

            currPower += allEnemiesList[i].powerSquad;

            if(currPower >= targetPower)
            {
                break;
            }
        }
        result.Recalculate();

        
        return result;
    }

    public float CheckFreePower()
    {
        float summ = 0;

        for (int i = 0; i < enemyGrops.Count; i++)
        {
            if (i < playerGroups.Count)
            {
                summ += enemyGrops[i].power - playerGroups[i].power;
            }
            else
            {
                summ += enemyGrops[i].power;
            }
        }

        return summ;
    }
}

