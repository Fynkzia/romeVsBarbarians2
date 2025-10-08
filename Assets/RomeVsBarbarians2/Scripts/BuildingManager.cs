using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{

    [SerializeField] private bool playerBuilding;
    [SerializeField] private bool inGroupBuilding;
    [SerializeField] public bool isDestroed;

    [SerializeField] private SquadSpawnerController spawnerController;
    [SerializeField] private SmallCityController smallCityController;

    [SerializeField] public float hitPoints;

    [SerializeField] public float buildingSize;

    [SerializeField] private bool shootingBuilding;
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
    [Header("Economic Spec")]
    [SerializeField] int coinCost;

    [Space(10)]
    [Header("Shooting Setup")]
    [SerializeField] float shotStartOffset;
    [SerializeField] float shotSpawnDelay;
    [SerializeField] float shotHalfDelay;
    [SerializeField] private GameObject pfArrow;
    [SerializeField] private GameObject shootingTrigger;

    [SerializeField] private GameObject shootingIndicator;

    [Space(10)]
    [Header("FX Setup")]
    [SerializeField] private GameObject[] damageFx;
    [SerializeField] private AddCoinsEffect coinFx;

    [SerializeField] private float shakeDuration = 1f; // Длительность тряски
    [SerializeField] private float shakeIntensity = 1f; // Интенсивность тряски
    [SerializeField] private float decayRate = 1f;

    [SerializeField] private GameObject meshObject;
    [SerializeField] private GameObject destroyObject;

    private float currentShakeDuration; // Оставшееся время тряски
    private Vector3 originalPosition; // Исходное положение объекта

    int hitFxAmount;

    private SphereCollider shotCollider;
    private Collider currentEnemy;
    private ShotRangeManager shotRangeManager;
    private float rapidityTimer;
    
    int hitsAmount;

    

    // Start is called before the first frame update
    void Start()
    {
        if (shootingBuilding)
        {
            rapidityTimer = shotRapidity;
            shotCollider = shootingTrigger.GetComponent<SphereCollider>();
            shotCollider.radius = shotRange;
            shotRangeManager = shootingTrigger.GetComponent<ShotRangeManager>();
        }

        originalPosition = meshObject.transform.localPosition;
        currentShakeDuration = 0 ;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDestroed)
        {
            if (shootingBuilding)
            {
                if (shotAmount > 0)
                {

                    if (rapidityTimer >= shotRapidity)
                    {


                        if (currentEnemy == null)
                        {
                            ShotNearest();
                        }
                        else
                        {
                            Shot(currentEnemy);
                        }



                        rapidityTimer = 0;
                    }
                    else
                    {
                        rapidityTimer += Time.deltaTime;
                    }
                }
            }

            if (currentShakeDuration > 0)
            {
                // Рассчитываем смещение тряски
                float damping = Mathf.Clamp01(currentShakeDuration / shakeDuration);
                float offsetX = Random.Range(-1f, 1f) * shakeIntensity * damping;
                float offsetY = Random.Range(-1f, 1f) * shakeIntensity * damping;

                // Применяем смещение к объекту
                meshObject.transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, 0f);

                // Уменьшаем время тряски с учетом затухания
                currentShakeDuration -= Time.deltaTime * decayRate;

                // Если тряска закончилась, возвращаем объект на исходную позицию
                if (currentShakeDuration <= 0)
                {
                    meshObject.transform.localPosition = originalPosition;
                }
            }
        }
    }

    public void Build()
    {
        isDestroed = false;
        gameObject.SetActive(true);

       
    }

    public void SetOwner( bool isPlayer)
    {
        if (isPlayer)
        {
            playerBuilding = true;
            gameObject.tag = "Player";
        }
        else
        {
            playerBuilding = false;
            gameObject.tag = "Enemy";
        }
        


    }

    public void GetDamage(float damage)
    {

        if (!isDestroed)
        {
            hitPoints -= damage;

            hitsAmount++;

            //Debug.Log("Build GetDamage : " + damage);

            currentShakeDuration = shakeDuration;

            if (hitPoints <= 0)
            {

                //squadController.SetBattle(false);
                //Destroy(gameObject);
                //meshObject.gameObject.SetActive(false);
                //destroyObject.gameObject.SetActive(true);



                //destroyObject.transform.parent = null;

                if (coinCost > 0)
                {
                    AddCoinsEffect coins = Instantiate(coinFx, transform.parent.parent.transform);

                    coins.transform.position = transform.position;

                    coins.SetEffect(coinCost);
                }


                

                if (inGroupBuilding)
                {
                    smallCityController.BuildDestroy(this);

                    spawnerController.battleSceneManager.resourceManager.ChangeAmountOfCoins(coinCost);
                }
                else
                {
                    ResourceManager resourceManager = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();
                    resourceManager.ChangeAmountOfCoins(coinCost);
                }

                isDestroed = true;
                //GetComponent<BoxCollider>().enabled = false;
                //Destroy(gameObject,2f);

                gameObject.SetActive(false);
                GameObject destroy = Instantiate(destroyObject, transform.position,transform.rotation,transform.parent);
                destroy.SetActive(true);

            }
            else
            {
                GameObject fx = Instantiate(damageFx[Random.Range(0, damageFx.Length)], transform);
                fx.SetActive(true);
            }
        }
    }




    private void Shot(Collider predictEnemy)
    {


        StartCoroutine(SpawnShotAfterDelay(predictEnemy));

    }

    IEnumerator SpawnShotAfterDelay(Collider predictEnemy)
    {

        Vector3 enemyPosition = predictEnemy.gameObject.transform.position;
        float distance = Vector3.Distance(transform.position, enemyPosition);

      

           

            yield return new WaitForSeconds(shotSpawnDelay);

         

            ShotMovement.Create(pfArrow, transform.position + new Vector3(0, shotStartOffset, 0), enemyPosition, shotSpeed, projectilesPerShotCount, shotDamage, (distance / shotRange), gameObject.tag, buildingSize);
       
        
    }


    private void ShotNearest()
    {
        Collider tMin = null;
        float minDist = Mathf.Infinity;
        Vector3 currentPos = transform.position;
        int indexToRemove = 0;
        foreach (Collider t in shotRangeManager.enemyColliders)
        {
            if (t == null)
            {
                shootingIndicator.SetActive(false);
                shotRangeManager.enemyColliders.RemoveAt(indexToRemove);
                return;
            }
            float dist = Vector3.Distance(t.gameObject.transform.position, currentPos);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
            indexToRemove++;
        }


        if (tMin != null)
        {
            currentEnemy = tMin;
            Shot(tMin);

            shootingIndicator.SetActive(true);

        }
        else
        {
            shootingIndicator.SetActive(false);
        }
    }

    public void GetNewTargets()
    {


            
                if (shootingBuilding)
                {

                    if (currentEnemy == null)
                    {
                        ShotNearest();
                    }
                    else
                    {
                       // Shot(currentEnemy);
                    }

                 }
            

        
        currentEnemy = null;

    }
}
