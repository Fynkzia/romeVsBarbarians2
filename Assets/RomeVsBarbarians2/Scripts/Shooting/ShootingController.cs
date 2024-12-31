using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    [SerializeField] bool isAttackShooting;
    [SerializeField] bool isShootingSquad;
    [SerializeField] bool isMovementShooting;
    [SerializeField] float shotRange;
    [SerializeField] int shotAmount;
    [SerializeField] float shotSpeed;
    [SerializeField] float shotDamage;
    [SerializeField] float shotAccuracy;
    [SerializeField] float shotRapidity;
    [SerializeField] float shotStartOffset;
    [SerializeField] float shotSpawnDelay;
    [SerializeField] private GameObject pfArrow;

    [SerializeField]private SquadController squadController;
    [SerializeField] private GameObject shootingTrigger;
    private float actionUnits;

    private bool isFirstShoot;
    private SphereCollider shotCollider;
    private Collider currentEnemy;
    private ShotRangeManager shotRangeManager;
    private float rapidityTimer ;

    private void Start() {
        isFirstShoot = true;
        rapidityTimer = shotRapidity;
        squadController = GetComponent<SquadController>();
        actionUnits = squadController.actionUnits;

        
            shotCollider = shootingTrigger.GetComponent<SphereCollider>();
            shotCollider.radius = shotRange;
            shotRangeManager = shootingTrigger.GetComponent<ShotRangeManager>();
        

        
    }
    private void Update() {

        if (isShootingSquad && squadController.isMoved)
        {
            if (squadController.predictEnemy != null)
            {
                if (shotRangeManager.enemyColliders.Contains(squadController.predictEnemy))
                {
                    squadController.CancelMovement();

                }
            }

        }

        if (shotAmount > 0) {
           
            if (rapidityTimer >= shotRapidity && !squadController.inBattle) {

                if (isShootingSquad && !squadController.isMoved) {
                    ShootingSquad();
                }

                if (isMovementShooting && squadController.isMoved)
                {
                    ShootingSquad();
                }
                rapidityTimer = 0;
            }
            else {
                rapidityTimer += Time.deltaTime;
            }
        }
    }

    private void AttackShooting() {
        if (squadController.isGoingToEnemy) {
            if (isFirstShoot) {
                
                if (shotRangeManager.enemyColliders.Contains(squadController.predictEnemy)) {
                    StartCoroutine(AttackShootingSiqunce());
                   
                    isFirstShoot = false;
                }
            }
        } else {
            isFirstShoot = true;
        }

    }

    private void ShootingSquad() {
        if (squadController.predictEnemy != null) {
            if (shotRangeManager.enemyColliders.Contains(squadController.predictEnemy)) {
                squadController.CancelMovement();
                Shot(squadController.predictEnemy);
            }
        } else {
            if (currentEnemy == null) {
                ShotNearest();
            } else { 
                Shot(currentEnemy);
            }
        }
    }

    private void Shot(Collider predictEnemy) {
       

        StartCoroutine(SpawnShotAfterDelay(predictEnemy));

    }

    IEnumerator SpawnShotAfterDelay(Collider predictEnemy)
    {
        actionUnits = squadController.actionUnits;
        Vector3 enemyPosition = predictEnemy.gameObject.transform.position;

        for (int i = 0; i < actionUnits; i++)
        {
            int index = Random.Range(0, squadController.unitArray.Count);
            squadController.SetShooting(index);

            Vector3 directionToEnemy = enemyPosition - transform.position;
            directionToEnemy.y = 0; // Оставляем только горизонтальную компоненту направления
            squadController.unitArray[index].transform.rotation = Quaternion.LookRotation(directionToEnemy) * Quaternion.EulerAngles(0f, 90f, 0f);
        }

        yield return new WaitForSeconds(shotSpawnDelay);
        

        ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, actionUnits,shotDamage,shotAccuracy,gameObject.tag);
    }

    IEnumerator AttackShootingSiqunce()
    {
        
        squadController.CancelMovement();

        actionUnits = squadController.unitArray.Count;
        Vector3 enemyPosition = squadController.predictEnemy.gameObject.transform.position;

        for (int i = 0; i < actionUnits; i++)
        {
            int index = Random.Range(0, squadController.unitArray.Count);
            squadController.SetShooting(index);

            Vector3 directionToEnemy = enemyPosition - transform.position;
            directionToEnemy.y = 0; // Оставляем только горизонтальную компоненту направления
            squadController.unitArray[index].transform.rotation = Quaternion.LookRotation(directionToEnemy) * Quaternion.EulerAngles(0f, -90f, 0f);
        }

        yield return new WaitForSeconds(shotSpawnDelay);
        ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, actionUnits, shotDamage, shotAccuracy, gameObject.tag);

        yield return new WaitForSeconds(0.5f);

        SquadControlManager controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();

        controlController.SquadWayToPoint(squadController, enemyPosition);
    }

    private void ShotNearest() {
        Collider tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Collider t in shotRangeManager.enemyColliders) {
            if(t == null)
            {
                return;
            }
            float dist = Vector3.Distance(t.gameObject.transform.position, currentPos);
            if (dist < minDist) {
                tMin = t;
                minDist = dist;
            }
        }
        if(tMin != null) {
            currentEnemy = tMin;
            Shot(tMin); 
        }
    }

    public void GetNewTargets()
    {
       
        

        if (squadController.isGoingToEnemy)
        {
           
            if (isAttackShooting)
            {
                AttackShooting();
                
            }
            else
            {
                squadController.CancelMovement();
                ShootingSquad();
                rapidityTimer = 0;
            }

        }
        currentEnemy = null;

    }
}
