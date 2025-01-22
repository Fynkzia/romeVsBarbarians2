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
    [SerializeField] public int actionUnits;
    [Space(10)]
    [SerializeField] public float actionColliderRadius;

    [Space(10)]
    [Header("Movement specs")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float boostSpeed;


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

    [SerializeField] public bool attack;
    [SerializeField] public bool defence;

    [SerializeField] private bool battleRot = false;
    [SerializeField] private bool battleMove = false;
    [SerializeField] private bool moveRot = false;
    [SerializeField] public bool escape = false;
    [SerializeField] private bool squadDie = false;
    [SerializeField] public bool canFirstAttack = true;
    [SerializeField] public bool isStunned = false;



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
    private float movingDistanceToPoint;
    private Vector3[] movingPositions;
    private int indexMove = 0;


    [Header("Fighting Settings")]
    [Space(10)]

    [SerializeField] private float maxEscapeTime;
    [SerializeField] public float attackCoef;
    [SerializeField] public float defenceCoef;



    [Header("Detect Units Settings")] // Селект файтинг юнитс - узнаем где какие войны в бою
    [Space(10)]
    [SerializeField] public List<GameObject> avaliableToAttack;
    [SerializeField] public LayerMask detectionMask;
    private float lastFormationValue = 0;
    private int lastAmountUnits = 0;


    [Header("Animation Settings")]
    [Space(10)]
    [SerializeField] public int animationState;
    [HideInInspector] public List<AnimationController> animatorControllers = new List<AnimationController>();
    private int runAnimIndex = 0;


    [SerializeField] public float animationIdleUpdateTime;
    [SerializeField] public float animationActionUpdateTime;

    public Vector3 spriteSize;
    private Vector3[] initialPositions;
    private int damageCountAnimations = 0;

    [Header("FX Settings")]
    [Space(10)]
    [SerializeField] private ParticleSystem[] bloodFx;
    [SerializeField] private GameObject[] fightFx;
    [SerializeField] private GameObject deadFx;
    [SerializeField] private GameObject addMoraleFx;
    [SerializeField] private GameObject lostMoraleFx;
    [SerializeField] private GameObject friendlyFireFx;
    [Space(10)]

    [Header("Enemy AI settings")]
    [Space(10)]
    [SerializeField] public int aiActionValue = 100;
    [SerializeField] public bool dangerAlert = false;


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
    private EnemyAIController aIController;
    private WinLoseManager winLoseManager;

    
    [Space(10)]
    [SerializeField] private GameObject unitPrefab;
    [Space(10)]

    [SerializeField] public SphereCollider colliderObject;
    [SerializeField] public Collider predictEnemy;
    private Rigidbody rb;
    [SerializeField] public List<GameObject> unitArray;
    [SerializeField] public List<SquadController> enemyController = new List<SquadController>();

    [SerializeField] public LineRenderer lineRenderer;


    [SerializeField] private Animator UnitInfo;
    [SerializeField] private GameObject drawingPrefab;
    [SerializeField] private GameObject movementIndicator;
    [SerializeField] private GameObject battleIndicator;
    [SerializeField] private PointsController unitPositions;

    [SerializeField] private GameObject shadowObject;
    [SerializeField] private GameObject selectObject;

    private float shadowStartScale = 0;
    private float selectStartScale = 0;

    [HideInInspector] public int tapCount = 0;

    private const string ENEMY_TAG = "Enemy";
    private const string SQUAD_TAG = "Squad";
    private const string ENEMY_TRIGGER_TAG = "EnemyTrigger";
    private const string SQUAD_TRIGGER_TAG = "SquadTrigger";


    private void Start() {

        coinsController = GameObject.Find("CoinsController").GetComponent<CoinsController>();
        controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();
        aIController = GameObject.Find("AIManager").GetComponent<EnemyAIController>();
        winLoseManager = GameObject.Find("WinLoseManager").GetComponent<WinLoseManager>();
        rb = GetComponent<Rigidbody>();

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
        colliderObject.radius = actionColliderRadius;

        shadowStartScale = shadowObject.transform.localScale.x;
        selectStartScale = selectObject.transform.localScale.x;

        //анимации
        spriteSize = animatorControllers[0].transform.localScale;


        animationState = 0;
        SpriteAnimationChange();

        //формация


       


        initialPositions = new Vector3[animatorControllers.Count];

        for (int i = 0; i < animatorControllers.Count; i++)
        {
            initialPositions[i] = animatorControllers[i].transform.localPosition;
        }

        RadiusUpdate();
        UnitsTransformInFormation();

    }

    private void FixedUpdate() {

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
            if (animTime > animationActionUpdateTime)
            {
                SpriteAnimationChange();
                animTime = 0;
            }

            if (isMoved)
            {
                MoveAnimation();
            }

            if (inBattle)
            {
                FigthAnimation();
            }
        }


        if (!isStunned)
        {

            if (isMoved || battleMove)
            {
                SquadMovement();


                FormationBonusChange(-lostDefenceMoving * Time.deltaTime);


                if (tapCount > 0)
                {
                    startTapTime += Time.fixedDeltaTime;

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

                if (currentActionTime >= actionTime / ((actionUnits / currentAmountUnits) * currentAmountUnits)) // вписываем экшн юнитс
                {
                    currentActionTime = 0f;
                    SetEnemyDamage();


                }

            }
        }
        else
        {
            stunnedTime -= Time.fixedDeltaTime;

           if(stunnedTime <= 0) {
                stunnedTime = 0;
                isStunned = false;
            }
        }


        restorTime += Time.fixedDeltaTime;
        if (restorTime > 1f)
        {
            if (!inBattle &&!isMoved && currentMorale < maxMorale)
            {
              
                MoraleChange(recoveryMoraleSpeed * (currentAmountUnits / amountUnits));

            }

            

            if (!inBattle && !isMoved && currentFormation <= formationSquad)
            {
                FormationBonusChange(resetDefence * (currentMorale / maxMorale));

            }

            restorTime = 0;
        }

        if (escape) {
            if (escapeTime < maxEscapeTime) {
                escapeTime += Time.fixedDeltaTime;
            }
            else {
                escape = false;
                escapeTime = 0;
            }
        }

    }




    private void SquadMovement() {

        if (!battleMove)
        {
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


                if (angleDifference > angleToUpdateFormation)
                {
                    moveRot = true;
                    
                }
                if (moveRot)
                {
                    unitPositions.transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
                    moveRot = false;
                    UnitsTransformInFormation();
                    squadRotate = unitPositions.transform.rotation;
                    
                }



                if (!isStopRot)
                {

                    float directionz = targetPos.z - transform.position.z;
                    float directionx = targetPos.x - transform.position.x;

                    Vector2 lineVec = new Vector2(-1,1) - new Vector2(1, -1);

                    float crossProduct = lineVec.x * directionz - lineVec.y * directionx;

                     if(crossProduct < 0) {

                        for (int i = 0; i < animatorControllers.Count; i++)
                        {
                            Transform mesh = animatorControllers[i].transform.GetChild(0).transform;
                            if (mesh.transform.localScale.x > 0)
                                mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
                        }

                    }
                    else {

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
                    movingPositions = pointsList.ToArray();
                    lineRenderer.SetPositions(movingPositions);
                    indexMove++;
                }

                MoraleChange(-(lostMoraleThenRun *Time.deltaTime));

            }
            else
            {
                CancelMovement();


                if (!playerSquad)
                {
                    AiActionValueReset();
                }
            }
        }
        else
        {
            Vector3 targetPos = Vector3.MoveTowards(transform.position, moveDir, CurrentSpeed() * Time.fixedDeltaTime);

            if (Vector3.Distance(rb.position, predictEnemy.transform.position) > (colliderObject.radius * 1.1f))
            {
                rb.MovePosition(targetPos);

            }
            else
            {
                

                battleMove = false;
                
            }
        }
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
                    animatorControllers[i].transform.localScale = new Vector3(spriteSize.x, spriteSize.y, spriteSize.z);

                    if (Random.Range(1, 3) % 2 == 0)
                    {
                        animatorControllers[i].SpriteAnimationChange(2);
                    }
                    else
                    {
                        animatorControllers[i].SpriteAnimationChange(1);
                    }
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
            squadRotate = unitPositions.transform.rotation;
            animTime = animationIdleUpdateTime;
        }

    }

    public void SetBattle(bool inBattle) {

        if (inBattle)
        {
            animationState = 2;
            
        }
        else
        {
            if (isMoved)
            {
                animationState = 1;

                for (int i = 0; i < animatorControllers.Count; i++)
                {
                    animatorControllers[i].transform.localScale = new Vector3(spriteSize.x, spriteSize.y, spriteSize.z);

                    if (Random.Range(1, 3) % 2 == 0)
                    {
                        animatorControllers[i].SpriteAnimationChange(2);
                    }
                    else
                    {
                        animatorControllers[i].SpriteAnimationChange(1);
                    }
                }
            }
            else
            {
                animationState = 0;
               
            }
            canFirstAttack = true;
        }

        

        this.inBattle = inBattle;
        battleIndicator.SetActive(inBattle);
        


        if (!playerSquad) {
            AiActionValueReset();
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

    private float AttackTriggerCoef(SquadController en)
    {
        
        Quaternion rotationDifference = Quaternion.Inverse(en.unitPositions.transform.rotation) * unitPositions.transform.rotation;

        // Преобразование разницы вращений в угол
        float angleY = Mathf.Abs(rotationDifference.eulerAngles.y);

       

        // Приведение угла в диапазон [0, 180]
        if (angleY > 180.0f)
        {
            angleY = 360.0f - angleY;
        }

        
        // Условия сравнения
        if (angleY <= 5.0f)
        {
            Debug.Log("ебашим во спину - angleY =" + angleY);
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
        float currentBoostSpeed = boostSpeed * MoraleToSpeedEffectIndex();
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

        return 0.3f;
    }

    private float ActionUnitsIndex()
    {

        return ((actionUnits / currentAmountUnits));
    }



    public void MoraleChange(float morale) {
        if(currentMorale + morale > 0.1 && currentMorale + morale < maxMorale)
        {
            currentMorale += morale;

            Debug.Log("MoraleChange - morale=" + morale, gameObject);
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
        currentSpeed = 0;
        movingPositions = null;
        tapCount = 0;
        startTapTime = 0;
        isGoingToEnemy = false;
       
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
      
        SetMoving(true);
    }

    public void SpriteRotate(Vector3 target, Transform unit)
    {
        float directionz = target.z - unit.position.z;
        float directionx = target.x - unit.position.x;

        Transform mesh = unit.transform.GetChild(0).transform;

        if (directionx < 0 && directionz <= 0)
        {

            if (mesh.transform.localScale.x < 0)
                mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);

        }
        else if (directionx > 0 && directionz >= 0)
        {

            if (mesh.transform.localScale.x > 0)
                mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);

        }

        if (directionx < 0 && directionz >= 0)
        {

            if (mesh.transform.localScale.x > 0)
                mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);

        }
        else if (directionx > 0 && directionz <= 0)
        {

            if (mesh.transform.localScale.x > 0)
                mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);

        }

    }


    // triggers
    public void OnMainTriggerEnter(Collider enemyCollider) { // тригеры тоже работают - все переводи на тригеры - колайдеры не нужны
        if (!escape) {

            if ((tag == SQUAD_TAG && enemyCollider.gameObject.tag == ENEMY_TAG) || (tag == ENEMY_TAG && enemyCollider.gameObject.tag == SQUAD_TAG))
            {


                if (enemyCollider.gameObject.layer == 12) // внутренний коллайдер отряда
                {
                    
                    isGoingToEnemy = false;

                    SquadController squad = enemyCollider.transform.gameObject.GetComponent<SquadController>();



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

                            
                            battleMove = true;
                            
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
                            battleMove = true;
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

                    if (squadExiter.escape) {

                        if (enemyController.Count == 0)
                        {
                            
                            SetBattle(false);
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
                            if (!inBattle)
                            {
                               
                                GoToSquad(enemyController[0].transform);
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

    // fight
    private void SelectFightingUnits() /// можно улучшить - несколько раз искать с каждыйм разом большим радиусом. Искать точное количество (шоб на одного не нападать)
    {
        avaliableToAttack.Clear();

        if (enemyController.Count <= 0)
        {
            return;
        }

        foreach (var eController in enemyController)
        {
            if (eController != null)
            {
                Collider[] hitColliders = Physics.OverlapSphere(eController.transform.position, colliderObject.radius, detectionMask);

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
            if (enemyController.Count > 0)
            {
                GoToSquad(enemyController[0].transform);
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

        if(currentSpeed-1f > enController.currentSpeed && canFirstAttack)
        {
            currentTriggerCoef = AttackTriggerCoef(enController);

           

            enController.GetUnitDie(currentTriggerCoef, -2);

            Vector3 dir = (enController.transform.position - transform.position).normalized;
            float force = (15f * pushSquad) + ((currentFormation - enController.currentFormation) * 10f) + ((currentSpeed- enController.currentSpeed)* 5f * pushSquad);
            enController.SquadPush(dir, force);
            SquadPush(dir*1.1f, force);

            enController.FormationBonusChange(-(pushSquad/10f) * (currentSpeed - enController.currentSpeed));


         enController.canFirstAttack = false;
            enController.isStunned = true;
            enController.stunnedTime = 1f;
            
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



        float min = GameOptions.GetMinDamageValue(enController,this); // уменьшение этого параметра влияет на уменьшение шансов сдохнуть от атаки.

        if (enController.defence) {
            min -= (enController.actionUnits / 25f) * 3f; //приказ на защиту
        }

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
               

                enController.GetUnitDie(currentTriggerCoef, -1);

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
                    enController.GetDamage(currentAmountUnits - enController.currentAmountUnits);
                   
                }
            }

            if (enController == null||enemyController.Count ==0)
            {
                return;
            }
            animationState = 2;



            


        }
    }

    public void GetDamage(float moraleLostCoef)
    {

        damageCountAnimations++;

        if (moraleLostCoef == 0)
        {
            MoraleChange(-lostMoraleThenAttacked - (0.75f - (currentAmountUnits / amountUnits)));
        }
        else
        {
            float finalLost = -lostMoraleThenAttacked - (0.75f - (currentAmountUnits/ amountUnits));
            finalLost -= (moraleLostCoef *0.01f) - (0.75f - (currentAmountUnits / amountUnits));

            MoraleChange(finalLost );
            

        }

        FormationBonusChange(-lostDefenceMoving);

    }

    public void GetUnitDie(float triggerCoef, int unitIndex) {// запускается на вражеском отряде, когда надо кого-то убить

        Debug.Log("GetUnitDie",gameObject);

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
            
           


            MoraleChange(-lostMoraleThenDie - (0.75f - (currentAmountUnits / amountUnits)));

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
        unit.transform.parent = null;
        //unit.SetActive(false);
        anim.SpriteAnimationChange(8);



        Instantiate(bloodFx[Random.Range(0, bloodFx.Length)], unit.transform.position, unit.transform.rotation);
        Instantiate(deadFx, unit.transform.position, deadFx.transform.rotation);

        StartCoroutine(DisableUnit(unit, GameOptions.unitsDeadTime));

    }

    IEnumerator DisableUnit(GameObject unit, float delay)
    {
        yield return new WaitForSeconds(delay);

        unit.SetActive(false);
    }




    // morale
    private void TryToRetreat() {
        if (currentAmountUnits < retreatMinCount && currentMorale / maxMorale < 0.3f) {
            float min = 0f - currentMorale - currentAmountUnits;

            float max = 10 + enemyController.Count;

            RetreatUpdate();


            if (Random.Range(min, max) > 10f) { ///// зависит от сложности
                SetBattle(false);
                squadDie = true;

                for (int i = 0; i < unitArray.Count; i++) {
                    if (unitArray[i].transform.parent == gameObject.transform)
                    {
                        Transform currentModel = unitArray[i].transform;
                        currentModel.gameObject.AddComponent<Rigidbody>();
                        currentModel.gameObject.AddComponent<SphereCollider>();
                        currentModel.GetComponent<UnitRetreatController>().enabled = true;
                        currentModel.parent = null;
                    }
                }
                SquadDie();
                return;
            }

            if (currentAmountUnits < retreatCount || currentAmountUnits == 0) {
                SetBattle(false);
                squadDie = true;

                for (int i = 0; i < unitArray.Count; i++)
                {
                    if (unitArray[i].transform.parent == gameObject.transform)
                    {
                        Transform currentModel = unitArray[i].transform;
                        currentModel.gameObject.AddComponent<Rigidbody>();
                        currentModel.gameObject.AddComponent<SphereCollider>();
                        currentModel.GetComponent<UnitRetreatController>().enabled = true;
                        currentModel.parent = null;
                    }
                }
                SquadDie();
            }

        }
        else
        {
            RetreatUpdate();
        }

    }

    public void RetreatUpdate()
    {


        if (currentMorale / maxMorale < 0.3f)
        {
            UnitInfo.SetBool("Retreat", true);

        }
        else
        {
            UnitInfo.SetBool("Retreat", false);
        }

    }

    // radius and formation
    private void RadiusUpdate()
    {
        if (lastAmountUnits - unitArray.Count >= 4)
        {
            
            colliderObject.radius = (actionColliderRadius + unitPositions.scatterAmount) * ((unitArray.Count / amountUnits)+0.2f);
            if (colliderObject.radius < 3)
            {
                colliderObject.radius = 3;
            }

                lastAmountUnits = unitArray.Count;
            

            float scale = 1-( 0.01f * (amountUnits - unitArray.Count));

            unitPositions.transform.localScale = new Vector3(scale, scale, scale);

            float shadowScale = amountUnits/unitArray.Count;

            shadowObject.transform.localScale = new Vector3(shadowStartScale - shadowScale, shadowStartScale - shadowScale, shadowStartScale - shadowScale);
            selectObject.transform.localScale = new Vector3(selectStartScale - shadowScale, selectStartScale - shadowScale, shadowStartScale - shadowScale);

            UnitsTransformInFormation();
            SelectFightingUnits();

        }

    }

    private void UnitsTransformInFormation()
    {
        //if(1f - FormationEffectIndex() <= minimumFormation)
        //{
        //    unitPositions.scatterAmount = minimumFormation;
        //}
        //else
        //{

        //}

        unitPositions.scatterAmount = 1f - currentFormation/10f;


        lastFormationValue = currentFormation;
        unitPositions.RadiusUpdate();

        Transform[] points = unitPositions.points;
        
        for (int i = 0; i < unitArray.Count; i++)
        {
            unitArray[i].transform.position = points[i].position;
            initialPositions[i] = unitArray[i].transform.localPosition;

        }
    }


   

    private void SquadDie() {

        if (enemyController.Count > 0) {
            for (int i = 0; i < enemyController.Count; i++) {

                enemyController[i].DeleteEnemySquad(this);
                enemyController[i].CanSquadFight(); 
            }
        }

        if (playerSquad) { // удаляем сквады с ии контролера
            aIController.DeletePlayerSquad(this);
            winLoseManager.playerSquadsCount--;
        }
        else {
            aIController.DeleteEnemySquad(this);
            winLoseManager.enemySquadsCount--;
        }

        Destroy(gameObject);
    }

    public void DeleteEnemySquad(SquadController squad)
    {
        enemyController.Remove(squad);
        CanSquadFight();

        MoraleChange(addMoraleKillSquad);

    }

    public void CanSquadFight() {
        if (enemyController.Count > 0) {
            GoToSquad(enemyController[0].transform);

        } else if (enemyController.Count == 0)
        {
            SetBattle(false);
            SpriteAnimationChange();

        }
    }







    //animation
    public void SpriteAnimationChange()
    {
        if (animationState == 0) // idle
        {
            for (int i = 0; i < animatorControllers.Count; i++)
            {


                animatorControllers[i].SpriteAnimationChange(0);

            }

            for (int i = 0; i < animatorControllers.Count; i++)
            {


                animatorControllers[i].transform.localScale = new Vector3(spriteSize.x, spriteSize.y, spriteSize.z);

                if (Random.Range(1, 3) % 2 == 0)
                {
                    float offset = MoraleEffectIndex();

                    
                    animatorControllers[i].transform.localScale = new Vector3(spriteSize.x, spriteSize.y + 0.08f*(1f/offset), spriteSize.z);
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
                animatorControllers[i].transform.localScale = new Vector3(spriteSize.x, spriteSize.y, spriteSize.z);

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
                  
                    animatorControllers[i].SpriteAnimationChange(1);
                }

            }
        }
        else if (animationState == 2)// fight
        {
            for (int i = 0; i < animatorControllers.Count; i++)
            {


                animatorControllers[i].SpriteAnimationChange(4);

            }



            float fxCounter = 0;

            for (int i = 0; i < avaliableToAttack.Count; i++)
            {
                if (Random.Range(1, 3) % 2 == 0)
                {
                    if (avaliableToAttack.Count == 0 || avaliableToAttack[i] == null) { return; }
                    avaliableToAttack[i].GetComponent<AnimationController>().SpriteAnimationChange(5);


                    SpriteRotate(enemyController[0].transform.position, avaliableToAttack[i].transform);



                    if (fxCounter <= 1)
                    {
                        if (Random.Range(1, 100) >= 95)
                        {
                            Instantiate(fightFx[Random.Range(0, fightFx.Length)], avaliableToAttack[i].transform.position, transform.rotation);
                            fxCounter++;
                        }
                    }
                }
                else
                {
                    if (avaliableToAttack.Count == 0 || avaliableToAttack[i] == null) { return; }
                    avaliableToAttack[i].GetComponent<AnimationController>().SpriteAnimationChange(4);
                }

            }

            for (int i = 0; i < damageCountAnimations; i++)
            {
                if (avaliableToAttack.Count == 0 || avaliableToAttack[0] == null) { return; }
                if (Random.Range(1, 3) % 2 == 0)
                {
                    avaliableToAttack[Random.Range(0, avaliableToAttack.Count)].GetComponent<AnimationController>().SpriteAnimationChange(6);

                }
                else
                {
                    avaliableToAttack[Random.Range(0, avaliableToAttack.Count)].GetComponent<AnimationController>().SpriteAnimationChange(6);
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
                    animatorControllers[i].SpriteAnimationChange(6);
                }
                else
                {
                    animatorControllers[i].SpriteAnimationChange(7);
                }

            }
        }
    }

    void MoveAnimation()
    {


        float time = Time.time * 10 * MoraleEffectIndex() ;

        for (int i = 0; i < animatorControllers.Count; i++)
        {
            // Рассчитываем индексы в сетке
            int x = i % 5;
            int z = i / 5;

            // Вычисляем смещение волны
            float waveOffset = Mathf.Sin((x + z) * 10f + time) * 0.07f* MoraleEffectIndex();

            // Обновляем позицию юнита
            Vector3 targetPosition = initialPositions[i];
            targetPosition.y += waveOffset;
            animatorControllers[i].transform.localPosition = targetPosition;

        }
    }

    void FigthAnimation()
    {


        float time = Time.time + (10f * ActionUnitsIndex());

        for (int i = 0; i < animatorControllers.Count; i++)
        {
            // Рассчитываем индексы в сетке
            int x = i % 5;
            int z = i / 5;

            // Вычисляем смещение волны
            float waveOffset = Mathf.Sin((x + z) * 10f + time) * 1.8f;

            // Обновляем позицию юнита
            Vector3 targetPosition = initialPositions[i];
            if (avaliableToAttack.Contains(animatorControllers[i].transform.gameObject))
            {

                targetPosition.x += waveOffset;
                animatorControllers[i].transform.localPosition = targetPosition;
            }
            else
            {
                targetPosition.x += waveOffset / 5;
                animatorControllers[i].transform.localPosition = targetPosition;
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
    public void AiActionValueReset()
    {
        aiActionValue = 100;
        dangerAlert = false;
    }

    //fx
    public void SpawnFriendlyFireFX()
    {
        Instantiate(friendlyFireFx, unitArray[Random.Range(0, unitArray.Count)].transform.position, friendlyFireFx.transform.rotation);

    }
}


