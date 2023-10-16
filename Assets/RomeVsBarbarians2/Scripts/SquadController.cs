using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SquadController : MonoBehaviour
{
    [SerializeField] private float movementSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float boostSpeed;
    [SerializeField] private GameObject[] unitArray;
    [SerializeField] private float maxStamina;
    [SerializeField] private float recoveryStamina;
    [SerializeField] private float consumptionStamina;

    [SerializeField] private float pointsDistance;
    

    public bool isMoved = false;
    public LineRenderer lineRenderer;

    private int index = 0;
    private Rigidbody rb;
    private List<Animator> animators = new List<Animator>();
    private Vector3[] positions;
    private const string IS_MOVING = "Moving";
    private const string RANDOM_SPEED = "RandomSpeed";
    private float currentSpeed = 0f;
    [SerializeField] private float currentStamina;

    private void Awake() {
        for (int i = 0; i < unitArray.Length; i++) {
            animators.Add(unitArray[i].transform.GetChild(0).GetComponent<Animator>());
        }
        currentStamina = maxStamina;
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

        
    }
    private void SquadMovement() {
        if (index < lineRenderer.positionCount) {
            positions = new Vector3[(int)lineRenderer.positionCount];
            lineRenderer.GetPositions(positions);

            Vector3 linePosition = new Vector3(lineRenderer.GetPosition(0).x, transform.position.y, lineRenderer.GetPosition(0).z);
            Vector3 targetPos = Vector3.MoveTowards(transform.position, linePosition, CurrentSpeed() * Time.fixedDeltaTime);

            float y = AngleBetweenTwoPoints(transform.position, lineRenderer.GetPosition(0)) ;
           

            transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
            rb.MovePosition(targetPos);

            if (Vector3.Distance(rb.position, linePosition) < pointsDistance) {
                var pointsList = new List<Vector3>(positions);
                pointsList.RemoveAt(0);
                positions = pointsList.ToArray();
                lineRenderer.SetPositions(positions);
                index++;
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
}
