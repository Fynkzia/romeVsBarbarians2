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
    [SerializeField] int maxShotsAmount;
    [SerializeField] float shotSpeed;
    [SerializeField] float shotDamage;
    [SerializeField] float shotAccuracy;
    [SerializeField] float shotRapidity;
    [SerializeField] int projectilesPerShotCount;

    [SerializeField] float checkMoveTime;

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
    [SerializeField] private Collider currentEnemy;
    private ShotRangeManager shotRangeManager;
    private float rapidityTimer ;
    private float checkMoveTimer;

    public void InitBattle()
    {
        shotAmount = maxShotsAmount;
        squadController.squadInfo.AmmoUpdate(shotAmount, maxShotsAmount);
    }


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

            //checkMoveTimer += Time.deltaTime;

            //if (checkMoveTimer >= checkMoveTime && !squadController.inBattle)
            //{

            //    checkMoveTimer = 0;

            //    if (!squadController.isMoved)
            //    {
            //        if (isShootingSquad && squadController.predictEnemy != null)
            //        {


            //            float dist = Vector3.Distance(squadController.predictEnemy.transform.position, transform.position);

            //            if (dist > shotRange)
            //            {
            //                if (!squadController.isMoved)
            //                {
            //                    Debug.Log("GoToDirection " + dist);
            //                    squadController.GoToDirection(squadController.predictEnemy.transform.position, shotRange * 0.5f);
            //                }
            //            }
            //            else if (dist < shotRange * 0.9f)
            //            {
            //                if (squadController.isMoved)
            //                {
            //                    squadController.CancelMovement();
            //                    //Shot(squadController.predictEnemy);
            //                    // squadController.predictEnemy = null;

            //                    Debug.Log("dist stop!!! " + dist);
            //                }
            //            }
            //        }
            //    }
            //}


                if (rapidityTimer >= shotRapidity && !squadController.inBattle) {

                    if (isShootingSquad && !squadController.isMoved) {
                        ShootingSquad();
                    }

                    if (isMovementShooting )
                    {
                        ShootingSquad();
                    }
                    rapidityTimer = 0;

                    squadController.squadInfo.AmmoUpdate(shotAmount, maxShotsAmount);

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

            if(squadController.predictEnemy.gameObject.activeSelf == false)
            {
                squadController.predictEnemy = null;

                return;
            }

            if (shotRangeManager.enemyColliders.Contains(squadController.predictEnemy)) {
                //squadController.CancelMovement();
                //Shot(squadController.predictEnemy);


                float dist = Vector3.Distance(squadController.predictEnemy.transform.position, transform.position);

                if (dist > shotRange * 1.2f)
                {
                    //
                    //squadController.CancelMovement();

                    Debug.Log("ShootingSquad + dist > shotRange  GoToSquad!! " + dist);

                    squadController.GoToSquad(squadController.predictEnemy.transform);
                }
                else
                {
                    squadController.CancelMovement();
                    Shot(squadController.predictEnemy);

                    Debug.Log("ShootingSquad + CancelMovement + Shot  " + dist);
                    // squadController.predictEnemy = null;
                }


                //squadController.predictEnemy = null;
            }
            else
            {
                squadController.squadInfo.ShootingIndicator(false);

                if (squadController.predictEnemy != null)
                {
                    //ector3 pointToGo = (squadController.predictEnemy.transform.position + transform.position) / 2f;


                    squadController.GoToSquad(squadController.predictEnemy.transform);
                    Debug.Log("ShootingSquad + GoToSquad   " + squadController.predictEnemy.name );


                }
            }
        } else {
            if (currentEnemy == null || currentEnemy.gameObject.active == false) {
                ShotNearest();

                Debug.Log("ShotNearest   " );
            } else { 
                Shot(currentEnemy);
                Debug.Log("Shot   ");
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
                Debug.Log("Shot   distance > 10f");

                for (int i = 0; i < projectilesPerShotCount; i++)
                {

                    animationController[i].SpriteAnimationChange(11);


                }
                shotAmount--;
                ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, projectilesPerShotCount, shotDamage, (distance / shotRange), gameObject.tag, squadController.TriggerObject.radius);
            }
            else
            {
                squadController.GoToSquad(predictEnemy.transform);
            }
        }
        else
        {
            yield return new WaitForSeconds(shotSpawnDelay);



            squadController.GoToSquad(predictEnemy.transform);

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

        shotAmount--;

        squadController.GoToSquad(squadController.predictEnemy.transform); // идем в рукопашную после выстрела атакаки

        squadController.squadInfo.ShootingIndicator(false);


        if (shotAmount <= 0)
        {
            squadController.squadInfo.ammoBar.gameObject.SetActive(false);
        }
        else
        {

            squadController.squadInfo.AmmoUpdate(shotAmount, maxShotsAmount);
        }
    }

    private void ShotNearest() {
        Collider tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        int index = 0;

        foreach (Collider t in shotRangeManager.enemyColliders) {

            if (t == null)
            {
                shotRangeManager.enemyColliders.RemoveAt(index);
            }



            if (t.gameObject.activeSelf == false)
            {
                shotRangeManager.enemyColliders.RemoveAt(index);

                squadController.squadInfo.ShootingIndicator(false);
                if (t == squadController.predictEnemy)
                {
                    squadController.predictEnemy = null;
                }

                //return;
            }
            else
            {

                float dist = Vector3.Distance(t.gameObject.transform.position, currentPos);
                if (dist < minDist)
                {
                    tMin = t;
                    minDist = dist;
                }
            }

            index++;
        }

        if(tMin != null) {
            currentEnemy = tMin;
            Shot(tMin);
            squadController.squadInfo.ShootingIndicator(true);
           
        }
        else
        {
            
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
                    squadController.squadInfo.ShootingIndicator(true);
                }
            }
            else
            {
                if (isShootingSquad)
                {
                    if (shotAmount > 0)
                    {
                        if (squadController.isMoved && shotRangeManager.enemyColliders.Contains(squadController.predictEnemy))
                        {
                            squadController.CancelMovement();
                        }
                        // ShootingSquad();
                        //rapidityTimer = 0;

                        squadController.squadInfo.ShootingIndicator(true);

                        squadController.animTime = 0;

                        for (int i = 0; i < projectilesPerShotCount; i++) // по фану стразу ставим в анимацию замахивания
                        {
                            int index = Random.Range(0, squadController.animatorControllers.Count);
                            squadController.animatorControllers[index].SpriteAnimationChange(10);


                        }
                    }
                    else
                    {

                        Instantiate(squadController.noAmmoFx, transform.position, squadController.noAmmoFx.transform.rotation);
                    }
                }
            }

        }
        currentEnemy = null;

    }
}
