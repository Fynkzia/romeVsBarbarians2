using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;


public enum SquadType { 
    Swordsman,
    Spearman,
    Horseman
}

[System.Serializable]
public class Coef {
    public SquadType type;
    public float attack;
    public float defence;
};
public class SquadController : MonoBehaviour {
    [Space(10)]
    [SerializeField] private bool playerSquad;
   
    [Header("Balance specs")]

    [SerializeField] public SquadType type;
    [SerializeField] private float levelSquad = 1f;
    [SerializeField] public float amountUnits;
    [Space(10)]
    [SerializeField] public float powerSquad;
    [SerializeField] public float defenceSquad;
    [SerializeField] public float pushSquad;
    [SerializeField] public float formationSquad;
    [SerializeField] public float maxMorale;
    [Space(10)]
    [SerializeField] public float actionTime;

    [Space(10)]
    [SerializeField] public float actionColliderRadius;

    [Space(10)]
    [Header("Movement specs")]
    [SerializeField] public float movementSpeed;
    [SerializeField] public float boostSpeed;


    [Header("Defence - Formation specs")]
    [Space(10)]
    [SerializeField] public float minimumFormation;
    [SerializeField] public float lostDefenceMoving;
    [SerializeField] public float resetDefence;

    [Space(10)]
    [Header("Morale specs")]

    [SerializeField] public float lostMoraleThenRun;
    [SerializeField] public float lostMoraleThenDie;
    [SerializeField] public float lostMoraleThenAttacked;
    [SerializeField] public float addMoraleKillSquad = 1f;
    [SerializeField] public float addMoraleKillUnit = 0.1f;
    [SerializeField] public float recoveryMoraleSpeed;


    [Space(10)]
    [Header("Retreat specs")]

    [SerializeField] private float retreatCount;
    [SerializeField] private float retreatMinCount;

    [Space(10)]
    [Header("Coef specs")]
    [SerializeField] public Coef[] coef;
    [SerializeField] private float leftTriggerCoef;
    [SerializeField] private float rightTriggerCoef;
    [SerializeField] private float frontTriggerCoef;
    [SerializeField] private float backTriggerCoef;

    [Header("Economy specs")]
    [SerializeField] private int coinsFromDeath;

    [Space(30)]
    [Header("Main/Debug")]

    [Space(10)]
    [Header("Sates")]

    [SerializeField] public bool isMoved = false;
    [SerializeField] public bool isStopRot = false;
    [SerializeField] public bool inBattle = false;
    [SerializeField] public bool isGoingToEnemy = false;

    [SerializeField] private bool battleRot = false;
    [SerializeField] private bool battleMove = false;
    [SerializeField] private bool moveRot = false;
    [SerializeField] public bool escape = false;
    [SerializeField] public bool squadDie = false;
    [SerializeField] public bool canFirstAttack = false;
    [SerializeField] public bool isStunned = false;
    [SerializeField] public bool inBuildBattle = false;



    [Space(10)]
    [Header("Current Spec")]
    [Space(10)]
    [SerializeField] public float currentAmountUnits;
    [SerializeField] public float currentMorale;
    [SerializeField] public float currentFormation;
    [SerializeField] public float currentSpeed;
    [SerializeField] public float stunnedTime = 0f;

    public float currentTriggerCoef;

    private Quaternion squadRotate;
    private Vector3 moveDir;
    private float lastMorale = 0;
    


    [Header("Movement Settings")]
    [Space(10)]
    [SerializeField] private float angleToUpdateFormation = 35f;
    [SerializeField] private float lossSpeedToStop = 2f;
    [SerializeField] private float timeToStop = 2f;

    [SerializeField] private float maxEscapeTime;
    private float lastSpeed;
    private float currentTimeToStop;

    private float movingDistanceToPoint;
    public Vector3[] movingPositions;
    public int indexMove = 0;

    private Vector3 previousPosition;
    public Vector3 calculatedVelocity;

    bool isBackwardsFormation;


    [Header("Fighting Settings")]
    [Space(10)]

   
    [SerializeField] public float attackCoef;
    [SerializeField] public float defenceCoef;



    [Header("Detect Units Settings")] // Селект файтинг юнитс - узнаем где какие войны в бою
    [Space(10)]
    [SerializeField] public List<GameObject> avaliableToAttack;
    [SerializeField] public LayerMask detectionMask;
    [SerializeField] public LayerMask maskToPush; 
    private float lastFormationValue = 0;
    private int lastAmountUnits = 0;
     private bool updatefirstFormation;
    [SerializeField] private bool updateFormation;
    [SerializeField] private int updateFormationIndex;
    [SerializeField] private float timeToUpdateUnit = 0;
    [SerializeField] private float currentTimeToUpdateUnit = 0;


    [Header("Animation Settings")]
    [Space(10)]
    [SerializeField] public int animationState;
    [HideInInspector] public List<AnimationController> animatorControllers = new List<AnimationController>();
     private int runAnimIndex = 0;
     private GameObject lastAttakedUnit;
     Vector3 directionToEnemy;
     float moveAnimtime;


    [SerializeField] public float animationIdleUpdateTime;
    [SerializeField] public float animationMoveUpdateTime;
    [SerializeField] public float animationFightUpdateTime = 0.1f;


    private Vector3[] initialPositions;
    private int damageCountAnimations = 0;

    [Header("FX Settings")]
    [Space(10)]
    [SerializeField] private ParticleSystem moveFX;
    [SerializeField] private ParticleSystem[] bloodFx;
    [SerializeField] private GameObject[] fightFx;
    [SerializeField] private GameObject deadFx;
    [SerializeField] private GameObject addMoraleFx;
    [SerializeField] private GameObject lostMoraleFx;
    [SerializeField] private GameObject friendlyFireFx;
    [Space(10)]
    [SerializeField] public float moveFxInterval = 0.5f; 
    [SerializeField] public int moveFXMax = 0;
    [SerializeField] private List<ParticleSystem> moveFXlist;
    private float moveFxtimer = 0;

    [Header("Enemy AI settings")]
    [Space(10)]
    [SerializeField] public float ai_squadPower = 0;
    [SerializeField] public float ai_groupPower = 0;
    [SerializeField] public bool ai_needHelp;

    [SerializeField] public int ai_currentState = 0;

    [Header("Times")]
    [Space(10)]
    private float currentActionTime = 0f;
    private float restorTime = 0f;
    private float escapeTime = 0f;
    private float startTapTime = 0;
    private float maxTimeWait = 0.5f;
    [HideInInspector] public float animTime = 0;


    [Header("Misc")]
    [Space(10)]
    private CoinsController coinsController;
    private SquadControlManager controlController;
    private AIController aIController;
    private WinLoseManager winLoseManager;

    
    [Space(10)]
    [SerializeField] private GameObject unitPrefab;
    [Space(10)]

    [SerializeField] public SphereCollider TriggerObject;
    [SerializeField] public SphereCollider mainCollider;
    [SerializeField] public Collider predictEnemy;
    [SerializeField] public BuildingManager predictBuild;
    private Rigidbody rb;
    [SerializeField] public List<GameObject> unitArray;
    [SerializeField] public List<SquadController> enemyController = new List<SquadController>();

    [SerializeField] public LineRenderer lineRenderer;


    [SerializeField] private Animator UnitInfo;
    [SerializeField] private GameObject drawingPrefab;
    [SerializeField] private GameObject movementIndicator;
    [SerializeField] private GameObject battleIndicator;
    [SerializeField] public GameObject shootingIndicator;
    [SerializeField] public PointsController unitPositions;

    [SerializeField] private GameObject shadowObject;
    [SerializeField] private GameObject selectObject;

    private Vector3 shadowStartScale ;
    private float selectStartScale = 0;

    [HideInInspector] public int tapCount = 0;

    private const string ENEMY_TAG = "Enemy";
    private const string SQUAD_TAG = "Squad";
    private const string ENEMY_TRIGGER_TAG = "EnemyTrigger";
    private const string SQUAD_TRIGGER_TAG = "SquadTrigger";


    public event Action OnInBattleTrue;
    public event Action OnSquadDie;

    public event Action<int> OnUnitsCountChange;


    private void Start() {

        coinsController = GameObject.Find("CoinsController").GetComponent<CoinsController>();
        controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();
        aIController = GameObject.Find("AIManager").GetComponent<AIController>();
        winLoseManager = GameObject.Find("WinLoseManager").GetComponent<WinLoseManager>();
        rb = GetComponent<Rigidbody>();
        mainCollider = GetComponent<SphereCollider>();

       

        movingDistanceToPoint = GameOptions.distanceToPoint;

        //юнитиы
        for (int i = 0; i < amountUnits; i++)
        {
           GameObject unit = Instantiate(unitPrefab, transform.position, unitPrefab.transform.rotation, transform);

            unitArray.Add(unit);

            animatorControllers.Add(unit.GetComponent<AnimationController>());

        }

        
        currentAmountUnits = unitArray.Count;
        lastAmountUnits = unitArray.Count;
        lastMorale = maxMorale;

        currentMorale = maxMorale;
        currentFormation = formationSquad;
        currentActionTime = actionTime;
        TriggerObject.radius = actionColliderRadius;
       

        shadowStartScale = shadowObject.transform.localScale;
        selectStartScale = selectObject.transform.localScale.x;

        //анимации
       


        animationState = 0;
        SpriteAnimationChange();

        //формация


       


        initialPositions = new Vector3[animatorControllers.Count];

        for (int i = 0; i < animatorControllers.Count; i++)
        {
            initialPositions[i] = animatorControllers[i].transform.localPosition;
            initialPositions[i].y += Random.Range(-0.1f, 0.1f);
        }

        RadiusUpdate();
        updatefirstFormation = true;
        UnitsTransformInFormation();


        //ai
        AiPowerCalculate();

        for (int i = 0; i < moveFXMax; i++)
        {
            GameObject newParticle = Instantiate(moveFX.gameObject, Vector3.zero, Quaternion.identity, transform);
            ParticleSystem ps = newParticle.GetComponent<ParticleSystem>();
            ps.Stop(); // Останавливаем, чтобы не проигрывался сразу
            moveFXlist.Add(ps);
            //ps.gameObject.SetActive(false);
        }

        OnUnitsCountChange?.Invoke((int)currentAmountUnits);

    }

    private void FixedUpdate()
    {
        if (!squadDie)
        {
            animTime += Time.fixedDeltaTime;

            if (!isMoved && !inBattle)
            {



                if (animTime > animationIdleUpdateTime)
                {
                    SpriteAnimationChange();

                    animTime = 0;


                }
            }
            else
            {


                if (isMoved)
                {
                    if (animTime > animationMoveUpdateTime / (currentSpeed/movementSpeed))
                    {
                        SpriteAnimationChange();
                        animTime = 0;
                        
                    }
                    MoveAnimation();

                    moveFxtimer += Time.deltaTime;

                    if (moveFxtimer >= moveFxInterval)
                    {
                        ActivateNextParticle();
                        moveFxtimer = 0f;
                    }
                }

                if (inBattle)
                {
                    if (animTime > animationFightUpdateTime)
                    {
                        if (inBuildBattle && predictBuild == null)
                        {

                            SetBattle(false);
                            // return;
                            animTime = 0;
                        }


                        SpriteAnimationChange();
                        animTime = 0;
                        FigthAnimation();

                    }


                }




            }


            if (!isStunned)
            {

                if (isMoved )
                {
                    SquadMovement();

                    calculatedVelocity = (rb.position - previousPosition) / Time.fixedDeltaTime;

                    // Сохраняем текущую позицию для следующего кадра
                    previousPosition = rb.position;

                    float rbSpeed = calculatedVelocity.magnitude;

                    if (rbSpeed >= movementSpeed) // елси достигли максимальной скорости - можем врываться
                    {
                        canFirstAttack = true;
                    }

                        if (rbSpeed < 10f && !escape) // ебейший костыль
                    {

                       

                        if (rbSpeed > lastSpeed)
                        {
                            lastSpeed = rbSpeed;
                            currentTimeToStop = 0;

                        }

                        if (rbSpeed + lossSpeedToStop < lastSpeed)
                        {
                            currentTimeToStop += Time.deltaTime;



                            if (currentTimeToStop > timeToStop)
                            {
                               
                               // Debug.Log("тут баш шоли?? - " + gameObject.name + "escape =" + escape, gameObject);
                                CancelMovement();

                            }


                        }
                    }

                    FormationBonusChange(-lostDefenceMoving * Time.deltaTime);


                    if (tapCount > 0)
                    {
                        startTapTime += Time.deltaTime;

                        if (startTapTime > maxTimeWait)
                        {
                            tapCount = 0;
                            startTapTime = 0;
                        }

                        if (tapCount == 2)
                        {
                            CancelMovement();
                        }
                    }

                    
                }



                if (inBattle)
                {
                    currentActionTime += Time.fixedDeltaTime;
                    // animationAttackTime += Time.fixedDeltaTime + (Random.Range(-0.1f, 0.1f) * Time.fixedDeltaTime);

                    if (currentActionTime >= actionTime) // вписываем экшн юнитс + (0.3f - actionUnits / currentAmountUnits)
                    {
                        if (inBuildBattle)
                        {
                            SetBuildDamage();
                        }
                        else
                        {
                            SetEnemyDamage();
                        }
                        currentActionTime = 0f;



                    }

                }
            }
            else
            {
                stunnedTime -= Time.fixedDeltaTime;

                if (stunnedTime <= 0)
                {
                    stunnedTime = 0;
                    isStunned = false;
                }
            }


            restorTime += Time.fixedDeltaTime;
            if (restorTime > 1f)
            {
                if (!inBattle && !isMoved && currentMorale < maxMorale)
                {

                    MoraleChange(recoveryMoraleSpeed * (currentAmountUnits / amountUnits));

                }



                if (!inBattle && !isMoved && currentFormation <= formationSquad)
                {
                    FormationBonusChange(resetDefence * (currentMorale / maxMorale));

                }

                restorTime = 0;
            }

            if (escape)
            {
                if (escapeTime < maxEscapeTime)
                {
                    escapeTime += Time.fixedDeltaTime;
                }
                else
                {
                   

                    Escape(false);
                    escapeTime = 0;
                }

                if(predictEnemy != null)
                {
                    if (Vector3.Distance(transform.position, predictEnemy.transform.position) < TriggerObject.radius)
                    {
                        Escape(false);
                        escapeTime = 0;
                    }
                }
            }

            if (updateFormation)
            {
                if (isMoved || updatefirstFormation)
                {
                    currentTimeToUpdateUnit += Time.deltaTime;
                }else if(inBattle || !isMoved)
                {
                    currentTimeToUpdateUnit += Time.deltaTime/2f;
                }

                

                if (currentTimeToUpdateUnit >= timeToUpdateUnit)
                {
                    

                    for (int i = 0; i < unitArray.Count ; i++)
                    {
                   
                           
                        
                            if (unitArray[i] != null)
                            {
                                unitArray[i].transform.position = Vector3.MoveTowards(unitArray[i].transform.position, unitPositions.points[i].position, movementSpeed *5f  * Time.deltaTime);
                                initialPositions[i] = unitArray[i].transform.localPosition;

                              
                            }
                            else
                            {
                            }

                            if (i == unitArray.Count - 1)
                            {
                               
                            }
                       
                    }

                    currentTimeToUpdateUnit = 0;
                    //Debug.Log("updateFormation - " + Vector3.Distance(unitPositions.points[unitArray.Count - 1].position, unitArray[unitArray.Count - 1].transform.position));
                }

              

                    if (Vector3.Distance(unitPositions.points[unitArray.Count - 1].position, unitArray[unitArray.Count-1].transform.position) < 0.05f &&

                    Vector3.Distance(unitPositions.points[0].position, unitArray[0].transform.position) < 0.05f)

                    {


                        updateFormationIndex = 0;

                        currentTimeToUpdateUnit = 0;

                        updateFormation = false;

                        if (updatefirstFormation)
                        {
                            updatefirstFormation = false;
                        }
                    }
                    else
                    {
                       
                    }
               
            }

        }
    }




    private void SquadMovement() {

        
            if (lineRenderer == null)
            {
               
                return;
            }


            if (indexMove < lineRenderer.positionCount - 1)
            {
                movingPositions = new Vector3[(int)lineRenderer.positionCount];
                lineRenderer.GetPositions(movingPositions);

                Vector3 linePosition = new Vector3(lineRenderer.GetPosition(1).x, transform.position.y, lineRenderer.GetPosition(1).z);
                Vector3 targetPos = Vector3.MoveTowards(transform.position, linePosition, CurrentSpeed() * Time.fixedDeltaTime);

                float y = MathUtilities.AngleBetweenTwoPoints(transform.position, lineRenderer.GetPosition(1));
             

                float angleDifference = Quaternion.Angle(squadRotate, Quaternion.Euler(new Vector3(0f, y, 0f)));


           
             isBackwardsFormation = angleDifference > 90f;

            // Если отряд должен развернуться назад
            if (isBackwardsFormation)
            {



                //if (angleDifference > angleToUpdateFormation)
                //{
                //    unitPositions.transform.rotation = Quaternion.Euler(new Vector3(0f, y + 180f, 0f));
                //    UnitsTransformInFormation(); // Обновляем позиции юнитов
                //    squadRotate = Quaternion.Euler(new Vector3(0f, y, 0f));

                //}

                unitPositions.transform.localScale = new Vector3(unitPositions.transform.localScale.x * -1f, unitPositions.transform.localScale.y, unitPositions.transform.localScale.z * -1f);

            }
            else
            {
                

                
            }

            if (angleDifference > angleToUpdateFormation)
            {
                //Debug.Log("angleDifference " + angleDifference);

                unitPositions.transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
                UnitsTransformInFormation(); // Обновляем позиции юнитов
                squadRotate = Quaternion.Euler(new Vector3(0f, y, 0f));
            }

            // Доворачиваем, если разница углов больше допустимого порога

            //angleDifference = Quaternion.Angle(squadRotate, Quaternion.Euler(new Vector3(0f, y, 0f)));





            if (!isStopRot)
                {

                    float directionz = targetPos.z - transform.position.z;
                    float directionx = targetPos.x - transform.position.x;

                    Vector2 lineVec = new Vector2(-1, 1) - new Vector2(1, -1);

                    float crossProduct = lineVec.x * directionz - lineVec.y * directionx;

                    if (crossProduct < 0)
                    {

                        for (int i = 0; i < animatorControllers.Count; i++)
                        {
                            Transform mesh = animatorControllers[i].transform.GetChild(0).transform;
                            if (mesh.transform.localScale.x > 0)
                                mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
                        }

                    }
                    else
                    {

                        for (int i = 0; i < animatorControllers.Count; i++)
                        {
                            Transform mesh = animatorControllers[i].transform.GetChild(0).transform;
                            if (mesh.transform.localScale.x < 0)
                                mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
                        }
                    }

                    rb.MovePosition(targetPos);
                    lineRenderer.SetPosition(0, transform.position);

                }

                if (Vector3.Distance(rb.position, linePosition) < movingDistanceToPoint)
                {
                    var pointsList = new List<Vector3>(movingPositions);
                    pointsList.RemoveAt(1);
                    

                    if (predictEnemy != null && predictEnemy.transform.position != pointsList[pointsList.Count-1])
                    {
                        pointsList.Add(predictEnemy.transform.position);
                        indexMove--;
                    }


                    movingPositions = pointsList.ToArray();
                    lineRenderer.SetPositions(movingPositions);
                    indexMove++;
                    currentTimeToStop = 0;
                    lastSpeed = 0;

                if (predictEnemy != null && lineRenderer.positionCount > 2)
                        {
                            //lineRenderer.SetPosition(lineRenderer.positionCount-1, predictEnemy.transform.position);
                        }
                    }

                

                MoraleChange(-(lostMoraleThenRun *Time.deltaTime));

            }
            else
            {
                CancelMovement();


               
            }
        
        //else ///  --- подходим чуууточку ближе 
        //{
        //    Vector3 targetPos = Vector3.MoveTowards(transform.position, moveDir, CurrentSpeed() * Time.fixedDeltaTime);

        //    if(predictEnemy == null)
        //    {
                
        //        battleMove = false;
        //        CancelMovement();
        //        return;
        //    }

        //    if (Vector3.Distance(rb.position, predictEnemy.transform.position) > (TriggerObject.radius * 1.1f))
        //    {
        //        rb.MovePosition(targetPos);

        //    }
        //    else
        //    {
        //        float y = MathUtilities.AngleBetweenTwoPoints(transform.position, predictEnemy.transform.position);

        //            unitPositions.transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
        //            moveRot = false;
        //            UnitsTransformInFormation();
        //            squadRotate = unitPositions.transform.rotation;

                

        //        battleMove = false;
                
        //    }
        //}
    }

    public void SetMoving(bool isMoved) {
        this.isMoved = isMoved;

        movementIndicator.SetActive(isMoved);
        if (!inBattle)
        {
            if (isMoved)
            {
                animationState = 1;

                for (int i = 0; i < animatorControllers.Count; i++)
                {
                    

                    if (Random.Range(1, 3) % 2 == 0)
                    {
                        animatorControllers[i].SpriteAnimationChange(2);
                    }
                    else
                    {
                        animatorControllers[i].SpriteAnimationChange(1);
                    }

                    animatorControllers[i].SpriteReset();
                }

                    FormationBonusChange(-lostDefenceMoving * 2); // / баланс уменшаем защиту при любом движении
                
            }
            else
            {
                animationState = 0;
            }
        }
        else
        {
            if (isMoved)
            {

            }
            animationState = 2;
            
        }

        if (!isMoved)
        {
            RadiusUpdate();
            UnitsTransformInFormation();
           // squadRotate = unitPositions.transform.rotation;
            
            animTime = animationIdleUpdateTime;
        }

    }

    public void SetBattle(bool inBattle) {

        if (inBattle)
        {
            animationState = 2;

         
            OnInBattleTrue?.Invoke();


        }
        else
        {
            if (isMoved)
            {
                animationState = 1;

                for (int i = 0; i < animatorControllers.Count; i++)
                {


                    if (Random.Range(1, 3) % 2 == 0)
                    {
                        animatorControllers[i].SpriteAnimationChange(2);
                    }
                    else
                    {
                        animatorControllers[i].SpriteAnimationChange(1);
                    }

                    animatorControllers[i].SpriteReset();
                }
            }
            else
            {
                animationState = 0;
               
            }

            canFirstAttack = true;


        }

        if (inBuildBattle && !inBattle)
        {
            inBuildBattle = false;
            predictBuild = null;
        }

        this.inBattle = inBattle;
        battleIndicator.SetActive(inBattle);


        if (inBattle)
        {
            if (enemyController.Count > 0)
            {
                //directionToEnemy = (enemyController[0].transform.position - (targetPosition + transform.position)).normalized;
                if (enemyController[0] != null)
                {

                    directionToEnemy = enemyController[0].transform.position;
                }
            }
            else
            {
                if (predictBuild == null)
                {
                    return;
                } ////////////////// можно улучшить!!

                //directionToEnemy = (predictBuild.transform.position - (targetPosition + transform.position)).normalized;
                directionToEnemy = predictBuild.transform.position;
            }
        }



    }


    public void CountCoef(SquadType enemyType)
    {
        foreach (Coef el in coef)
        {
            if (el.type == enemyType)
            {
                attackCoef = el.attack;
                defenceCoef = el.defence;
            }
        }
    }
    public Vector2 CountCoefAnalis(SquadType enemyType)
    {
        foreach (Coef el in coef)
        {
            if (el.type == enemyType)
            {
                return new Vector2(el.attack, el.defence);
            }
        }
        return new Vector2(0, 0);
    }

    private float AttackTriggerCoef(SquadController en)
    {
        if(en == null)
        {
            return 0;
        }

        Quaternion enRot = en.squadRotate;
        

        Quaternion rotationDifference = Quaternion.Inverse(enRot) * squadRotate;

        // Преобразование разницы вращений в угол
        float angleY = Mathf.Abs(rotationDifference.eulerAngles.y);

        

        // Приведение угла в диапазон [0, 180]
        if (angleY > 180.0f)
        {
            angleY = 360.0f - angleY;
        }

        Debug.Log("angleY " + angleY, gameObject);

        // Условия сравнения
        if (angleY <= 5.0f)
        {
            Debug.Log("!!!ебашим во спину - angleY =" + angleY, gameObject);
            return backTriggerCoef; // Объекты смотрят в одну сторону
           
        }
        else if (angleY > 85.0f && angleY < 95.0f)
        {
            Debug.Log("ебашим во бочину - angleY =" + angleY);
            return leftTriggerCoef; // Объекты перпендикулярны
        }
        else if (angleY >= 175.0f)
        {
            return frontTriggerCoef; // Объекты смотрят в противоположные стороны
        }



        return 0;
    }


    private float CurrentSpeed() {
        float currentMoveSpeed = movementSpeed * MoraleToSpeedEffectIndex();
        float currentBoostSpeed = boostSpeed ;
        if (currentSpeed < currentMoveSpeed) {
            currentSpeed += currentBoostSpeed * Time.fixedDeltaTime;
        }
        else {
            currentSpeed = currentMoveSpeed;
        }
        return currentSpeed;
    }

    private float FormationEffectIndex()
    {
        return currentFormation / formationSquad; 
    }

    private float MoraleEffectIndex() {


        if (currentMorale / maxMorale > 0.9)
        {
            return 1f;
        }
        if (currentMorale / maxMorale > 0.75)
        {
            return 0.9f;
        }
        if (currentMorale / maxMorale > 0.5)
        {
            return 0.7f;
        }
        if (currentMorale / maxMorale > 0.25)
        {
            return 0.5f;
        }

        return 0.5f;
    }


    private float MoraleToSpeedEffectIndex()
    {
        if (currentMorale / maxMorale > 0.9)
        {
            return 1f;
        }
        if (currentMorale / maxMorale > 0.75)
        {
            return 0.9f;
        }
        if (currentMorale / maxMorale > 0.5)
        {
            return 0.7f;
        }
        if (currentMorale / maxMorale > 0.25)
        {
            return 0.5f;
        }

        return 0.5f;
    }

   



    public void MoraleChange(float morale) {
        if(currentMorale + morale > 0.1 && currentMorale + morale < maxMorale)
        {
            currentMorale += morale;

            //Debug.Log("MoraleChange - morale=" + morale, gameObject);
        }
        
        if (currentAmountUnits < retreatMinCount)
        {
            RetreatUpdate();
        }

        if(lastMorale - currentMorale > 1 )
        {
            
                Instantiate(lostMoraleFx, transform.position, transform.rotation);

            lastMorale = currentMorale;
            RetreatUpdate();
        }

        if (lastMorale - currentMorale < -1)
        {
             Instantiate(addMoraleFx, transform.position, transform.rotation);

            lastMorale = currentMorale;
            RetreatUpdate();
        }

       
    }

    public void FormationBonusChange(float newDefence)
    {
        if (currentFormation + newDefence > 1 && currentFormation + newDefence < formationSquad)
        {
            currentFormation += newDefence;
        }
        if (currentFormation  - lastFormationValue > 1f || currentFormation - lastFormationValue < -1f)
        {

            RadiusUpdate();
            UnitsTransformInFormation();
        }


    }


    // moving
    public void CancelMovement()
    {
        SetMoving(false);

        if (lineRenderer != null)
        {
            Destroy(lineRenderer.gameObject);
        }

       

        indexMove = 0;
        currentTimeToStop = 0;
        lastSpeed = 0;

        movingPositions = null;
        tapCount = 0;
        startTapTime = 0;
        isGoingToEnemy = false;
        canFirstAttack = false;

        currentSpeed = 0;

        if (escape)
        {
            Escape(false);
            escapeTime = 0;
        }

        //Collider[] nearColliders = Physics.OverlapSphere(transform.position, TriggerObject.radius, maskToPush,QueryTriggerInteraction.Collide);



        //if (nearColliders.Length > 0)
        //{
        //    Vector3 dir = new Vector3();
        //    for (int i = 0; i < nearColliders.Length; i++)
        //    {

        //        if (nearColliders[i] != TriggerObject)
        //        {
        //            if ((tag == SQUAD_TAG && TriggerObject.gameObject.tag == SQUAD_TAG) || (tag == ENEMY_TAG && TriggerObject.gameObject.tag == ENEMY_TAG))
        //            {
        //                dir += (nearColliders[i].transform.position - transform.position);
        //            }

        //        }
        //    }

        //    Debug.Log("this collider - " + dir+ " mag - " + dir.magnitude );

        //    //float force = 100f * Vector3.Distance(nearColliders[0].transform.position, transform.position);

        //    if (dir.magnitude > 1.5f)
        //    {
        //        //SquadPush(-dir.normalized, (1 / dir.magnitude) * 800f);
        //    }
        //}
    }

    public void GoToSquad(Transform squad)
    {
        if (isMoved)
        {
            indexMove = 0;
            movingPositions = null;
            // CancelMovement();
            if (lineRenderer != null)
            {
                Destroy(lineRenderer.gameObject);
            }
            Debug.Log("GoToSquad", gameObject);

        }
        moveDir = squad.position;
        predictEnemy = squad.gameObject.GetComponent<Collider>();

        GameObject drawing = Instantiate(drawingPrefab);
        LineRenderer _lineRenderer = drawing.GetComponent<LineRenderer>();
        lineRenderer = _lineRenderer;

        Vector3 norm = Vector3.Normalize(squad.transform.position - transform.position);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position + norm);


        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, squad.transform.position);

        if (inBattle)
        {
           // battleMove = true;
        }
        else
        {
            SetMoving(true);
        }
       
    }

    public void SpriteRotate(Vector3 target, Transform unit)
    {
        float directionz = target.z - unit.transform.position.z;
        float directionx = target.x - unit.transform.position.x;

        Vector2 lineVec = new Vector2(-1, 1) - new Vector2(1, -1);

        float crossProduct = lineVec.x * directionz - lineVec.y * directionx;

        if (crossProduct < 0)
        {

            for (int i = 0; i < animatorControllers.Count; i++)
            {
                Transform mesh = animatorControllers[i].transform.GetChild(0).transform;
                if (mesh.transform.localScale.x > 0)
                    mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
            }

        }
        else
        {

            for (int i = 0; i < animatorControllers.Count; i++)
            {
                Transform mesh = animatorControllers[i].transform.GetChild(0).transform;
                if (mesh.transform.localScale.x < 0)
                    mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
            }
        }

        //float directionz = target.z - unit.position.z;
        //float directionx = target.x - unit.position.x;

        //Transform mesh = unit.transform.GetChild(0).transform;

        //if (directionx < 0 && directionz <= 0)
        //{

        //    if (mesh.transform.localScale.x < 0)
        //        mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);

        //}
        //else if (directionx > 0 && directionz >= 0)
        //{

        //    if (mesh.transform.localScale.x > 0)
        //        mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);

        //}

        //if (directionx < 0 && directionz >= 0)
        //{

        //    if (mesh.transform.localScale.x > 0)
        //        mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);

        //}
        //else if (directionx > 0 && directionz <= 0)
        //{

        //    if (mesh.transform.localScale.x > 0)
        //        mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);

        //}

    

    }


    // triggers
    public void OnMainTriggerEnter(Collider enemyCollider) { 
        if (!escape) {

            if ((tag == SQUAD_TAG && enemyCollider.gameObject.tag == ENEMY_TAG) || (tag == ENEMY_TAG && enemyCollider.gameObject.tag == SQUAD_TAG))
            {


                if (enemyCollider.gameObject.layer == 12) // внутренний коллайдер отряда
                {
                    
                    

                } else if (enemyCollider.gameObject.layer == 11)// внешний коллайдер отряда - подходим чтоб начать пиздилку
                {
                    SquadController squad = enemyCollider.transform.parent.gameObject.GetComponent<SquadController>();


                    if (!inBattle)
                    {
                        if (!enemyController.Contains(squad))
                        {
                            
                            GoToSquad(enemyCollider.transform);
                        }
                    }
                    else // дрались со зданием, все забыли
                    {
                        if (inBuildBattle) {

                            predictEnemy = null;
                            inBuildBattle = false;
                            SetBattle(false);

                            if (!enemyController.Contains(squad))
                            {

                                GoToSquad(enemyCollider.transform);
                            }
                        }
                        
                    }

                }
            }



        }



    }

    public void OnMainTriggerExit(Collider other) {
       
            if ((tag == SQUAD_TAG && other.gameObject.tag == ENEMY_TAG) || (tag == ENEMY_TAG && other.gameObject.tag == SQUAD_TAG))
            {

                if (other.gameObject.layer == 12) // внутренний коллайдер отряда
                {

                    if (!escape)
                    {
                        SquadController squadExiter = other.transform.gameObject.GetComponent<SquadController>();

                        if (squadExiter.escape)
                        {

                            if (enemyController.Count == 0)
                            {

                                //SetBattle(false);
                            }
                        }
                        else
                        {
                            if (!inBattle)
                            {

                                GoToSquad(other.transform);
                            }
                            else
                            {
                                if (enemyController.Contains(squadExiter) && enemyController.Count == 1)
                                {


                                    GoToSquad(other.transform);
                                }
                            }
                        }



                    }
                }
                else if (other.gameObject.layer == 11)// внешний коллайдер отряда - подходим чтоб начать пиздилку
                {



                    SquadController squadExiter = other.transform.parent.gameObject.GetComponent<SquadController>();



                    if ((tag == SQUAD_TAG && squadExiter.tag == ENEMY_TAG) || (tag == ENEMY_TAG && squadExiter.tag == SQUAD_TAG))
                    {

                        if (enemyController.Contains(squadExiter))
                        {
                            enemyController.Remove(squadExiter);

                            if (enemyController.Count == 0)
                            {
                                SetBattle(false);

                            }
                        }


                        if (enemyController.Count > 0)
                        {
                            if (!escape)
                            {
                                if (!isMoved && inBattle)
                                {

                                    //GoToSquad(enemyController[0].transform);
                                }
                            }
                        }
                        else
                        {

                            if (!isMoved && inBattle)
                            {

                                SetBattle(false);

                            }
                        }


                        //CanSquadFight();


                        
                        //battleRot = false;


                    }
                }
            }
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        if ((tag == SQUAD_TAG && collision.gameObject.tag == ENEMY_TAG) || (tag == ENEMY_TAG && collision.gameObject.tag == SQUAD_TAG))
        {
           

            if (collision.gameObject.layer == 12) // внутренний коллайдер отряда
            {

                isGoingToEnemy = false;

                SquadController squad = collision.transform.gameObject.GetComponent<SquadController>();



                if (!enemyController.Contains(squad))
                {
                    enemyController.Add(squad);
                    CountCoef(enemyController[enemyController.Count - 1].type); // тут вопросы по напвильносит 




                    if (isMoved)
                    {
                        if (canFirstAttack)
                        {
                            FirstAtackDamage();
                        }
                        CancelMovement();


                        // = false;

                    }
                    if (!inBattle)
                    {


                        SetBattle(true);

                    }
                    SelectFightingUnits();





                }
                else
                {

                    if (isMoved)
                    {


                        CancelMovement();
                       // battleMove = false;
                        SetBattle(true);
                    }

                    if (!inBattle)
                    {


                        SetBattle(true);
                    }
                    else
                    {

                        CancelMovement();
                        SetBattle(true);
                    }
                    SelectFightingUnits();
                    squad.SelectFightingUnits();
                }

            }
            else if (collision.gameObject.layer == 16)// пиздим здание
            {
                if (isMoved)
                {
                    //moveDir = enemyCollider.transform.position;
                    predictBuild = collision.gameObject.GetComponent<BuildingManager>();
                    inBuildBattle = true;


                    CancelMovement();
                    //battleMove = true;
                    SetBattle(true);
                    SelectFightingUnits();
                }
            }
        }
     }

    public void Escape(bool esc)
    {
        escape = esc;

        if (esc)
        {
            gameObject.layer = 17;
            escapeTime = 0;
        }
        else
        {
            gameObject.layer = 12;
        }
        
    }

    // fight
    private void SelectFightingUnits() /// можно улучшить - несколько раз искать с каждыйм разом большим радиусом. Искать точное количество (шоб на одного не нападать)
    {
        avaliableToAttack.Clear();

        if (enemyController.Count <= 0 && inBuildBattle == false)
        {
            return;
        }

        if (!inBuildBattle)
        {
            foreach (var eController in enemyController)
            {
                if (eController != null)
                {
                    Collider[] hitColliders = Physics.OverlapSphere(eController.transform.position, TriggerObject.radius, detectionMask);

                    foreach (var hitCollider in hitColliders)
                    {
                        if (squadDie)
                        {
                            return;
                        }
                        if (hitCollider != null && hitCollider.transform.parent == gameObject.transform)
                            avaliableToAttack.Add(hitCollider.gameObject);
                    }
                }

            }
            if (avaliableToAttack.Count == 0) // если нет никого - подходим к врагам
            {
                if (enemyController.Count > 0 && inBattle)
                {
                    GoToSquad(enemyController[0].transform);
                }

            }
        }
        else
        {
            Collider[] hitColliders = Physics.OverlapSphere(predictBuild.transform.position, TriggerObject.radius, detectionMask);

            foreach (var hitCollider in hitColliders)
            {
                if (squadDie)
                {
                    return;
                }
                if (hitCollider != null && hitCollider.transform.parent == gameObject.transform)
                    avaliableToAttack.Add(hitCollider.gameObject);
            }

        }

    }

    private void FirstAtackDamage()
    {
        if (enemyController.Count == 0)
        {
            SetBattle(false);
            return;
            

        }
        SquadController enController = null;

        enController = enemyController[ enemyController.Count - 1];

        if(currentSpeed-2f > enController.currentSpeed && canFirstAttack)
        {
            currentTriggerCoef = AttackTriggerCoef(enController);



            GetUnitDie(-2, 0, currentTriggerCoef*2f, amountUnits - enController.amountUnits);

            Vector3 dir = (enController.transform.position - transform.position).normalized;
            float force = (15f * pushSquad) + ((currentFormation - enController.currentFormation) * 10f) + ((currentSpeed- enController.currentSpeed)* 5f * pushSquad);
            enController.SquadPush(dir, force);
            SquadPush(dir*1.1f, force);

            enController.FormationBonusChange(-(pushSquad/10f) * (currentSpeed - enController.currentSpeed) - currentTriggerCoef);


         enController.canFirstAttack = false;
            enController.isStunned = true;
            enController.stunnedTime = 1.5f;
            
        }

       
        canFirstAttack = false;


        SetBattle(true);
    }

    private void SetEnemyDamage() {


        if (enemyController.Count == 0)
        {
            SetBattle(false);
            return;
            
        }
        if (enemyController.Count == 1 && enemyController[0] == null)
        {
            SetBattle(false);
            return;
        }

        SquadController enController = null;

        enController = enemyController[Random.Range(0, enemyController.Count)];

        currentTriggerCoef = AttackTriggerCoef(enController); 


        if (enController == null)
        {
            return;
        }

        if(avaliableToAttack.Count == 0)
        {
            SelectFightingUnits();
        }


        float min = GameOptions.GetMinDamageValue(enController,this); // уменьшение этого параметра влияет на уменьшение шансов сдохнуть от атаки.


        float max = GameOptions.GetMaxDamageValue(enController, this);

        if (enController == null)
        {
            return;
        }


        if (enController != null) //// можно улучшить 
        {


            float r = Random.Range(min, max);

            if (r > 9f)
            {
               

                enController.GetUnitDie(-1,0, currentTriggerCoef, amountUnits - enController.amountUnits);

                Vector3 dir = (enController.transform.position - transform.position).normalized;
                float force = (20f* pushSquad) + ((FormationEffectIndex() - enController.FormationEffectIndex())*150f);
                enController.SquadPush(dir, force  );
                SquadPush(dir, force );

                MoraleChange(addMoraleKillUnit);

                //.Log("min =" + min + "  max =" + max + " Random = " + r, gameObject);
               // Debug.Log("enController.defenceCoef =" + enController.defenceCoef + "  attackCoef =" + attackCoef + " currentTriggerCoef =" + currentTriggerCoef,gameObject);
            }
            else
            {
                if (r > 5f)
                {
                    //enController.GetDamage(currentAmountUnits - enController.currentAmountUnits);

                    enController.GetDamage(0, currentTriggerCoef, amountUnits - enController.amountUnits); // сюда павер? + currentTriggerCoef+ количество?

                }
            }

            if (enController == null||enemyController.Count ==0)
            {
                return;
            }
            animationState = 2;



            


        }
    }
    private void SetBuildDamage()
    {
        if(predictBuild == null)
        {
            SetBattle(false);
            return;
        }

        if(inBuildBattle){
            predictBuild.GetDamage((amountUnits * powerSquad)* MoraleEffectIndex());
        }
    }

    public void GetDamage(float bonusMoraleLost, float triggerCoef, float countDiff)
    {

        damageCountAnimations++;

        float moraleLost = 0;

        
            moraleLost -= Mathf.Pow(lostMoraleThenAttacked, 0.3f + (currentAmountUnits / amountUnits));
            moraleLost -= (countDiff/100f);
            moraleLost -= triggerCoef /10f;
            moraleLost -= bonusMoraleLost;
            moraleLost -= enemyController.Count / 10f;

     //finalLost -= (moraleLostCoef *0.01f) - (0.75f - (currentAmountUnits / amountUnits)); /// переделать на павер?? /// и количество и павер с коеэфицентами

        Debug.Log("GetDamage moraleLost - " + moraleLost, gameObject);


    
        MoraleChange(moraleLost);

        FormationBonusChange(-lostDefenceMoving- triggerCoef/10f);
        TryToRetreat();
    }

    public void GetUnitDie(int unitIndex, float bonusMoraleLost, float triggerCoef, float countDiff) {// запускается на вражеском отряде, когда надо кого-то убить

       

        int r = 0;
        GameObject currentUnit = null;

        if (unitIndex == -1)// если не задан конкретный юнит, но нам надо звять случайного из первой линии
        {
            if (avaliableToAttack.Count == 0)
            {
                SelectFightingUnits();

                return;
            }

            for (int i = 0; i < avaliableToAttack.Count; i++) // берем случайного
            {
                r = Random.Range(0, avaliableToAttack.Count);

                if (avaliableToAttack[r].transform.parent == gameObject.transform)
                {
                   
                    i = avaliableToAttack.Count;
                    currentUnit = avaliableToAttack[r].gameObject;
                }
                else
                {
                    r = -1;
                }


            }
        }
        else if(unitIndex == -2)// если не задан конкретный юнит, но нам надо звять случайного из всего отряда
        {
            

            for (int i = 0; i < unitArray.Count; i++)
            {
                r = Random.Range(0, unitArray.Count);

                if (unitArray[r].transform.parent == gameObject.transform)
                {
                   
                    i = unitArray.Count;
                    currentUnit = unitArray[r].gameObject;
                }
                else
                {
                    r = -2;
                }


            }
        }

     
        
          

        if (currentUnit == null) { return; }


            DeleteUnit(currentUnit);


        float moraleLost = 0;

        moraleLost -= Mathf.Pow(lostMoraleThenDie, 0.3f + (currentAmountUnits / amountUnits));
        
        moraleLost -= (countDiff / 100f);
        moraleLost -= triggerCoef / 10f;
        moraleLost -= bonusMoraleLost;
        moraleLost -= enemyController.Count / 10f;

        MoraleChange(moraleLost);

            RadiusUpdate();
            TryToRetreat();


    }

    public void DeleteUnit(GameObject unit)
    {
        AnimationController anim = unit.GetComponent<AnimationController>();
        if (unitArray.Contains(unit))
            unitArray.Remove(unit);

        if (avaliableToAttack.Contains(unit))
            avaliableToAttack.Remove(unit);

        if (animatorControllers.Contains(anim))
            animatorControllers.Remove(anim);

        //GameObject currentUnit = unit;
        unit.layer = 0;
        currentAmountUnits--;
        updateFormationIndex--;
        unit.transform.parent = null;
        //unit.SetActive(false);
        anim.SpriteAnimationChange(9);

        


        Instantiate(bloodFx[Random.Range(0, bloodFx.Length)], unit.transform.position, unit.transform.rotation);
        Instantiate(deadFx, unit.transform.position, deadFx.transform.rotation);

        StartCoroutine(DisableUnit(unit, GameOptions.unitsDeadTime));

        OnUnitsCountChange?.Invoke((int)currentAmountUnits);

    }

    IEnumerator DisableUnit(GameObject unit, float delay)
    {
        yield return new WaitForSeconds(delay);

        unit.SetActive(false);
    }




    // morale
    private void TryToRetreat() {
        if (currentAmountUnits < retreatMinCount && currentMorale / maxMorale < 0.35f)
        {

            float min = 0f - (2 * currentMorale) - (2 * (currentAmountUnits / amountUnits));

            float max = 10 + enemyController.Count;

            int count = 1;

            if(currentMorale / maxMorale > 0.3f)
            {
                
            }
            else if(currentMorale / maxMorale > 0.15f)
            {
                count = 2;
            }
            else if (currentMorale / maxMorale > 0.1f)
            {
                count = 3;
            }


            RetreatUpdate();

           // Debug.Log("TryToRetreat min = " + min + "max = " + max);
            if (Random.Range(min, max) > 4f)
            {///// зависит от сложности

                for (int i = 0; i < count; i++)
                {
                    if (unitArray[unitArray.Count - i - 1].transform.parent == gameObject.transform)
                    {
                        Transform currentModel = unitArray[i].transform;
                        currentModel.gameObject.AddComponent<Rigidbody>();
                        currentModel.gameObject.AddComponent<SphereCollider>();

                        UnitRetreatController unitRetreatController = currentModel.GetComponent<UnitRetreatController>();
                        unitRetreatController.enabled = true;

                        if (enemyController.Count > 0)
                        {
                            if (enemyController[0] != null)
                                unitRetreatController.enemyPos = enemyController[0].transform.position;
                        }
                        else
                        {
                            if (inBuildBattle)
                            {
                                unitRetreatController.enemyPos = predictBuild.transform.position;
                            }
                            else
                            {
                                unitRetreatController.enemyPos = new Vector3(0, 0, 0);
                            }
                        }
                        currentModel.parent = null;



                        currentModel.parent = null;



                        if (unitArray.Contains(currentModel.gameObject))
                            unitArray.Remove(currentModel.gameObject);

                        if (avaliableToAttack.Contains(currentModel.gameObject))
                            avaliableToAttack.Remove(currentModel.gameObject);


                        AnimationController anim = currentModel.GetComponent<AnimationController>();

                        if (animatorControllers.Contains(anim))
                            animatorControllers.Remove(anim);

                        currentAmountUnits--;
                        OnUnitsCountChange?.Invoke((int)currentAmountUnits);

                    }
                }

            }



            if (currentAmountUnits < retreatCount || currentAmountUnits == 0)
            {
                // SetBattle(false);
                //squadDie = true;
                inBattle = false;
                battleIndicator.SetActive(false);

                for (int i = 0; i < unitArray.Count; i++)
                {
                    if (unitArray[i].transform.parent == gameObject.transform)
                    {
                        Transform currentModel = unitArray[i].transform;
                        currentModel.gameObject.AddComponent<Rigidbody>();
                        currentModel.gameObject.AddComponent<SphereCollider>();

                        UnitRetreatController unitRetreatController = currentModel.GetComponent<UnitRetreatController>();
                        unitRetreatController.enabled = true;

                        if (enemyController.Count > 0)
                        {
                            if (enemyController[0] != null)
                                unitRetreatController.enemyPos = enemyController[0].transform.position;
                        }
                        else
                        {
                            if (inBuildBattle)
                            {
                                unitRetreatController.enemyPos = predictBuild.transform.position;
                            }
                            else
                            {
                                unitRetreatController.enemyPos = new Vector3(0, 0, 0);
                            }
                        }
                        currentModel.parent = null;
                    }
                }

                SquadDie();
            }
        }

     }
        

    

    public void RetreatUpdate()
    {


        if (currentMorale / maxMorale < 0.35f)
        {
            UnitInfo.SetBool("Retreat", true);
            ai_needHelp = true;

        }
        else
        {
            UnitInfo.SetBool("Retreat", false);
            ai_needHelp = false;
        }

    }

    // radius and formation
    private void RadiusUpdate()
    {
        if (lastAmountUnits - unitArray.Count >= 4)
        {
            
            TriggerObject.radius = (actionColliderRadius + unitPositions.scatterAmount) * ((unitArray.Count / amountUnits)+0.3f);
            if (TriggerObject.radius < 3)
            {
                TriggerObject.radius = 3;
            }

            mainCollider.radius = TriggerObject.radius * 0.4f;
            mainCollider.center = new Vector3(0, TriggerObject.radius * 0.4f, 0) ;

            lastAmountUnits = unitArray.Count;
            

            float scale = 1-( 0.01f * (amountUnits - unitArray.Count));

            unitPositions.transform.localScale = new Vector3(scale, scale, scale);

            float shadowScale =  1f - (currentAmountUnits/ amountUnits);
            shadowScale *= 15f;

            shadowObject.transform.localScale = shadowStartScale - new Vector3(shadowScale, shadowScale, shadowScale);
            selectObject.transform.localScale = new Vector3(selectStartScale - shadowScale, selectStartScale - shadowScale, selectStartScale - shadowScale);

            UnitsTransformInFormation();
            SelectFightingUnits();

        }

    }

    public void UnitsTransformInFormation()
    {
        //if(1f - FormationEffectIndex() <= minimumFormation)
        //{
        //    unitPositions.scatterAmount = minimumFormation;
        //}
        //else
        //{

        //}
       // Debug.Log("UnitsTransformInFormation - " + gameObject.name , gameObject);

        unitPositions.scatterAmount = 1f - currentFormation/10f;


        lastFormationValue = currentFormation;
        unitPositions.RadiusUpdate();

        updateFormation = true;
        currentTimeToUpdateUnit = 0;

        //Transform[] points = unitPositions.points;
        
        //for (int i = 0; i < unitArray.Count; i++)
        //{
        //    unitArray[i].transform.position = points[i].position;
        //    initialPositions[i] = unitArray[i].transform.localPosition;
        //    //initialPositions[i].y += Random.Range(-0.1f, 0.1f);

        //}

        if (unitPositions.scatterAmount > 0.6f)
        {
            if (inBattle)
            {
                if (enemyController.Count > 0)
                {
                    if(enemyController[0] == null) { return; }
                        

                    Transform targetSquad = enemyController[0].transform;

                    for (int i = 0; i < animatorControllers.Count; i++)
                    {
                        SpriteRotate(targetSquad.position, animatorControllers[i].transform);
                    }
                }
            }
        }
    }


   

    private void SquadDie() {

        if (enemyController.Count > 0) {
            for (int i = 0; i < enemyController.Count; i++) {

                enemyController[i].DeleteEnemySquad(this);
              
            }
        }

        if (!squadDie)
        {
            if (playerSquad)
            { // удаляем сквады с ии контролера

                winLoseManager.playerSquadsCount--;
            }
            else
            {
                aIController.DeleteSquadFromQueue(this);
                winLoseManager.enemySquadsCount--;
            }

            aIController.squadsToDelete.Add(this);
            //Destroy(gameObject);
            squadDie = true;
            gameObject.SetActive(false);
        }

        OnSquadDie?.Invoke();
    }

    public void DeleteEnemySquad(SquadController squad)
    {
        if (enemyController.Contains(squad))
        {
            enemyController.Remove(squad);
        }
        CanSquadFight();

        MoraleChange(addMoraleKillSquad);

    }

    public void CanSquadFight() {
        if (enemyController.Count > 0) {
            if (!inBattle && !isMoved && !escape)
            {
                if(enemyController[0] != null)
                GoToSquad(enemyController[0].transform);
            }

        } else if (enemyController.Count == 0)
        {
            SetBattle(false);
            SpriteAnimationChange();

        }
    }







    //animation
    public void SpriteAnimationChange()
    {
        if (squadDie){return; }


        if (animationState == 0) // idle
        {
            for (int i = 0; i < animatorControllers.Count; i++)
            {


                animatorControllers[i].SpriteAnimationChange(0);

            }

            for (int i = 0; i < animatorControllers.Count; i++)
            {


                animatorControllers[i].SpriteReset();

                if (Random.Range(1, 3) % 2 == 0)
                {
                    float offset = MoraleEffectIndex();

                    Vector3 spriteSize = animatorControllers[i].startScale;


                    animatorControllers[i].meshRenderer.transform.localScale = new Vector3(spriteSize.x, spriteSize.y + 0.08f*(1f/offset), spriteSize.z);
                }
                else
                {
                    if (MoraleEffectIndex() <= 0.7f)
                    {
                        if (Random.Range(0, 100) > 100* MoraleEffectIndex())
                        {
                            animatorControllers[i].SpriteAnimationChange(7);
                        }
                    }
                }

            }

            for (int i = 0; i < damageCountAnimations; i++)
            {
                if (animatorControllers.Count == 0 || animatorControllers[0] == null) { return; }

                if (Random.Range(1, 3) % 2 == 0)
                {
                    animatorControllers[Random.Range(0, animatorControllers.Count)].GetComponent<AnimationController>().SpriteAnimationChange(6);

                }
                else
                {
                    animatorControllers[Random.Range(0, animatorControllers.Count)].GetComponent<AnimationController>().SpriteAnimationChange(7);
                }

              
            }

            damageCountAnimations = 0;

        }
        else if (animationState == 1)// run
        {

            if (runAnimIndex == 0)
            {
                runAnimIndex = 1;
            }
            else
            {
                runAnimIndex = 0;
            }

            for (int i = 0; i < animatorControllers.Count; i++)
            {
                animatorControllers[i].SpriteReset();

                if (animatorControllers[i].state == 1)
                {
                  
                    animatorControllers[i].SpriteAnimationChange(2);
                }else

                if (animatorControllers[i].state == 2)
                {

                    animatorControllers[i].SpriteAnimationChange(3);
                }
                else

                if (animatorControllers[i].state == 3)
                {
                  
                    animatorControllers[i].SpriteAnimationChange(4);
                }
                else
                if (animatorControllers[i].state == 4)
                {

                    animatorControllers[i].SpriteAnimationChange(1);
                }

            }
        }
        else if (animationState == 2)// fight
        {
            for (int i = 0; i < animatorControllers.Count; i++)
            {


                animatorControllers[i].SpriteAnimationChange(5);

            }



            float fxCounter = 0;

            for (int i = 0; i < 1; i++)
            {

                if (avaliableToAttack.Count == 0) { return; }

               GameObject randomUnit =  avaliableToAttack[Random.Range(0, avaliableToAttack.Count)];

                if (randomUnit != lastAttakedUnit) {

                    randomUnit.GetComponent<AnimationController>().SpriteAnimationChange(6);

                    if (fxCounter <= 1)
                    {
                        if (Random.Range(1, 100) >= 95)
                        {
                            Instantiate(fightFx[Random.Range(0, fightFx.Length)], randomUnit.transform.position, transform.rotation);
                            fxCounter++;

                            
                        }
                    }

                    lastAttakedUnit = randomUnit;
                }
                else
                {
                    //i++;
                }
   

            }

            for (int i = 0; i < damageCountAnimations; i++)
            {
                if (avaliableToAttack.Count == 0 || avaliableToAttack[0] == null) { return; }
                if (Random.Range(1, 3) % 2 == 0)
                {
                    avaliableToAttack[Random.Range(0, avaliableToAttack.Count)].GetComponent<AnimationController>().SpriteAnimationChange(7);

                }
                else
                {
                    avaliableToAttack[Random.Range(0, avaliableToAttack.Count)].GetComponent<AnimationController>().SpriteAnimationChange(7);
                }
            }

            damageCountAnimations = 0;

        }
        else if (animationState == 3)// Damage
        {
            for (int i = 0; i < animatorControllers.Count; i++)
            {

                if (Random.Range(1, 3) % 2 == 0)
                {
                    animatorControllers[i].SpriteAnimationChange(7);
                }
                else
                {
                    animatorControllers[i].SpriteAnimationChange(8);
                }

            }
        }
    }

    void MoveAnimation()
    {


         moveAnimtime += Time.deltaTime * (4f + (4f * currentSpeed/movementSpeed));

        for (int i = 0; i < animatorControllers.Count; i++)
        {
            // Рассчитываем индексы в сетке
            int x = i % 5;
            int z = i / 5;

            // Вычисляем смещение волны
            float waveOffset = Mathf.Sin((x + z) * 10f + moveAnimtime) / (15f - currentSpeed);

            // Обновляем позицию юнита
            Vector3 targetPosition = animatorControllers[i].startPos;
            targetPosition.y += waveOffset;
            animatorControllers[i].SpriteTransform(targetPosition);

        }


    }


    void FigthAnimation()
    {


        float time = Time.time * (3f + (4f * MoraleEffectIndex()));

        for (int i = 0; i < animatorControllers.Count; i++)
        {
            // Рассчитываем индексы в сетке
            int x = i % 5;
            int z = i / 5;

            // Вычисляем смещение волны
            float waveOffset = (Mathf.Sin((x + z) * 1f + time))/4f ;

            // Обновляем позицию юнита
            Vector3 targetPosition = animatorControllers[i].meshRenderer.transform.localPosition;

            Vector3 dir = directionToEnemy;

            if (enemyController.Count > 0)
            {
                dir = (directionToEnemy - (targetPosition + transform.position)).normalized;

                
            }
            else
            {
                if (predictBuild == null)
                {
                    return;
                } ////////////////// можно улучшить!!

                dir = (directionToEnemy - (targetPosition + transform.position)).normalized;
              
            }



            if (avaliableToAttack.Contains(animatorControllers[i].transform.gameObject))
            {

                targetPosition += dir * waveOffset;
                animatorControllers[i].SpriteTransform(targetPosition);
            }
            else
            {
                targetPosition += dir * (waveOffset/3f);
                animatorControllers[i].SpriteTransform(targetPosition);
            }

        }
    }


    public void SquadPush(Vector3 dir, float force)// отталкивание
    {

        rb.AddForce(dir * force );
        //animationState = 3;
        //SpriteAnimationChange();
    }

    //AI
    public float AiPowerCalculate()
    {
        ai_squadPower = currentAmountUnits * powerSquad;

        return ai_squadPower;
    }

    //fx
    public void SpawnFriendlyFireFX()
    {
        Instantiate(friendlyFireFx, unitArray[Random.Range(0, unitArray.Count)].transform.position, friendlyFireFx.transform.rotation);

    }

    void ActivateNextParticle()
    {
        foreach (ParticleSystem ps in moveFXlist)
        {
            if (!ps.isPlaying)
            {
                // Выбираем случайного юнита
                int randomIndex = Random.Range(0, unitArray.Count);
                Transform selectedUnit = unitArray[randomIndex].transform;

                // Перемещаем партикл на юнита и включаем его
                ps.transform.position = selectedUnit.position;
                ps.Play();

                return; // Включаем только один партикл за раз
            }
        }
    }


}


