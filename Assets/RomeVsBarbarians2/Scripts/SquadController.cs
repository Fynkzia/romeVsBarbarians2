using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class SquadController : MonoBehaviour
{
    [SerializeField] public List<GameObject> unitArray;
    [HideInInspector] public bool isMoved = false;
    [HideInInspector] public LineRenderer lineRenderer;
    [HideInInspector] public List<Animator> animators = new List<Animator>();
    //[SerializeField] private GameObject yourPointHeh;


    [HideInInspector] public bool isStopRot = false;

    // [SerializeField] private float debugAngel;


    [Header("Movement Settings")]
    [Space(10)]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float boostSpeed;
    [SerializeField] public float maxStamina;
    [SerializeField] private float recoveryStamina;
    [SerializeField] private float consumptionStamina;
    [SerializeField] private float pointsDistance; 
    [SerializeField] public float currentStamina;
    [SerializeField] private float maxDeltaAngel;

    [Header("Fighting Settings")]
    [Space(10)]
    [SerializeField] private float deadTime;
    [SerializeField] private bool playerSquad;
    [SerializeField] private bool inBattle;
    [SerializeField] private float levelSquad;
    [SerializeField] private float amountUnits;
    [SerializeField] public float powerSquad;
    [SerializeField] public float defenceSquad;
    [SerializeField] private float moraleSquad;
    [SerializeField] private float actionTime;
    [SerializeField] private int actionUnits;
    [SerializeField] private bool attack;
    [SerializeField] private bool defence;

    private Rigidbody rb;
    private GameObject enemySquad;
    private SquadController enemyController;
    private Vector3[] positions;

    private int index = 0;
    private float currentSpeed = 0f;
    private float battleTime = 0f;
    private bool battleRot = false;

    private const string IS_MOVING = "Moving";
    private const string IS_BATTLE = "Battle";
    private const string ATTACK = "Attack";
    private const string DIE = "Die";
    private const string RANDOM_SPEED = "RandomSpeed";
    private const string ENEMY_TAG = "Enemy";
    private const string SQUAD_TAG = "Squad";

    private void Awake() {
        for (int i = 0; i < unitArray.Count; i++) {
            animators.Add(unitArray[i].transform.GetChild(0).GetComponent<Animator>());
        }
        currentStamina = maxStamina;
        battleTime = actionTime;
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
    }

    private void FixedUpdate() {
        if (isMoved) {
            SquadMovement();
            if (currentStamina > 0) { currentStamina -= consumptionStamina * Time.fixedDeltaTime; }
        } else if(currentStamina < maxStamina) {
            currentStamina += recoveryStamina * Time.fixedDeltaTime;
        }

        if (inBattle) {
            battleTime += Time.fixedDeltaTime;
            if (!battleRot) { 
                LookOnEnemy(); 
            } else {
                if (battleTime >= actionTime) {
                    battleTime = 0f;
                    UnitKick(enemySquad.transform);
                    EnemyDamage();
                }
            
            }

        }

    }
    private void SquadMovement() {
        if (index < lineRenderer.positionCount) {
            positions = new Vector3[(int)lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);

            Vector3 linePosition = new Vector3(lineRenderer.GetPosition(0).x, transform.position.y, lineRenderer.GetPosition(0).z);
            Vector3 targetPos = Vector3.MoveTowards(transform.position, linePosition, CurrentSpeed() * Time.fixedDeltaTime);

            float y = AngleBetweenTwoPoints(transform.position, lineRenderer.GetPosition(0)) ;
            float deltaAngel = Math.Abs(transform.rotation.eulerAngles.y - y);
            //Debug.Log(deltaAngel);
            if (!isStopRot) {
                if (deltaAngel > maxDeltaAngel && deltaAngel < 360 - maxDeltaAngel) {
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)), rotationSpeed * Time.fixedDeltaTime);
                    Debug.Log("Stop");
                    isStopRot = true;
                    currentSpeed = 0;
                    //
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
                index++;
            }

            if (isStopRot) {
                transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)), rotationSpeed * Time.fixedDeltaTime);

                if(deltaAngel < 4) {
                    isStopRot = false;
                }

            }
        }
        else {
            CancelMovement();
        }
    }

    public void SetMoving(bool isMoved) {
        SquadControlManager.Instance.isSquadMoving = isMoved;
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

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return 180f - Mathf.Atan2(a.z - b.z, a.x - b.x) * Mathf.Rad2Deg;
    }

    private float CurrentSpeed() {
        float currentMoveSpeed = movementSpeed * StaminaEffectIndex();
        float currentBoostSpeed = boostSpeed * StaminaEffectIndex();
        if (currentSpeed < currentMoveSpeed) {
            currentSpeed += currentBoostSpeed * Time.fixedDeltaTime;
        } else {
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
        index = 0;
        currentSpeed = 0;
        positions = null;
    }

    private void OnTriggerEnter(Collider enemyCollider) {
        enemySquad = enemyCollider.gameObject;
        if ((tag==SQUAD_TAG && enemySquad.tag == ENEMY_TAG) || (tag == ENEMY_TAG && enemySquad.tag == SQUAD_TAG)) {
            enemyController = enemyCollider.transform.GetComponent<SquadController>();
            SetBattle(true);
            movementSpeed /= 2;
            rotationSpeed /= 2;
            boostSpeed /= 2;
            if(isMoved) {
                CancelMovement();
            }
        }
    }
    private void OnTriggerStay(Collider other) {
        
    }

    private void OnTriggerExit(Collider other) {
        if ((tag == SQUAD_TAG && enemySquad.tag == ENEMY_TAG) || (tag == ENEMY_TAG && enemySquad.tag == SQUAD_TAG)) {
            SetBattle(false);
            movementSpeed *= 2;
            rotationSpeed *= 2;
            boostSpeed *= 2;
        }
    }

    private void LookOnEnemy() {
        float y = AngleBetweenTwoPoints(transform.position, enemySquad.transform.position);
        //float b = AngleBetweenTwoPoints(enemySquad.transform.position, transform.position);
            transform.rotation = Quaternion.LerpUnclamped(transform.rotation, Quaternion.Euler(new Vector3(0f, y, 0f)), rotationSpeed * Time.fixedDeltaTime);
            //enemySquad.transform.rotation = Quaternion.LerpUnclamped(enemySquad.transform.rotation, Quaternion.Euler(new Vector3(0f, b, 0f)), rotationSpeed * Time.fixedDeltaTime);

        if (Mathf.Abs(transform.rotation.eulerAngles.y  - y) < 3) {
            battleRot = true;
        }
    }

    async private void UnitKick(Transform enemyTransform) {

        for (int i = 0; i < actionUnits; i++) {
            //int index = Random.Range(0, unitArray.Count - 1);
            int index = 0;
            Transform currentUnit = unitArray[index].transform;
            Vector3 startPosition = currentUnit.position;
            animators[index].SetTrigger(ATTACK);

            await currentUnit.DOMove(enemyTransform.position + new Vector3(0, (int)Random.Range(-1, 2), 0), actionTime / 2).AsyncWaitForCompletion();
            await currentUnit.DOMove(startPosition, actionTime / 2).AsyncWaitForCompletion();
        }
    }

    private void EnemyDamage() {
        //float attack = powerSquad * (currentStamina/maxStamina);
        //float defence = enemyController.defenceSquad;
        //float minDamage = 
        if (Random.Range(0, 11) % 2 == 0) {
            //int index = Random.Range(0, enemyController.unitArray.Count - 1);
            int index = 0;
            GameObject currentUnit = enemyController.unitArray[index];
            Transform currentModel = currentUnit.transform.GetChild(0);
            currentModel.parent = null;
            enemyController.unitArray.RemoveAt(index);
            enemyController.animators[index].SetTrigger(DIE);
            enemyController.animators.RemoveAt(index);
            Destroy(currentModel.gameObject, deadTime);
        
        }
    }
}
