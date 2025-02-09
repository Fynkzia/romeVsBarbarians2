using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{

    [Space(10)]
    [Header("Shooting Type")]
    [SerializeField] bool isAttackShooting;
    [SerializeField] bool isShootingSquad;
    [SerializeField] bool isMovementShooting;
    [Space(10)]

    [Header("Shooting Spec")]
    [SerializeField] float shotRange;
    [SerializeField] int shotAmount;
    [SerializeField] float shotSpeed;
    [SerializeField] float shotDamage;
    [SerializeField] float shotAccuracy;
    [SerializeField] float shotRapidity;
    [SerializeField] int projectilesPerShotCount;

    [Space(10)]
    [Header("Shooting Setup")]
    [SerializeField] float shotStartOffset;
    [SerializeField] float shotSpawnDelay;
    [SerializeField] float shotHalfDelay;
    [SerializeField] private GameObject pfArrow;

    [SerializeField]private SquadController squadController;
    [SerializeField] private GameObject shootingTrigger;


    private bool isFirstShoot;
    private SphereCollider shotCollider;
    private Collider currentEnemy;
    private ShotRangeManager shotRangeManager;
    private float rapidityTimer ;

    private void Start() {
        isFirstShoot = true;
        rapidityTimer = shotRapidity;
        squadController = GetComponent<SquadController>();
        

        
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
                    Debug.Log("AttackShooting");
                    isFirstShoot = false;
                }
            }
        } else {
           // isFirstShoot = true;
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

        if (distance > 10f)
        {

            AnimationController[] animationController = new AnimationController[(int)projectilesPerShotCount];

           

            squadController.animTime = 0;

            for (int i = 0; i < projectilesPerShotCount; i++)
            {
                int index = Random.Range(0, squadController.animatorControllers.Count);
                squadController.animatorControllers[index].SpriteAnimationChange(10);
                animationController[i] = squadController.animatorControllers[index];

            }

            yield return new WaitForSeconds(shotSpawnDelay);

            distance = Vector3.Distance(transform.position, enemyPosition);

            if (distance > 10f)
            {

                for (int i = 0; i < projectilesPerShotCount; i++)
                {

                    animationController[i].SpriteAnimationChange(11);


                }

                ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, projectilesPerShotCount, shotDamage, (distance / shotRange), gameObject.tag, squadController.TriggerObject.radius);
            }
            else
            {
                SquadControlManager controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();

                controlController.SquadWayToPoint(squadController, enemyPosition); // идем в рукопашную вместо атакаки
            }
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

     
        Vector3 enemyPosition = squadController.predictEnemy.gameObject.transform.position;

        yield return new WaitForSeconds(0.5f);

        

          int shootingUnitsCouns = (int)squadController.currentAmountUnits/3;


        for (int i = 0; i < shootingUnitsCouns; i++)
        {
            
            squadController.animatorControllers[i].SpriteAnimationChange(10);
           

        }

        yield return new WaitForSeconds(shotSpawnDelay);
        float distance = Vector3.Distance(transform.position, enemyPosition);
        ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, shootingUnitsCouns, shotDamage, (distance / shotRange), gameObject.tag, squadController.TriggerObject.radius);

        yield return new WaitForSeconds(shotHalfDelay);
        ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, shootingUnitsCouns, shotDamage, (distance / shotRange), gameObject.tag, squadController.TriggerObject.radius);

        for (int i = 0; i < shootingUnitsCouns; i++)
        {

            squadController.animatorControllers[i].SpriteAnimationChange(11);


        }

        yield return new WaitForSeconds(1f);

        SquadControlManager controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();

        controlController.SquadWayToPoint(squadController, enemyPosition); // идем в рукопашную после выстрела атакаки

        squadController.shootingIndicator.SetActive(false);
    }

    private void ShotNearest() {
        Collider tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        foreach (Collider t in shotRangeManager.enemyColliders) {
            if(t == null)
            {
                squadController.shootingIndicator.SetActive(false);

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
            squadController.shootingIndicator.SetActive(true);

        }
        else
        {
            squadController.shootingIndicator.SetActive(false);
        }
    }

    public void GetNewTargets()
    {
       
        

        if (squadController.isGoingToEnemy)
        {
           
            if (isAttackShooting)
            {
                if (isFirstShoot)
                {
                    AttackShooting();
                    squadController.shootingIndicator.SetActive(true);
                }
            }
            else
            {
                if (isShootingSquad)
                {
                    squadController.CancelMovement();
                    ShootingSquad();
                    rapidityTimer = 0;

                    squadController.shootingIndicator.SetActive(true);

                    squadController.animTime = 0;

                    for (int i = 0; i < projectilesPerShotCount; i++) // по фану стразу ставим в анимацию замахивания
                    {
                        int index = Random.Range(0, squadController.animatorControllers.Count);
                        squadController.animatorControllers[index].SpriteAnimationChange(10);
                        

                    }
                }
            }

        }
        currentEnemy = null;

    }
}
