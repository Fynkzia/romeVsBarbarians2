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
    [SerializeField] float shotHalfDelay;
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

    IEnumerator SpawnShotAfterDelay(Collider predictEnemy  )
    {

        Vector3 enemyPosition = predictEnemy.gameObject.transform.position;
        float distance = Vector3.Distance(transform.position, enemyPosition);

        if (distance > 2f)
        {

            AnimationController[] animationController = new AnimationController[(int)actionUnits];

            actionUnits = squadController.actionUnits;

            squadController.animTime = 0;

            for (int i = 0; i < actionUnits; i++)
            {
                int index = Random.Range(0, squadController.animatorControllers.Count);
                squadController.animatorControllers[index].SpriteAnimationChange(9);
                animationController[i] = squadController.animatorControllers[index];

            }

            yield return new WaitForSeconds(shotSpawnDelay);

            for (int i = 0; i < actionUnits; i++)
            {

                animationController[i].SpriteAnimationChange(10);


            }

            ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, actionUnits, shotDamage, (distance / shotRange), gameObject.tag, squadController.colliderObject.radius);
        }
        else
        {
            yield return new WaitForSeconds(shotSpawnDelay);

            SquadControlManager controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();

            controlController.SquadWayToPoint(squadController, enemyPosition); // идем в рукопашную вместо атакаки

        }
    }

    IEnumerator AttackShootingSiqunce()
    {
        
        squadController.CancelMovement();

        actionUnits = squadController.unitArray.Count;
        Vector3 enemyPosition = squadController.predictEnemy.gameObject.transform.position;

        yield return new WaitForSeconds(0.5f);


        for (int i = 0; i < actionUnits; i++)
        {
            
            squadController.animatorControllers[i].SpriteAnimationChange(9);
           

        }

        yield return new WaitForSeconds(shotSpawnDelay);
        float distance = Vector3.Distance(transform.position, enemyPosition);
        ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, actionUnits / 2, shotDamage, (distance / shotRange), gameObject.tag, squadController.colliderObject.radius);

        yield return new WaitForSeconds(shotHalfDelay);
        ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, actionUnits/2, shotDamage, (distance / shotRange), gameObject.tag, squadController.colliderObject.radius);

        for (int i = 0; i < actionUnits; i++)
        {

            squadController.animatorControllers[i].SpriteAnimationChange(10);


        }

        yield return new WaitForSeconds(1f);

        SquadControlManager controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();

        controlController.SquadWayToPoint(squadController, enemyPosition); // идем в рукопашную после выстрела атакаки
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
                if (isShootingSquad)
                {
                    squadController.CancelMovement();
                    ShootingSquad();
                    rapidityTimer = 0;
                }
            }

        }
        currentEnemy = null;

    }
}
