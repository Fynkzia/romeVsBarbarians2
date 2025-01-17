using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;
using System.Linq;

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

    [Header("Balance specs")]
    [Space(10)]
    [SerializeField] public float currentAmountUnits;
    [Space(10)]

    [SerializeField] private float levelSquad;
    [Space(10)]

    [SerializeField] public float amountUnits;
    [SerializeField] public float powerSquad;
    [SerializeField] public float defenceSquad;
    [SerializeField] public float maxMorale;
   
    [SerializeField] public float actionTime;
    [SerializeField] public int actionUnits;
    [SerializeField] public float colliderRadius;

    [Space(10)]
    [Header("Movement specs")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float boostSpeed;


    [Header("Defence/Formation specs")]
    [Space(10)]
    [SerializeField] public float minimumFormation;
    [SerializeField] public float lostDefenceMoving;
    [SerializeField] public float resetDefence;

    [Space(10)]
    [Header("Morale specs")]

    [SerializeField] public float lostMoraleThenRun;
    [SerializeField] public float lostMoraleThenDie;
    [SerializeField] public float lostMoraleThenAttack;
    [SerializeField] public float recoveryMoraleSpeed;

    [SerializeField] public float aroundRadius;
    [SerializeField] public LayerMask aroundMask;

    [SerializeField] public float aroundSquadsBonus;
    [SerializeField] public float aroundRetreatBonus;

    [Space(10)]
    [Header("Retreat specs")]

    [SerializeField] private float retreatCount;
    [SerializeField] private float retreatChanceCount;

    [Space(10)]
    [Header("Attck coef specs")]
    [SerializeField] public Coef[] coef;
    [SerializeField] private float leftTriggerCoef;
    [SerializeField] private float rightTriggerCoef;
    [SerializeField] private float frontTriggerCoef;
    [SerializeField] private float backTriggerCoef;
    [SerializeField] private float offsetRadius = 0.2f;

    [Header("Economy specs")]

    [SerializeField] private int coinsFromDeath;

    [Space(20)]
    [Header("Main")]
    [Space(10)]
    [SerializeField] public SquadType type;

    [SerializeField] private bool playerSquad;

    [Space(10)]

    [SerializeField] public bool isMoved = false;
    [SerializeField] public bool isStopRot = false;
    [SerializeField] public bool inBattle = false;
    [SerializeField] public bool isGoingToEnemy = false;

    [SerializeField] public bool attack;
    [SerializeField] public bool defence;
    [Space(10)]

    [SerializeField] public SphereCollider colliderObject;
    [SerializeField] public Collider predictEnemy;


    [Space(10)]
    [Header("Current Spec")]
    [SerializeField] public float currentMorale;
    [SerializeField] public float currentDefence;
    [SerializeField] public float currentSpeed;



    [Space(10)]
    private Rigidbody rb;
    [SerializeField] public List<GameObject> unitArray;
    [SerializeField] public List<SquadController> enemyController = new List<SquadController>();

    [SerializeField] public LineRenderer lineRenderer;



    [Header("Movement Settings")]
    [Space(10)]
    private float pointsDistance;
    [SerializeField] private float maxDeltaAngel;

    [SerializeField] private float deltaAngels;
    private Vector3[] positions;



    [Header("Fighting Settings")]
    [Space(10)]

    [SerializeField] private float maxEscapeTime;
    [SerializeField] private float attackTriggerCoef;//???
    [SerializeField] public float attackCoef;
    [SerializeField] public float defenceCoef;



    [Header("Detect Units Settings")]
    [Space(10)]
    [SerializeField] public float radiusDetection = 1f;
    [SerializeField] public List<GameObject> avaliableToAttack;
    [SerializeField] public LayerMask detrctionMask;
    [SerializeField] public bool ignoreTriggers;


    [Header("Animation Settings")]
    [Space(10)]

    [SerializeField] public int animationState;


    [HideInInspector] public List<AnimationController> animatorControllers = new List<AnimationController>();
    [SerializeField] public Vector3 spriteSize;

    [SerializeField] public float animationIdleUpdateTime;
    [SerializeField] public float animationActionUpdateTime;

    private Vector3[] initialPositions;
    private int damageCountAnimations = 0;

    [Header("FX Settings")]
    [Space(10)]
    [SerializeField] private ParticleSystem[] bloodFx;
    [SerializeField] private GameObject[] fightFx;
    [SerializeField] private GameObject deadFx;
    [Space(10)]

    [Header("Enemy AI settings")]
    [Space(10)]
    [SerializeField] public int aiActionValue = 100;
    [SerializeField] public bool dangerAlert = false;

    [Space(10)]


    private int indexMove = 0;
   
    private float battleTime = 0f;
    private float restorTime = 0f;

    [SerializeField] private bool battleRot = false;
    [SerializeField] private bool battleMove = false;
    [SerializeField] private bool moveRot = false;
    [SerializeField] public bool escape = false;
    [SerializeField] private bool squadDie = false;
    [SerializeField] private float escapeTime = 0f;
    private float currentTriggerCoef;
    private float aroundBonus;
    private Quaternion rotRotate;
    private Vector3 moveDir;
    


    private CoinsController coinsController;
    private SquadControlManager controlController;
    private EnemyAIController aIController;
    private WinLoseManager winLoseManager;


    [SerializeField] private Animator UnitInfo;
    [SerializeField] private GameObject drawingPrefab;
    [SerializeField] private GameObject movementIndicator;
    [SerializeField] private GameObject battleIndicator;
    [SerializeField] private PointsController unitPositions;

    [SerializeField] private GameObject shadowObject;
    [SerializeField] private GameObject selectObject;

    public float shadowStartScale = 0;
    public float selectStartScale = 0;

    public float angleToUpdateFormation = 0;
    public int tapCount = 0;
    private float startTapTime = 0;
    private float maxTimeWait = 0.5f;

    public float animTime = 0;
    private int runAnimIndex = 0;
    private float lastDefValue = 0;
    private int lastAmountUnits = 0;


    private const string ENEMY_TAG = "Enemy";
    private const string SQUAD_TAG = "Squad";
    private const string ENEMY_TRIGGER_TAG = "EnemyTrigger";
    private const string SQUAD_TRIGGER_TAG = "SquadTrigger";

    private void Start() {
        for (int i = 0; i < unitArray.Count; i++)
        {

            animatorControllers.Add(unitArray[i].GetComponent<AnimationController>());

        }
        spriteSize = animatorControllers[0].transform.localScale;
        colliderObject.radius = colliderRadius;

        
        currentMorale = maxMorale;
        battleTime = actionTime;

        currentDefence = defenceSquad;
        lastAmountUnits = unitArray.Count;

        coinsController = GameObject.Find("CoinsController").GetComponent<CoinsController>();
        controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();
        aIController = GameObject.Find("AIManager").GetComponent<EnemyAIController>();
        winLoseManager = GameObject.Find("WinLoseManager").GetComponent<WinLoseManager>();

        deltaAngels = transform.rotation.eulerAngles.y;
        currentAmountUnits = unitArray.Count;

        pointsDistance = GameOptions.distanceToPoint;
        rb = GetComponent<Rigidbody>();

        shadowStartScale = shadowObject.transform.localScale.x;
        selectStartScale = selectObject.transform.localScale.x;

        animationState = 0;
        SpriteAnimationChange();

        initialPositions = new Vector3[animatorControllers.Count];
        for (int i = 0; i < animatorControllers.Count; i++)
        {
            initialPositions[i] = animatorControllers[i].transform.localPosition;
        }

        RadiusUpdate();
        UnitsTransform();
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




        if (isMoved || battleMove) {
            SquadMovement();
           
            
                DefenceChange(-lostDefenceMoving * Time.deltaTime);
            

            if (tapCount > 0) {
                startTapTime += Time.fixedDeltaTime;

                if (startTapTime > maxTimeWait) {
                    tapCount = 0;
                    startTapTime = 0;
                }

                if (tapCount == 2) {
                    CancelMovement();
                }
            }
        }


        if (inBattle) {
            battleTime += Time.fixedDeltaTime;
            // animationAttackTime += Time.fixedDeltaTime + (Random.Range(-0.1f, 0.1f) * Time.fixedDeltaTime);

            if (battleTime >= actionTime)
            {
                battleTime = 0f;
                EnemyDamage();


            }

        }


        restorTime += Time.fixedDeltaTime;
        if (restorTime > 1f)
        {
            if (!inBattle &&!isMoved && currentMorale < maxMorale)
            {
              
                MoraleChange(recoveryMoraleSpeed * (currentAmountUnits / amountUnits) + aroundBonus);

            }

            

            if (!inBattle && !isMoved && currentDefence <= defenceSquad)
            {
                DefenceChange(resetDefence * (currentMorale / maxMorale));


               

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
                SetRotation(false);
                return;
            }


            if (indexMove < lineRenderer.positionCount - 1)
            {
                positions = new Vector3[(int)lineRenderer.positionCount];
                lineRenderer.GetPositions(positions);

                Vector3 linePosition = new Vector3(lineRenderer.GetPosition(1).x, transform.position.y, lineRenderer.GetPosition(1).z);
                Vector3 targetPos = Vector3.MoveTowards(transform.position, linePosition, CurrentSpeed() * Time.fixedDeltaTime);

                float y = MathUtilities.AngleBetweenTwoPoints(transform.position, lineRenderer.GetPosition(1));
                float deltaAngel = Math.Abs(deltaAngels - y);

                float angleDifference = Quaternion.Angle(rotRotate, Quaternion.Euler(new Vector3(0f, y, 0f)));
                //float angleDifference = Quaternion.Angle(unitPositions.transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)));




                if (angleDifference > 35f)
                {
                    moveRot = true;
                    //Debug.Log("wow");


                }
                if (moveRot)
                {
                    unitPositions.transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
                    moveRot = false;
                    UnitsTransform();
                    rotRotate = unitPositions.transform.rotation;
                }




                // unitPositions.transform.rotation = Quaternion.Slerp(unitPositions.transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)), Time.deltaTime * 5f);




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

                if (Vector3.Distance(rb.position, linePosition) < pointsDistance)
                {
                    var pointsList = new List<Vector3>(positions);
                    pointsList.RemoveAt(1);
                    positions = pointsList.ToArray();
                    lineRenderer.SetPositions(positions);
                    indexMove++;
                }

                MoraleChange(-(lostMoraleThenRun * (amountUnits / currentAmountUnits)*Time.deltaTime));

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
            // Vector3 norm = (moveDir - transform.position).normalized;


            if (Vector3.Distance(rb.position, moveDir) > (colliderObject.radius + 2.5f))
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

                    DefenceChange(-lostDefenceMoving * 2); // / баланс уменшаем защиту при любом движении
                
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
            UnitsTransform();
            rotRotate = unitPositions.transform.rotation;
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
                Debug.Log("все вырубай нахуй",gameObject);
            }
        }

        

        this.inBattle = inBattle;
        battleIndicator.SetActive(inBattle);



        if (!playerSquad) {
            AiActionValueReset();
        }
    }

    public void SetShield(int i, bool isShield) {

    }

    private void SetRotation(bool isStopRot) {
        this.isStopRot = isStopRot;

    }
    public void SetShooting(int i)
    {



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
    private float DefenceEffectIndex()
    {


        return currentDefence / defenceSquad;

        
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

    public void CancelMovement() {
        SetMoving(false);
        if (lineRenderer != null)
        {
            Destroy(lineRenderer.gameObject);
        }
        indexMove = 0;
        currentSpeed = 0;
        positions = null;
        tapCount = 0;
        startTapTime = 0;
        isGoingToEnemy = false;
    }

    public void MoraleChange(float morale) {
        if(currentMorale + morale > 0.1 && currentMorale + morale < maxMorale)
        {
            currentMorale += morale;
        }
        
        if (currentAmountUnits < retreatChanceCount)
        {
            RetreatUpdate();
        }
    }

    public void DefenceChange(float defence)
    {
        if (currentDefence + defence > 1 && currentDefence + defence < defenceSquad)
        {
            currentDefence += defence;
        }
        if (currentDefence  - lastDefValue > 0.3f || currentDefence - lastDefValue < -0.3f)
        {

            RadiusUpdate();
            UnitsTransform();
        }


    }
    
    public void RetreatUpdate()
    {


        if (currentMorale/maxMorale < 0.3f)
        {
            UnitInfo.SetBool("Retreat", true);

        }
        else
        {
            UnitInfo.SetBool("Retreat", false);
        }

    }





    public void OnMainTriggerEnter(Collider enemyCollider) { // тригеры тоже работают - все переводи на тригеры - колайдеры не нужны
        if (!escape) {

            if ((tag == SQUAD_TAG && enemyCollider.gameObject.tag == ENEMY_TAG) || (tag == ENEMY_TAG && enemyCollider.gameObject.tag == SQUAD_TAG))
            {


                if (enemyCollider.gameObject.layer == 12) // внутренний коллайдер отряда
                {
                    //if(mainEnemySquad == null) { 
                    //mainEnemySquad = enemyCollider.gameObject;
                    //}
                    isGoingToEnemy = false;

                    SquadController squad = enemyCollider.transform.gameObject.GetComponent<SquadController>();



                    if (!enemyController.Contains(squad))
                    {
                        enemyController.Add(squad);
                        CountCoef(enemyController[enemyController.Count - 1].type); // тут вопросы по напвильносит 


                       

                        if (isMoved)
                        {
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
                            Debug.Log("squadExiter.escape", gameObject);
                            SetBattle(false);
                        }
                    }
                    else
                    {
                        if (!inBattle)
                        {
                            Debug.Log("OnMainTriggerExit", gameObject);
                            GoToSquad(other.transform);
                        }
                        else
                        {
                            if (enemyController.Contains(squadExiter) && enemyController.Count == 1)
                            {

                                Debug.Log("GoToSquad OnMainTriggerExit", gameObject);
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
                            Debug.Log(" SetBattle(false);",gameObject);
                        }
                    }
                    if (enemyController.Count > 0)
                    {
                        if (!escape)
                        {
                            if (!inBattle)
                            {
                                Debug.Log(" !inBattle ", gameObject);
                                GoToSquad(enemyController[0].transform);
                            }
                        }
                    }
                    else
                    {
                        
                        if (!isMoved && inBattle)
                        {
                            Debug.Log("!isMoved && inBattle", gameObject);
                            SetBattle(false);
                           
                        }
                    }


                    //CanSquadFight();



                    //battleRot = false;


                }
            }
        }
    }


    private void EnemyDamage() {


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

        currentTriggerCoef = AttackTriggerCoef(); /// --------- посмотреть шо за фигня !!!!


        if (enController == null)
        {
            return;
        }



        float min = 0f - (enController.defenceSquad * 0.1f) - (enController.defenceCoef) - ((enController.currentMorale / enController.maxMorale) * 100f * 0.02f);

        if (enController.defence) {
            min -= (enController.actionUnits / 25f) * 3f;
        }

        float max = 10f + ((powerSquad - enController.defenceSquad) * 1.5f) + attackCoef + currentTriggerCoef + ((currentMorale / maxMorale) * 100f * 0.02f);


        if (!defence) {
            max += ((actionUnits / 25f) * 3f);
        } else {
            max += ((3f / 25f) * 3f);
        }

        if (enController == null)
        {
            return;
        }


        if (enController != null) //// можно улучшить 
        {


            float r = Random.Range(min, max);

            if (r > 8.8f)
            {
                int randomUnit = Random.Range(0, enController.avaliableToAttack.Count);

                enController.DieAttackUnit(currentTriggerCoef, randomUnit);

                Vector3 dir = (enController.transform.position - transform.position).normalized;
                float force = 100f + ((DefenceEffectIndex() - enController.DefenceEffectIndex())*150f);
                enController.SquadPush(dir, force  );
                SquadPush(dir, force );
                Debug.Log("force " + force);

            }
            else
            {
                enController.GetDamage();
            }

            if (enController == null||enemyController.Count ==0)
            {
                return;
            }
            animationState = 2;



            


        }
    }




    public void ClearUnits() // удаляем, очищаем все не нужное
    {

        List<GameObject> newUnitArray = new List<GameObject>();

        for (int i = 0; i < unitArray.Count; i++)
        {


            if (unitArray[i].transform.parent == gameObject.transform)
            {

                newUnitArray.Add(unitArray[i]);
            }
            else
            {

                Destroy(unitArray[i], GameOptions.unitsDeadTime);


                Animator unitAnimator = unitArray[i].GetComponent<Animator>();

                //if (animators.Contains(unitAnimator))
                //    animators.Remove(unitAnimator);

                if (avaliableToAttack.Contains(unitArray[i]))
                    avaliableToAttack.Remove(unitArray[i]);

            }



        }

        unitArray = newUnitArray;
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

    public void DieAttackUnit(float triggerCoef, int unitIndex) {// запускается на вражеском отряде, когда надо кого-то убить

        int r = -1;

        if (unitIndex == -1)// если не задан конкретный юнит
        {


            for (int i = 0; i < avaliableToAttack.Count; i++)
            {
                r = Random.Range(0, avaliableToAttack.Count);

                if (avaliableToAttack[r].transform.parent == gameObject.transform)
                {

                    i = avaliableToAttack.Count;
                }
                else
                {
                    r = -1;
                }


            }
        }
        else
        {
            r = unitIndex;
        }

        if (r >= 0)
        {
            if(r > avaliableToAttack.Count || avaliableToAttack.Count == 0) { return; }
            if (avaliableToAttack[r].gameObject == null) { return; }

            GameObject currentUnit = avaliableToAttack[r].gameObject;
            //GameObject currentUnit = unitArray[index];
            DeleteUnit(currentUnit);


            RadiusUpdate();
            TryToRetreat();




           
        }



    }

    public void DieRandomUnit()
    { // запускается на вражеском отряде, когда надо кого-то убить

        int r = -1;

        for (int i = 0; i < unitArray.Count; i++)
        {
            r = Random.Range(0, unitArray.Count);

            if (unitArray[r].transform.parent == gameObject.transform)
            {

                i = unitArray.Count;
            }
            else
            {
                r = -1;
            }


        }

        if (r >= 0)
        {


            GameObject currentUnit = unitArray[r].gameObject;



            DeleteUnit(currentUnit);




            Instantiate(bloodFx[Random.Range(0, bloodFx.Length)], currentUnit.transform.position, currentUnit.transform.rotation);
            Instantiate(deadFx, currentUnit.transform.position, deadFx.transform.rotation);

            //Destroy(currentUnit, deadTime);
            currentUnit.transform.parent = null;


            TryToRetreat();
            MoraleChange(-(lostMoraleThenDie * (amountUnits / currentAmountUnits)));
        }

    }

    private void TryToRetreat() {
        if (currentAmountUnits < retreatChanceCount) {
            float min = 0f - (1 - (amountUnits / currentAmountUnits)) + enemyController.Count;

            float max = currentMorale;

            RetreatUpdate();


            if (Random.Range(min, max) < 0.9f) { ///// зависит от сложности
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

    private void RadiusUpdate()
    {
        if (lastAmountUnits - unitArray.Count >= 4)
        {
            
            colliderObject.radius = (colliderRadius + unitPositions.scatterAmount) * ((unitArray.Count / amountUnits)+0.2f);
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

            UnitsTransform();
            SelectFightingUnits();

            Debug.Log("SelectFightingUnits", gameObject);
        }








        // colliderObject.radius = colliderRadius * (unitArray.Count / amountUnits);

    }

    private void UnitsTransform()
    {
        if(1f - DefenceEffectIndex() <= minimumFormation)
        {
            unitPositions.scatterAmount = minimumFormation;
        }
        else
        {
            unitPositions.scatterAmount = 1f - DefenceEffectIndex();
        }
       

        lastDefValue = currentDefence;
        unitPositions.RadiusUpdate();

        Transform[] points = unitPositions.points;
        
        for (int i = 0; i < unitArray.Count; i++)
        {
            unitArray[i].transform.position = points[i].position;
            initialPositions[i] = unitArray[i].transform.localPosition;

        }
    }
    public void DeleteEnemySquad(SquadController squad)
    {
        enemyController.Remove(squad);
        if (enemyController.Count == 0)
        {
            SetBattle(false);
            SpriteAnimationChange();
            //Debug.Log(" SetBattle(false);", gameObject);
        }

    }

        private void SquadDie() {

        if (enemyController.Count > 0) {
            for (int i = 0; i < enemyController.Count; i++) {

                enemyController[i].DeleteEnemySquad(this);
                //enemyController[i].mainEnemySquad = null;
                //enemyController[i].CanSquadFight(); 
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


        //for (int i = 0; i < unitArray.Count; i++)
        //{

        //    if(unitArray[i].gameObject.activeInHierarchy == false)
        //        Destroy(unitArray[i].gameObject);



        //}

        Destroy(gameObject);
    }

    public void CanSquadFight() {
        if (enemyController.Count > 0) {
            // тут надо развернуть отряд на 
        } else {
           // SetBattle(false);


            //escape = true;


        }

    }

    public void GetDamage() {

        damageCountAnimations++;


        MoraleChange(-lostMoraleThenDie );

       DefenceChange(- lostDefenceMoving/2f); 
        
    }

    public void CountCoef(SquadType enemyType) {
        foreach (Coef el in coef) {
            if (el.type == enemyType) {
                attackCoef = el.attack;
                defenceCoef = el.defence;
            }
        }
    }



    private float AttackTriggerCoef() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, colliderObject.radius + offsetRadius);

        foreach (var hitCollider in hitColliders) {
            if ((hitCollider.gameObject.tag == ENEMY_TRIGGER_TAG && gameObject.tag == SQUAD_TAG) || (hitCollider.gameObject.tag == SQUAD_TRIGGER_TAG && gameObject.tag == ENEMY_TAG)) {

                if (hitCollider.gameObject.name == "LeftTrigger") {
                    return leftTriggerCoef;
                }
                if (hitCollider.gameObject.name == "RightTrigger") {
                    return rightTriggerCoef;
                }
                if (hitCollider.gameObject.name == "FrontTrigger") {
                    return frontTriggerCoef;
                }
                if (hitCollider.gameObject.name == "BackTrigger") {
                    return backTriggerCoef;
                }
            }
        }
        return 0;
    }

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


                Collider[] hitColliders = Physics.OverlapSphere(eController.transform.position, colliderObject.radius, detrctionMask);

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



    public void AiActionValueReset() {
        aiActionValue = 100;
        dangerAlert = false;
    }

    public void SpriteRotate(Vector3 target,Transform unit)
    {
        float directionz = target.z - unit.position.z;
        float directionx = target.x - unit.position.x;

        //direction = Quaternion.EulerAngles(0f, -45f, 0f) * direction;

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
                    // meshRenderers[i].transform.localScale = new Vector3(spriteSize.x, spriteSize.y + 0.3f, spriteSize.z);
                    animatorControllers[i].SpriteAnimationChange(2);
                }else

                if (animatorControllers[i].state == 2)
                {
                    // meshRenderers[i].transform.localScale = new Vector3(spriteSize.x, spriteSize.y + 0.3f, spriteSize.z);
                    animatorControllers[i].SpriteAnimationChange(3);
                }
                else

                if (animatorControllers[i].state == 3)
                {
                    // meshRenderers[i].transform.localScale = new Vector3(spriteSize.x, spriteSize.y + 0.3f, spriteSize.z);
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


        float time = Time.time * (12f * MoraleEffectIndex());

        for (int i = 0; i < animatorControllers.Count; i++)
        {
            // Рассчитываем индексы в сетке
            int x = i % 5;
            int z = i / 5;

            // Вычисляем смещение волны
            float waveOffset = Mathf.Sin((x + z) * 10f + time) * 0.18f * MoraleEffectIndex();

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

    public void GoToSquad(Transform squad)
    {
        if (isMoved)
        {
            CancelMovement();
        }
        moveDir = squad.position;

        GameObject drawing = Instantiate(drawingPrefab);
        LineRenderer lineRenderer = drawing.GetComponent<LineRenderer>();
        this.lineRenderer = lineRenderer;

        Vector3 norm = Vector3.Normalize(squad.transform.position - transform.position);

        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, transform.position + norm);


        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, squad.transform.position);

        SetMoving(true);
        //SetBattle(false);
        Debug.Log("GoToSquad", gameObject);

    }
    public void SquadPush(Vector3 dir, float force)// отталкивание
    {

        rb.AddForce(dir * force );
        //animationState = 3;
        //SpriteAnimationChange();


    }
}


