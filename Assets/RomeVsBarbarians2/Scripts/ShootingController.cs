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
    private bool isFirstShoot;

    private void Start() {
        isFirstShoot = true;
        squadController = GetComponent<SquadController>();
    }
    private void Update() {
        if (shotAmount > 0) {
            if (isAttackShooting) {
                AttackShooting();
            }
        }
    }

    private void AttackShooting() {
        if (squadController.isGoingToEnemy) {
            if (isFirstShoot) {
                //if enemy in shotRange;
                Shot();
                isFirstShoot = false;
            }
        } else {
            isFirstShoot = true;
        }

    }

    private void Shot() {
        
        ShotMovement.Create(pfArrow, transform.position, squadController.predictEnemy.position, shotSpeed);
    }
}
