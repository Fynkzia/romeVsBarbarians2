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

    [SerializeField] private float levelSquad;
    [SerializeField] public float powerSquad;
    [SerializeField] public float defenceSquad;
    [SerializeField] public float maxMorale;
    [SerializeField] public float maxStamina;
    [SerializeField] public float actionTime;
    [SerializeField] public int actionUnits;
    [Space(10)]

    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float boostSpeed;
    
    
    
    [Space(10)]
    [Header("morale specs")]
    [SerializeField] public float lostMoraleThenDie;
    [SerializeField] public float lostMoraleThenAttack;
    [SerializeField] public float recoveryMoraleSpeed;
    [Space(10)]
    [SerializeField] public float aroundRadius;
    [SerializeField] public float aroundSquadsBonus;
    [SerializeField] public float aroundRetreatBonus;
    [SerializeField] public LayerMask aroundMask;

    [Space(10)]
    [SerializeField] public float lostStaminaMoving;
    [SerializeField] public float lostStaminaAttack;
   
    [SerializeField] private float recoveryStamina;


    [Space(10)]
    [SerializeField] private float retreatCount;
    [SerializeField] private float retreatChanceCount;

    [Space(10)]
    [SerializeField] public List<GameObject> unitArray;
    [SerializeField] public float amountUnits;
    [SerializeField] public SquadType type;
    [SerializeField] private int coinsFromDeath;
    [SerializeField] public Coef[] coef;
    [SerializeField] private float leftTriggerCoef;
    [SerializeField] private float rightTriggerCoef;
    [SerializeField] private float frontTriggerCoef;
    [SerializeField] private float backTriggerCoef;
    [SerializeField] private float offsetRadius = 0.2f;

    [HideInInspector] public List<GameObject> nowAttacked;// массив юнитов которые уже находятся в атаке
    [SerializeField] public bool isMoved = false;
    [SerializeField] public LineRenderer lineRenderer;
    [HideInInspector] public List<Animator> animators = new List<Animator>();
    [HideInInspector] public float currentStamina;
    [HideInInspector] public float currentMorale;
    [SerializeField] public bool isStopRot = false;

    [Header("Movement Settings")]
    [Space(10)]
    
    
    [SerializeField] private float pointsDistance;
    [SerializeField] private float maxDeltaAngel;
    [SerializeField] private bool unitsRotating = false;
   

    [SerializeField]private float deltaAngels;


    [Header("Fighting Settings")]
    [Space(10)]
    [SerializeField] private float deadTime;
    [SerializeField] private float maxEscapeTime;
    [SerializeField] private bool playerSquad;
    [SerializeField] public bool inBattle;
    
    [SerializeField] public bool attack;
    [SerializeField] public bool defence;

    [SerializeField] private float attackTriggerCoef;//???
    [SerializeField] public float attackCoef;
    [SerializeField] public float defenceCoef;

    [SerializeField] public GameObject[,] SquadFormation = new GameObject[5, 5];
    [SerializeField] public Vector3[,] SquadFormationPositions = new Vector3[5, 5];
    
    [SerializeField] public int formationX = 5;
    [SerializeField] public int formationY = 5;
    [SerializeField] public float unitsSpacing = 1;

    [Header("DetectUnits")]
    [Space(10)]
    [SerializeField] public float radiusDetection = 1f;
    [SerializeField] public List<GameObject> avaliableToAttack;
    [SerializeField] public List<GameObject> avaliableTargetsToAttack;
    [SerializeField] public LayerMask detrctionMask;
    [SerializeField] public bool ignoreTriggers;
        

    [Header("Animation Settings")]
    [Space(10)]
    [SerializeField] public int maxFightingUnit;
    [SerializeField] private ParticleSystem bloodFx;
    [SerializeField] private GameObject movementIndicator;
    [SerializeField] private GameObject battleIndicator;
    [SerializeField] private float unitAttackDistance = 1.5f;


    [Header("Enemy AI settings")]
    [Space(10)]
    [SerializeField] public int aiActionValue = 100;
    [SerializeField] public bool dangerAlert = false;

    

    private Rigidbody rb;
    [SerializeField] private GameObject enemySquad;
    [Space(10)]
    public bool isGoingToEnemy = false;
    public Collider predictEnemy;
    [SerializeField] public List<SquadController> enemyController = new List<SquadController>();
    private Vector3[] positions;
    private List<Transform> fightingUnitList = new List<Transform>();
    private Quaternion currentRotation;


    private int indexMove = 0;
    private float currentSpeed = 0f;
    private float battleTime = 0f;
    private float restorTime = 0f;
    private float animationAttackTime = 0f;
    private bool battleRot = false;
    private bool escape = false;
    private bool squadDie = false;
    private float escapeTime = 0f;
    private float currentTriggerCoef;
    private float aroundBonus;
    private CoinsController coinsController;
    private SquadControlManager controlController;
    private EnemyAIController aIController;
    private WinLoseManager winLoseManager;
    [SerializeField] private Animator UnitInfo;

    private float colliderRadius;

    public int tapCount = 0;
    private float startTapTime = 0;
    private float maxTimeWait = 0.5f;

    private const string IS_MOVING = "Moving";
    private const string IS_BATTLE = "Battle";
    private const string IS_ROTATION = "Rotation";
    private const string ATTACK = "Attack";
    private const string DIE = "Die";
    private const string RANDOM_SPEED = "RandomSpeed";
    private const string ENEMY_TAG = "Enemy";
    private const string SQUAD_TAG = "Squad";
    private const string ENEMY_TRIGGER_TAG = "EnemyTrigger";
    private const string SQUAD_TRIGGER_TAG = "SquadTrigger";

    private void Awake() {
        for (int i = 0; i < unitArray.Count; i++) {
            animators.Add(unitArray[i].GetComponent<Animator>());
        }
        currentStamina = maxStamina;
        currentMorale = maxMorale;
        battleTime = actionTime;

        coinsController = GameObject.Find("CoinsController").GetComponent<CoinsController>();
        controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();
        aIController = GameObject.Find("AIManager").GetComponent<EnemyAIController>();
        winLoseManager = GameObject.Find("WinLoseManager").GetComponent<WinLoseManager>();

        deltaAngels = transform.rotation.eulerAngles.y;
    }
    private void Start() {
        rb = GetComponent<Rigidbody>();
        for (int i = 0; i < animators.Count; i++) {
            if (Random.Range(1, 3) % 2 == 0) {
                animators[i].SetFloat(RANDOM_SPEED, 1.25f);
            }
            else {
                animators[i].SetFloat(RANDOM_SPEED, 0.75f);
            }
        }
        colliderRadius = GetComponents<SphereCollider>()[1].radius;


        

 }

   
   


    private void FixedUpdate() {
        if (isMoved) {
            SquadMovement();
            if (currentStamina > 0) { currentStamina -= lostStaminaMoving * Time.fixedDeltaTime; }

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
            animationAttackTime += Time.fixedDeltaTime + (Random.Range(-0.1f, 0.1f) * Time.fixedDeltaTime);
            if (!battleRot) {
                LookOnEnemy();
            }
            else {
                if(!defence){
                    if (battleTime >= actionTime) { // только нанесение урона
                        battleTime = 0f;
                        EnemyDamage();
                       

                    }
                    if (animationAttackTime >= actionTime / actionUnits) {// только анимация атаки юнитов
                        animationAttackTime = 0f;
                        UnitKickOnce();
                    }
                }else{

                     
                        if (battleTime >= actionTime) { // только нанесение урона
                            battleTime = 0f;
                            EnemyDamage();
                            

                        }
                        if (animationAttackTime >= actionTime / 3f) {// только анимация атаки юнитов
                            animationAttackTime = 0f;
                            UnitKickOnce();
                        }
                
                }
            }
        }

       
        restorTime += Time.fixedDeltaTime;
        if(restorTime > 1f)
        {
            if (!inBattle && currentMorale < maxMorale)
            {
                MoraleAnalise();
                MoraleChange(recoveryMoraleSpeed * (unitArray.Count / amountUnits) + aroundBonus);
                
            }

             if (!isMoved && currentStamina < maxStamina && !inBattle)
            {
                currentStamina += recoveryStamina;
            }

            restorTime = 0;
        }

        if (escape) {
            if (escapeTime < maxEscapeTime) {
                escapeTime += Time.fixedDeltaTime;
            }
            else {
                escape = false;
            }
        }

    }
    private void SquadMovement() {
        if (indexMove < lineRenderer.positionCount) {
            positions = new Vector3[(int)lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);

            Vector3 linePosition = new Vector3(lineRenderer.GetPosition(1).x, transform.position.y, lineRenderer.GetPosition(1).z);
            Vector3 targetPos = Vector3.MoveTowards(transform.position, linePosition, CurrentSpeed() * Time.fixedDeltaTime);

            float y = MathUtilities.AngleBetweenTwoPoints(transform.position, lineRenderer.GetPosition(1));
            float deltaAngel = Math.Abs(deltaAngels - y );

            
            if (!isStopRot) {
                if (deltaAngel > maxDeltaAngel && deltaAngel < 360 - maxDeltaAngel) {
                    SetRotation(true);
                    currentSpeed = 0;
                    unitsRotating = true;
                }
                else {
                    if (unitsRotating)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)), rotationSpeed / 5f * Time.fixedDeltaTime);
                        for (int i = 0; i < unitArray.Count; i++)
                        {
                            if(unitArray[i] != null)
                            unitArray[i].transform.rotation = Quaternion.Euler(new Vector3(0f, y + 180f, 0f));
                        }

                        float angleDifference = Quaternion.Angle(transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)));
                        if (angleDifference < 1)
                        {
                            unitsRotating = true;
                        }
                    

                    }
                    rb.MovePosition(targetPos);
                    lineRenderer.SetPosition(0, transform.position);
                }
            }
            if (Vector3.Distance(rb.position, linePosition) < pointsDistance) {
                var pointsList = new List<Vector3>(positions);
                pointsList.RemoveAt(1);
                positions = pointsList.ToArray();
                lineRenderer.SetPositions(positions);
                indexMove++;
            }

            if (isStopRot) {
               


                foreach (GameObject unit in unitArray)
                {
                    if (unit != null)
                    {
                        Quaternion unitRotation = unit.transform.rotation;
                        unit.transform.rotation = Quaternion.RotateTowards(unitRotation, Quaternion.Euler(new Vector3(0f, y + 180f, 0f)), rotationSpeed * Time.fixedDeltaTime);
                        deltaAngels = unit.transform.rotation.eulerAngles.y-180;
                    }
                }




               
                float deltaAngelLocal = Math.Abs(deltaAngels - y);

                

                if (deltaAngelLocal < 1f || deltaAngelLocal > 360 - 1f)
                {
                    SetRotation(false);
                    for (int i = 0; i < unitArray.Count; i++)
                    {
                        unitArray[i].transform.rotation = Quaternion.Euler(new Vector3(0f, y + 180f, 0f));
                    }
                }
               


            }
        }
        else {
            CancelMovement();
            

            if (!playerSquad){
                AiActionValueReset();
            }
        }
    }

    public void SetMoving(bool isMoved) {
        this.isMoved = isMoved;
        for (int i = 0; i < animators.Count; i++) {
            if (animators.Count < i)
                return;
            if (animators[i] == null)
                return;
            animators[i].SetBool(IS_MOVING, isMoved);
        }
        movementIndicator.SetActive(isMoved);
    }

    public void SetBattle(bool inBattle) {

        if (this.inBattle && !inBattle)
        {
            ArrangeInSquare(unitArray, unitsSpacing);
        }

        this.inBattle = inBattle;
        for (int i = 0; i < animators.Count; i++) {
            if (animators.Count < i)
                return;
            if (animators[i] == null)
                return;
            animators[i].SetBool(IS_BATTLE, inBattle);
        }
        battleIndicator.SetActive(inBattle);

        

        if(!playerSquad){
                AiActionValueReset();
            }
    }
    public void SetShield(int i, bool isShield) {
        animators[i].SetBool("Shield", isShield);
    }

    private void SetRotation(bool isStopRot) {
        this.isStopRot = isStopRot;
        for (int i = 0; i < animators.Count; i++) {
            if (animators.Count < i)
                return;
            if (animators[i] == null)
                return;
            animators[i].SetBool(IS_ROTATION, isStopRot);
        }
    }
    public void SetShooting(int i)
    {
        animators[i].SetTrigger("Shooting");

        
    }

    private float CurrentSpeed() {
        float currentMoveSpeed = movementSpeed * StaminaEffectIndex();
        float currentBoostSpeed = boostSpeed * StaminaEffectIndex();
        if (currentSpeed < currentMoveSpeed) {
            currentSpeed += currentBoostSpeed * Time.fixedDeltaTime;
        }
        else {
            currentSpeed = currentMoveSpeed;
        }
        return currentSpeed;
    }

    private float StaminaEffectIndex() {
        if (currentStamina / maxStamina > 0.75) {
            return 1f;
        }
        if (currentStamina / maxStamina > 0.5) {
            return 0.85f;
        }
        if (currentStamina / maxStamina > 0.25) {
            return 0.6f;
        }

        return 0.5f;
    }

    public void CancelMovement() {
        SetMoving(false);
        Destroy(lineRenderer.gameObject);
        indexMove = 0;
        currentSpeed = 0;
        positions = null;
        tapCount = 0;
        startTapTime = 0;
        isGoingToEnemy = false;
    }

    public void MoraleChange(float morale) {

        currentMorale += morale;
        if (unitArray.Count < retreatChanceCount)
        {
            RetreatUpdate();
        }
    }
    public void MoraleAnalise()
    {
        aroundBonus = 0;

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, aroundRadius, aroundMask);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider != null)
            {
                if (hitCollider.transform.parent.gameObject.layer == 12)
                {
                    if ((gameObject.tag == SQUAD_TAG && hitCollider.transform.parent.gameObject.tag == SQUAD_TAG) || (gameObject.tag == ENEMY_TAG && hitCollider.transform.parent.gameObject.tag == ENEMY_TAG))
                        aroundBonus += aroundSquadsBonus;

                    if ((gameObject.tag == SQUAD_TAG && hitCollider.transform.parent.gameObject.tag == ENEMY_TAG) || (gameObject.tag == ENEMY_TAG && hitCollider.transform.parent.gameObject.tag == SQUAD_TAG))
                        aroundBonus -= aroundSquadsBonus;
                }

                if (hitCollider.gameObject.layer == 10)
                {
                    aroundBonus -= aroundRetreatBonus;
                }
            }
        }

    }

    public void RetreatUpdate()
    {
       

        if (currentMorale <  2f)
        {
            UnitInfo.SetBool("Retreat", true);
            
        }
        else
        {
            UnitInfo.SetBool("Retreat", false);
        }

    }

   



public void OnMainTriggerEnter(Collider enemyCollider) { // тригеры тоже работают - все переводи на тригеры - колайдеры не нужны
        if (!escape || !ignoreTriggers ) {
               
                if ((tag == SQUAD_TAG && enemyCollider.gameObject.tag == ENEMY_TAG) || (tag == ENEMY_TAG && enemyCollider.gameObject.tag == SQUAD_TAG) ) { 
                    enemySquad = enemyCollider.gameObject;
                    isGoingToEnemy = false;

                SquadController squad = enemySquad.transform.parent.gameObject.GetComponent<SquadController>();



                if (!enemyController.Contains(squad))
                {
                    enemyController.Add(squad);
                    CountCoef(enemyController[enemyController.Count - 1].type); // тут вопросы по напвильносит 


                    MoraleChange(- lostMoraleThenAttack * (enemyController[enemyController.Count - 1].unitArray.Count/ unitArray.Count));
                    if (!inBattle)
                    {
                        movementSpeed /= 1.5f;
                        rotationSpeed /= 1.5f;
                        boostSpeed /= 1.5f;

                        SetBattle(true);
                    }
                    SelectFightingUnits(enemySquad.transform.parent.transform);
                    SelectTargetsUnits();


                    if (isMoved)
                    {
                        CancelMovement();
                    }
                }
                }
         

        }
    }
    public void OnMainTriggerExit(Collider other) {
        if ((other.gameObject.tag != ENEMY_TRIGGER_TAG && other.gameObject.tag != SQUAD_TRIGGER_TAG) || other.gameObject.layer != 15) {

            SquadController squadExiter = other.transform.parent.gameObject.GetComponent<SquadController>();

         

            if ((tag == SQUAD_TAG && squadExiter.tag == ENEMY_TAG) || (tag == ENEMY_TAG && squadExiter.tag == SQUAD_TAG)) {

                if (enemyController.Contains(squadExiter))
                {
                    Debug.Log("exit");
                    enemyController.Remove(squadExiter);
                }
                

                
                    CanSquadFight();
                   
                

                battleRot = false;

               
            }
        }
    }

    private void LookOnEnemy() {
        float y = MathUtilities.AngleBetweenTwoPoints(transform.position, enemyController[0].transform.position);

        foreach (GameObject unit in unitArray)
        {
            if (unit != null)
            {
                Quaternion unitRotation = unit.transform.rotation;
                unit.transform.rotation = Quaternion.RotateTowards(unitRotation, Quaternion.Euler(new Vector3(0f, y + 180f, 0f)), rotationSpeed * Time.fixedDeltaTime);
                currentRotation = unit.transform.rotation;
            }
        }

        float deltaAngelLocal = Math.Abs(currentRotation.eulerAngles.y - y + 180f);

        

        if (deltaAngelLocal < 1 || deltaAngelLocal > 360f - 1f)
        {
            SetRotation(false);
            battleRot = true;
            SelectFightingUnits(enemySquad.transform);

            transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));

            foreach (GameObject unit in unitArray)
            {
                if (unit != null)
                {

                    unit.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

                }
            }
        }
    }

    
    async private void UnitKickOnce() {

        int index = 0;

        if (avaliableTargetsToAttack.Count < 2)
        {
            SelectTargetsUnits();
        }

        index = Random.Range(0, avaliableToAttack.Count);

        if(index >= avaliableToAttack.Count || avaliableToAttack[index] == null )
        {
            return;
        }

        Transform currentUnit = avaliableToAttack[index].transform;




        if (!nowAttacked.Contains(currentUnit.gameObject))  {
            nowAttacked.Add(currentUnit.gameObject);

            Vector3 startPosition = currentUnit.position;
            Animator unitAnimator = currentUnit.GetComponent<Animator>();
            unitAnimator.SetTrigger(ATTACK);

            Transform atackedEnemy;

            int enemyIndex = 0;
            int enemySquadIndex = 0;


            enemyIndex = Random.Range(0, avaliableTargetsToAttack.Count);
            enemySquadIndex = Random.Range(0, enemyController.Count);

            if (enemyIndex >= avaliableTargetsToAttack.Count || avaliableTargetsToAttack[enemyIndex] == null)
            {
                return;
            }

            atackedEnemy = avaliableTargetsToAttack[enemyIndex].transform;

                Vector3 directionToEnemy = atackedEnemy.transform.position - currentUnit.transform.position;
                directionToEnemy.y = 0; // Оставляем только горизонтальную компоненту направления
                currentUnit.transform.rotation = Quaternion.LookRotation(directionToEnemy) * Quaternion.EulerAngles(0f, 90f, 0f);

          

            // Вычисляем позицию удара на определенном расстоянии от врага
            Vector3 attackPosition = atackedEnemy.transform.position - directionToEnemy * unitAttackDistance;


           // Vector3 halfPosition = new Vector3((currentUnit.position.x + atackedEnemy.position.x) / 2, (currentUnit.position.y + atackedEnemy.position.y) / 2, (currentUnit.position.z + atackedEnemy.position.z) / 2);

            await currentUnit.DOMove(attackPosition, actionTime / 2f).AsyncWaitForCompletion();
                if ( enemyController.Count != 0 && enemySquadIndex < enemyController.Count && enemyController[enemySquadIndex] != null )
                {
                    enemyController[enemySquadIndex].GetDamage(enemyIndex);
                }
            await currentUnit.DOMove(startPosition, actionTime / 1.5f).AsyncWaitForCompletion();

                
            


            if(currentUnit != null){
            nowAttacked.Remove(currentUnit.gameObject);
            }

        }
    }

    async private void GoToEmptySpace(Transform unit, Vector3 place)
    {
        await unit.DOMove(place, actionTime / 2f).AsyncWaitForCompletion();
        if (enemyController.Count > 0)
        {

            SelectFightingUnits(enemySquad.transform);
           

        }
    }

    async private void GoToFormation(Transform unit, Vector3 place)
    {
        await unit.DOMove(place, actionTime ).AsyncWaitForCompletion();
       

    }
    


    private void EnemyDamage() {

        if (enemyController.Count == 0)
        {
            return;
        }

        currentTriggerCoef = AttackTriggerCoef();

        int enemySquadIndex = 0;
        enemySquadIndex = Random.Range(0, enemyController.Count);



        if(enemyController.Count == 0)
        {
            return;
        }

        float min = 0f - (enemyController[enemySquadIndex].defenceSquad*0.1f) - (enemyController[enemySquadIndex].defenceCoef) - ((enemyController[enemySquadIndex].currentStamina / enemyController[enemySquadIndex].maxStamina)*100f*0.02f);

        if(enemyController[enemySquadIndex].defence){
            min -= (enemyController[enemySquadIndex].actionUnits/25f)*3f;
        }

        float max = 10f + ((powerSquad-enemyController[enemySquadIndex].defenceSquad)*1.5f)+attackCoef +  currentTriggerCoef +((currentStamina / maxStamina)*100f*0.02f);

        if(!defence){
           max += ((actionUnits/25f)*3f);
        }else{
             max += ((3f/25f)*3f);
        }

        if (enemyController.Count == 0)
        {
            return;
        }



        if (avaliableTargetsToAttack.Count < 2)
        {
        
            SelectTargetsUnits();
        }

        int index;


        //index = Random.Range(0, avaliableTargetsToAttack.Count);
        index = Random.Range(0, enemyController.Count);

        if (enemyController.Count == 0)
        {
            return;
        }

        if (enemyController[index] != null) //// можно улучшить 
        {
            SquadController enController = null;
            if (enemyController[index] != null && enemyController.Count != 0)
            {
                //enController = avaliableTargetsToAttack[index].transform.parent.parent.GetComponent<SquadController>();
                enController = enemyController[index];
            }

            if (index < avaliableTargetsToAttack.Count && enController != null) //// можно улучшить 
            {

                float r = Random.Range(min, max);

                if (r > 8.8f)
                {
                    if (enemyController.Count > 0 || enController != null)
                    {
                        GameObject killEnemy;
                        killEnemy = avaliableTargetsToAttack[index];

                        if (killEnemy != null)
                        {
                            enController.DieUnit(index, currentTriggerCoef, killEnemy);
                            coinsController.ChangeAmountOfCoins(coinsFromDeath);
                        }
                    }

                    if (enemyController.Count > 0 || enController != null)
                    {
                        avaliableTargetsToAttack.Remove(avaliableTargetsToAttack[index].gameObject);
                    }

                    if (enemyController.Count > 0 || enController != null)
                    {

                       // SelectFightingUnits(enemyController[enemySquadIndex].transform);
                        SelectTargetsUnits();

                    }





                    }
                else
                {
                    enemyController[enemySquadIndex].GetDamage(index);
                }
                if (currentStamina > 0)
                {
                    currentStamina -= lostStaminaAttack;
                }
            }

        }
    }

    public void DeleteUnit(GameObject unit)
    {
        GameObject currentUnit = unit;
        Animator unitAnimator = unit.GetComponent<Animator>();

        unitArray.Remove(unit);
        animators.Remove(unitAnimator);

        if(avaliableToAttack.Contains(unit))
        avaliableToAttack.Remove(unit);

        unitAnimator.SetTrigger(DIE);
        
    }

     public void DieUnit(int index,float triggerCoef, GameObject unit) { // запускается на вражеском отряде, когда надо кого-то убить

        //GameObject currentUnit = unitArray[index];
        DeleteUnit(unit);
        unit.layer = 0;

        
        Instantiate(bloodFx, unit.transform);

        
        Destroy(unit, deadTime);
        unit.transform.parent = null;

        if (avaliableToAttack.Contains(unit))
        {
            avaliableToAttack.Remove(unit);
        }

        TryToRetreat();

        GoToEmptySpace(unitArray[Random.Range(0, unitArray.Count)].transform, unit.transform.position);

        MoraleChange( - (lostMoraleThenDie * (amountUnits/unitArray.Count)) + (triggerCoef / 10f));
        
    }

    public void DieRandomUnit()
    { // запускается на вражеском отряде, когда надо кого-то убить

        int random = Random.Range(0, unitArray.Count);

        if(random >= unitArray.Count||unitArray[random] == null)
        {
            return;
        }


        GameObject currentUnit = unitArray[random].gameObject;

       

        DeleteUnit(currentUnit);

        currentUnit.layer = 0;

      
        Instantiate(bloodFx, currentUnit.transform);


        Destroy(currentUnit, deadTime);
        currentUnit.transform.parent = null;


        TryToRetreat();
        MoraleChange(-(lostMoraleThenDie * (amountUnits / unitArray.Count)));
        
    }

    private void TryToRetreat() {
        if(unitArray.Count < retreatChanceCount){
            float min = 0f - (1 - (amountUnits / unitArray.Count)) + enemyController.Count;

            float max = currentMorale;

            RetreatUpdate();


            if (Random.Range(min, max) < 0.9f) { ///// зависит от сложности
                    SetBattle(false);
                squadDie = true;

                for (int i = 0; i < unitArray.Count;i++) {
                    Transform currentModel = unitArray[i].transform;
                    currentModel.gameObject.AddComponent<Rigidbody>();
                    currentModel.gameObject.AddComponent<SphereCollider>();
                    currentModel.GetComponent<UnitRetreatController>().enabled = true;
                    currentModel.parent = null;
                }
                 SquadDie();
                 return;
            }

             if (unitArray.Count < retreatCount||unitArray.Count == 0) {
                SquadDie();
             }

        }

    }

    private void SquadDie() {

        if (enemyController.Count > 0) {
             for(int i = 0; i < enemyController.Count;i++) {
             
             enemyController[i].enemyController.Remove(this);
             enemyController[i].enemySquad = null;
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

     public void CanSquadFight() {
        if(enemyController.Count > 0){
// тут надо развернуть отряд на 
        }else{
            SetBattle(false);
            
            movementSpeed *= 1.5f;
            rotationSpeed *= 1.5f;
            boostSpeed *= 1.5f;
            escape = true;

            
        }

     }

    public void GetDamage(int index) {
        if (index >= animators.Count || animators[index] == null)
            return;

        if (Random.Range(1, 11) % 2 == 0) { 
            animators[index].SetTrigger("GetDamage");
        } else {
            animators[index].SetTrigger("GetDamage2");
        }

        MoraleChange(-(lostMoraleThenDie * (amountUnits / unitArray.Count) * enemyController.Count)/5f);
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
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, colliderRadius+offsetRadius);
        
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

    private void SelectFightingUnits(Transform enemyTransform) /// можно улучшить - несколько раз искать с каждыйм разом большим радиусом. Искать точное количество (шоб на одного не нападать)
    {
        avaliableToAttack.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(enemyTransform.position, radiusDetection, detrctionMask);

        foreach (var hitCollider in hitColliders)
        {
            if (squadDie)
            {
                return;
            }
            if(hitCollider.transform.parent == gameObject.transform)
            avaliableToAttack.Add(hitCollider.gameObject);
        }

        if(avaliableToAttack.Count == 0)
        {
            
            
        }
                
    }

    private void SelectTargetsUnits()
    {
        avaliableTargetsToAttack.Clear();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radiusDetection, detrctionMask);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.tag != gameObject.tag)
                avaliableTargetsToAttack.Add(hitCollider.gameObject);
        }

    }

    public void AiActionValueReset() {
        aiActionValue = 100;
        dangerAlert = false;
    }

    void ArrangeInSquare(List<GameObject> units, float spacing)
    {
        int unitCount = units.Count;
        int sideLength = Mathf.CeilToInt(Mathf.Sqrt(unitCount)); // Определяем длину стороны квадрата

        for (int i = 0; i < unitCount; i++)
        {
            int row = i / sideLength; // Вычисляем строку
            int col = i % sideLength; // Вычисляем столбец

            float offsetX = (sideLength - 1) * spacing / 2;
            float offsetZ = (sideLength - 1) * spacing / 2;

           

            Vector3 newPosition = new Vector3(col * spacing - offsetX, 1, row * spacing - offsetZ); // Определяем новую позицию
           // units[i].transform.localPosition = newPosition; // Перемещаем юнит
           if(i < units.Count || units[i] != null)
            GoToFormation(units[i].transform, newPosition + transform.position);
        }
    }

    

}


