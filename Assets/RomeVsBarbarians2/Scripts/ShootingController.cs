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
    [SerializeField] private GameObject pfArrow;

    [SerializeField]private SquadController squadController;
    private float actionUnits;

    private bool isFirstShoot;
    private SphereCollider shotCollider;
    private Collider currentEnemy;
    private GameObject shotSphere;
    private ShotRangeManager shotRangeManager;
    private float rapidityTimer ;

    private void Start() {
        isFirstShoot = true;
        rapidityTimer = shotRapidity;
        squadController = GetComponent<SquadController>();
        actionUnits = squadController.actionUnits;

        shotSphere = new GameObject();
        shotSphere.transform.parent = transform;
        shotCollider = shotSphere.AddComponent<SphereCollider>();
        shotCollider.radius = shotRange;
        shotCollider.isTrigger = true;

        shotRangeManager = shotSphere.AddComponent<ShotRangeManager>();
    }
    private void Update() {
        if (rapidityTimer >= shotRapidity) {
            if (shotAmount > 0) {
                if (isAttackShooting) {
                    AttackShooting();
                }
                if (isShootingSquad) {
                    ShootingSquad();
                }
            }

            rapidityTimer = 0;
        } else{
            rapidityTimer += Time.deltaTime;
        }
    }

    private void AttackShooting() {
        if (squadController.isGoingToEnemy) {
            if (isFirstShoot) {
                if (shotRangeManager.enemyColliders.Contains(squadController.predictEnemy)) { 
                    Shot(squadController.predictEnemy);
                    isFirstShoot = false;
                }
            }
        } else {
            isFirstShoot = true;
        }

    }

    private void ShootingSquad() {
        if (squadController.isGoingToEnemy) {
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
        Vector3 enemyPosition = predictEnemy.gameObject.transform.position;
        ShotMovement.Create(pfArrow, transform.position, enemyPosition, shotSpeed, actionUnits);
    }

    private void ShotNearest() {
        Collider tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Collider t in shotRangeManager.enemyColliders) {
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
}
