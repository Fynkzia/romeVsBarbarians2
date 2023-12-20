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
    [HideInInspector] public LineRenderer lineRenderer;
    [HideInInspector] public List<Animator> animators = new List<Animator>();
    [HideInInspector] public float currentStamina;
    [HideInInspector] public float currentMorale;
    [HideInInspector] public bool isStopRot = false;

    [Header("Movement Settings")]
    [Space(10)]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float boostSpeed;
    [SerializeField] public float maxStamina;
    [SerializeField] private float recoveryStamina;
    [SerializeField] private float consumptionStamina;
    [SerializeField] private float pointsDistance;
    [SerializeField] private float maxDeltaAngel;

    [Header("Fighting Settings")]
    [Space(10)]
    [SerializeField] private float deadTime;
    [SerializeField] private float maxEscapeTime;
    [SerializeField] private bool playerSquad;
    [SerializeField] public bool inBattle;
    [SerializeField] private float levelSquad;
    [SerializeField] public float powerSquad;
    [SerializeField] public float defenceSquad;
    [SerializeField] public float maxMorale;
    [SerializeField] public float actionTime;
    [SerializeField] public int actionUnits;
    [SerializeField] public bool attack;
    [SerializeField] public bool defence;

    [SerializeField] private float attackTriggerCoef;//???
    [SerializeField] public float attackCoef;
    [SerializeField] public float defenceCoef;

    [Header("Animation Settings")]
    [Space(10)]
    [SerializeField] public int maxFightingUnit;
    [SerializeField] private ParticleSystem bloodFx;


    [Header("Morale Settings")]
    [Space(10)]
    [SerializeField] private float lostMoraleThenDie;
    [SerializeField] private float lostMoraleThenAttack;
    [SerializeField] private float recoveryMoraleSpeed;

    [Header("Retreat Settings")]
    [Space(10)]
    [SerializeField] private float retreatCount;
    [SerializeField] private float retreatChanceCount;
    [SerializeField] private float retreatChanceDefault;
    [SerializeField] private float retreatCoef;

    private Rigidbody rb;
    private GameObject enemySquad;
    private SquadController enemyController;
    private Vector3[] positions;
    private List<Transform> fightingUnitList = new List<Transform>();
    [SerializeField] private List<Transform> backUnitList = new List<Transform>();
    [SerializeField] private List<Transform> rightUnitList = new List<Transform>();
    [SerializeField] private List<Transform> leftUnitList = new List<Transform>();

    private int indexMove = 0;
    private float currentSpeed = 0f;
    private float battleTime = 0f;
    private float animationAttackTime = 0f;
    private bool battleRot = false;
    private bool escape = false;
    private float escapeTime = 0f;
    private float currentTriggerCoef;
    private CoinsController coinsController;
    private EnemyAIController aIController;
    private WinLoseManager winLoseManager;
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
            animators.Add(unitArray[i].transform.GetChild(0).GetComponent<Animator>());
        }
        currentStamina = maxStamina;
        currentMorale = maxMorale;
        battleTime = actionTime;
        coinsController = GameObject.Find("CoinsController").GetComponent<CoinsController>();
        aIController = GameObject.Find("AIManager").GetComponent<EnemyAIController>();
        winLoseManager = GameObject.Find("WinLoseManager").GetComponent<WinLoseManager>();
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

//SidesUnitsListSet();
               

 }

    private void FixedUpdate() {
        if (isMoved) {
            SquadMovement();
            if (currentStamina > 0) { currentStamina -= consumptionStamina * Time.fixedDeltaTime; }

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
        else if (currentStamina < maxStamina && !inBattle) {
            currentStamina += recoveryStamina * Time.fixedDeltaTime;
        }

        if (inBattle) {
            battleTime += Time.fixedDeltaTime;
            animationAttackTime += Time.fixedDeltaTime + (Random.Range(-0.1f, 0.1f) * Time.fixedDeltaTime);
            if (!battleRot) {
                LookOnEnemy();
            }
            else {
                if (battleTime >= actionTime) { // только нанесение урона
                    battleTime = 0f;
                    EnemyDamage();
                    // UnitKick(enemySquad.transform);

                }
                if (animationAttackTime >= actionTime / actionUnits) {// только анимация атаки юнитов
                    animationAttackTime = 0f;
                    UnitKickOnce();
                }
            }
        }

        if (!inBattle && currentMorale < maxMorale) {
            currentMorale += recoveryMoraleSpeed * (unitArray.Count / amountUnits) * Time.fixedDeltaTime;
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

            Vector3 linePosition = new Vector3(lineRenderer.GetPosition(0).x, transform.position.y, lineRenderer.GetPosition(0).z);
            Vector3 targetPos = Vector3.MoveTowards(transform.position, linePosition, CurrentSpeed() * Time.fixedDeltaTime);

            float y = AngleBetweenTwoPoints(transform.position, lineRenderer.GetPosition(0));
            float deltaAngel = Math.Abs(transform.rotation.eulerAngles.y - y);
            //Debug.Log(deltaAngel);
            if (!isStopRot) {
                if (deltaAngel > maxDeltaAngel && deltaAngel < 360 - maxDeltaAngel) {
                    SetRotation(true);
                    currentSpeed = 0;
                }
                else {
                    transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
                    rb.MovePosition(targetPos);
                }
            }
            if (Vector3.Distance(rb.position, linePosition) < pointsDistance) {
                var pointsList = new List<Vector3>(positions);
                pointsList.RemoveAt(0);
                positions = pointsList.ToArray();
                lineRenderer.SetPositions(positions);
                indexMove++;
            }

            if (isStopRot) {
                //transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)), rotationSpeed * Time.fixedDeltaTime);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)), rotationSpeed * Time.fixedDeltaTime);

                if (deltaAngel < 4) {
                    SetRotation(false);
                }

            }
        }
        else {
            CancelMovement();
        }
    }

    public void SetMoving(bool isMoved) {
        this.isMoved = isMoved;
        for (int i = 0; i < animators.Count; i++) {
            animators[i].SetBool(IS_MOVING, isMoved);
        }
    }

    public void SetBattle(bool inBattle) {
        this.inBattle = inBattle;
        for (int i = 0; i < animators.Count; i++) {
            animators[i].SetBool(IS_BATTLE, inBattle);
        }
    }
    public void SetShield(int i, bool isShield) {
        animators[i].SetBool("Shield", isShield);
    }

    private void SetRotation(bool isStopRot) {
        this.isStopRot = isStopRot;
        for (int i = 0; i < animators.Count; i++) {
            animators[i].SetBool(IS_ROTATION, isStopRot);
        }
    }

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return 180f - Mathf.Atan2(a.z - b.z, a.x - b.x) * Mathf.Rad2Deg;
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
    }

    private void OnTriggerEnter(Collider enemyCollider) {
        if (!escape) {
            if(enemyCollider.gameObject.tag != ENEMY_TRIGGER_TAG && enemyCollider.gameObject.tag != SQUAD_TRIGGER_TAG) {
                enemySquad = enemyCollider.gameObject;
                if ((tag == SQUAD_TAG && enemySquad.tag == ENEMY_TAG) || (tag == ENEMY_TAG && enemySquad.tag == SQUAD_TAG)) {
                    enemyController = enemyCollider.transform.GetComponent<SquadController>();
                    CountCoef(enemyController.type);
                    
                    SetBattle(true);
                    currentMorale -= lostMoraleThenAttack * enemyController.unitArray.Count;
                    movementSpeed /= 1.5f;
                    rotationSpeed /= 1.5f;
                    boostSpeed /= 1.5f;

                     aIController.SetBattlePoint(transform.position);

                    //GetComponents<SphereCollider>()[1].radius = colliderRadius + 0.1f;
                    if (isMoved) {
                        CancelMovement();
                    }
                }
            }

        }
    }
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.tag != ENEMY_TRIGGER_TAG && other.gameObject.tag != SQUAD_TRIGGER_TAG) {

            if ((tag == SQUAD_TAG && enemySquad.tag == ENEMY_TAG) || (tag == ENEMY_TAG && enemySquad.tag == SQUAD_TAG)) {
                Debug.Log(gameObject.name+" here",other.gameObject);
                SetBattle(false);
                movementSpeed *= 1.5f;
                rotationSpeed *= 1.5f;
                boostSpeed *= 1.5f;
                escape = true;

               

                //GetComponents<SphereCollider>()[1].radius = colliderRadius;
            }
        }
    }

    private void LookOnEnemy() {
        float y = AngleBetweenTwoPoints(transform.position, enemySquad.transform.position);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)), rotationSpeed * Time.fixedDeltaTime);

        if (Mathf.Abs(transform.rotation.eulerAngles.y - y) < 3) {
            battleRot = true;
        }
    }

    async private void UnitKick(Transform enemyTransform) {
        List<int> indexArray = new List<int>();
        for (int i = 0; i < actionUnits; i++) {
            //int index = Random.Range(0, unitArray.Count - 1);
            int index = 0;
            if (unitArray.Count > maxFightingUnit) {
                index = Random.Range(0, maxFightingUnit);
            }
            else {
                index = Random.Range(0, unitArray.Count);
            }
            //if (tag == SQUAD_TAG) { 
            //
            //Debug.Log( "random = "+index);
            //}
            if (indexArray.Contains(index)) {
                i--;
                //if (tag == SQUAD_TAG) {
                //
                //Debug.Log("nahyi = "+index);
                //}
            }
            else {
                indexArray.Add(index);
                Transform currentUnit = unitArray[index].transform;
                //if (tag == SQUAD_TAG) { 
                //
                //Debug.Log("adding = " + index);
                //}

                Vector3 startPosition = currentUnit.position;
                animators[index].SetTrigger(ATTACK);



                Transform atackedEnemy;

                if (enemyController.unitArray.Count > enemyController.maxFightingUnit) {
                    atackedEnemy = enemyController.unitArray[Random.Range(0, enemyController.maxFightingUnit)].transform;
                }
                else {
                    atackedEnemy = enemyController.unitArray[Random.Range(0, enemyController.unitArray.Count)].transform;
                }

                await currentUnit.DOMove(atackedEnemy.position, actionTime / 2).AsyncWaitForCompletion();
                await currentUnit.DOMove(startPosition, actionTime / 2).AsyncWaitForCompletion();
            }

        }

        //Debug.Log("next");
        //Debug.Log(indexArray[0] + "," + indexArray[1] + "," + indexArray[2]);
    }
    async private void UnitKickOnce() {

        int index = 0;
        if (unitArray.Count > maxFightingUnit) {
            index = Random.Range(0, maxFightingUnit);
        }
        else {
            index = Random.Range(0, unitArray.Count);
        }
        Transform currentUnit = unitArray[index].transform;

        if (!nowAttacked.Contains(currentUnit.gameObject))  {
            nowAttacked.Add(currentUnit.gameObject);

            Vector3 startPosition = currentUnit.position;
            animators[index].SetTrigger(ATTACK);

            Transform atackedEnemy;

                int enemyIndex = 0;
            if (enemyController.unitArray.Count > enemyController.maxFightingUnit) {
                enemyIndex = Random.Range(0, enemyController.maxFightingUnit);
            }
            else {
                enemyIndex = Random.Range(0, enemyController.unitArray.Count);
            }
                atackedEnemy = enemyController.unitArray[enemyIndex].transform;

            

            Vector3 halfPosition = new Vector3((currentUnit.position.x + atackedEnemy.position.x) / 2, (currentUnit.position.y + atackedEnemy.position.y) / 2, (currentUnit.position.z + atackedEnemy.position.z) / 2);

            await currentUnit.DOMove(halfPosition, actionTime / 4f).AsyncWaitForCompletion();
            enemyController.GetDamage(enemyIndex);
            await currentUnit.DOMove(startPosition, actionTime / 1.5f).AsyncWaitForCompletion();



            if(currentUnit != null){
            nowAttacked.Remove(currentUnit.gameObject);
            }

        }
    }


    private void EnemyDamage() {
        currentTriggerCoef = AttackTriggerCoef();
       
        float min = 0f - (enemyController.defenceSquad*0.1f) - (enemyController.defenceCoef) - ((enemyController.currentStamina / enemyController.maxStamina)*100f*0.02f);

        if(enemyController.defence){
            min -= (enemyController.actionUnits/25f)*3f;
        }

        float max = 10f + ((powerSquad-enemyController.defenceSquad)*1.5f)+attackCoef +  currentTriggerCoef + ((actionUnits/25f)*3f)*((currentStamina / maxStamina)*100f*0.02f);
       
         Debug.Log("min "+min +" max " + max);

        int index;
        if (enemyController.unitArray.Count > enemyController.maxFightingUnit) {
            index = Random.Range(0, enemyController.maxFightingUnit);
        } else {
            index = Random.Range(0, enemyController.unitArray.Count);
        }


            float r = Random.Range(min, max);

                        Debug.Log("Attack "+ gameObject.name +"\n"+ 
            "Random = " + r
                        );

            if (r > 8.6f) {                
               
                enemyController.DieUnit(index);
                coinsController.ChangeAmountOfCoins(coinsFromDeath);


            } else {
                //enemyController.GetDamage(index);
            }
                 if (currentStamina > 0) {
                    currentStamina -= 0.05f;
                }
    }

    public void DieUnit(int index) {
        currentMorale -= lostMoraleThenDie;
        GameObject currentUnit = unitArray[index];

// backUnitList.Remove(currentUnit.transform);
// leftUnitList.Remove(currentUnit.transform);
// rightUnitList.Remove(currentUnit.transform);

        Transform currentModel = currentUnit.transform.GetChild(0);
        Instantiate(bloodFx, currentModel);
        currentModel.parent = null;
        unitArray.RemoveAt(index);
        animators[index].SetTrigger(DIE);
        animators.RemoveAt(index);
        Destroy(currentModel.gameObject, deadTime);

        TryToRetreat();
    }

    private void TryToRetreat() {
        retreatChanceCount = retreatChanceDefault + (1 - currentMorale / maxMorale);
        if (unitArray.Count < retreatChanceCount && Random.Range(0, retreatChanceCount) > retreatCount + retreatCoef) {
            SetBattle(false);
            
            for(int i = 0; i < unitArray.Count;i++) {
                Transform currentModel = unitArray[i].transform.GetChild(0);
                currentModel.gameObject.AddComponent<Rigidbody>();
                currentModel.gameObject.AddComponent<SphereCollider>();
                currentModel.GetComponent<UnitRetreatController>().enabled = true;
                currentModel.parent = null;
            }

            SquadDie();
        } else if (unitArray.Count == 0) {
            SquadDie();
        }

    }

    private void SquadDie() {

        if (enemyController) { enemyController.SetBattle(false); }

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

    public void GetDamage(int index) {
        if (Random.Range(1, 11) % 2 == 0) { 
            animators[index].SetTrigger("GetDamage");
        } else {
            animators[index].SetTrigger("GetDamage2");
        }
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
                Debug.Log(gameObject.name + " AttackTriggerCoef", hitCollider.gameObject);
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
     public void SidesUnitsListSet() {

       
        List<Transform> prewListB = new List<Transform>();
        List<Transform> prewListR = new List<Transform>();
        List<Transform> prewListL = new List<Transform>();


        for (int i = 0; i < unitArray.Count; i++) {
                            prewListB.Add(unitArray[i].transform);
                            prewListR.Add(unitArray[i].transform);
                            prewListL.Add(unitArray[i].transform);
                        }


     Transform  prewB = prewListB[0];
        Transform  prewR = prewListR[0];
        Transform  prewL = prewListL[0];

            for (int j = 0; j < 5; j++) {

        prewB = prewListB[0];
        prewR = prewListR[0];
        prewL = prewListL[0];

                for (int i = 0; i < prewListB.Count-1; i++) {
                    if(prewListB[i+1].localPosition.x < prewB.localPosition.x ){
                        prewB = prewListB[i+1];
        
                    }
                    
                }

                prewListB.Remove(prewB);
                    backUnitList.Add(prewB);

                for (int i = 0; i < prewListR.Count-1; i++) {
                    if(prewListR[i+1].localPosition.z < prewR.localPosition.z ){
                        prewR = prewListR[i+1];
                    }
                    
                }
                prewListR.Remove(prewR);
                    rightUnitList.Add(prewR);

        for (int i = 0; i < prewListL.Count-1; i++) {
                    if(prewListL[i+1].localPosition.z > prewL.localPosition.z ){
                        prewL = prewListL[i+1];
                        
                    }
                    
        }
                prewListL.Remove(prewL);
                    leftUnitList.Add(prewL);    

            }
            }

}


