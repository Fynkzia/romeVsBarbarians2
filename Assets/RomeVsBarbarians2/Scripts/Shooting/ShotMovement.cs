using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMovement : MonoBehaviour
{


    public static void Create(GameObject pfArrow,Vector3 spawnPosition,Vector3 target, float speed, float arrowsAmount, float damage, float accuracy, string tag, float radius) {
        float y = MathUtilities.AngleBetweenTwoPoints(spawnPosition, target);
        GameObject shot = Instantiate(pfArrow, spawnPosition, Quaternion.Euler(new Vector3(0f, y, 0f)));
        //Debug.Log(shot.transform.position);
        ShotMovement shotMovement = shot.GetComponent<ShotMovement>();
        shotMovement.Setup(target, speed, arrowsAmount, damage, accuracy,tag, radius);
        

        //Debug.Log("Create", shot);

    }


    public float speed;
    public Vector3 target;
    public float damage;
    public float accuracy;
    
    public float arrowsAmount;
    public float radius;

    public float randomOffest;

    [SerializeField] private GameObject pfVisual;
    [SerializeField] private GameObject fxVisual;
    [SerializeField] private Vector3 offsetPosition;
    [SerializeField] public LayerMask shotMask;

    private Vector3 _startPosition;
    private float _stepScale;
    private float _progress;
    [SerializeField] private float arcHeight = 3;
    [SerializeField] private float offsetArrivedArrows = 0;
    private float distance;
    private bool shown = false;
    private bool arrived = false;

    private const string ENEMY_TAG = "Enemy";
    private const string SQUAD_TAG = "Squad";


    private void Setup(Vector3 target, float speed, float arrowsAmount,float damage, float accuracy, string tag, float radius) {
        _startPosition = transform.position;

        distance = Vector3.Distance(_startPosition, target);
        this.target = target + new Vector3 (Random.Range(-randomOffest, randomOffest) *  accuracy , 0, Random.Range(-randomOffest, randomOffest) *  accuracy); //не точность, чем дальше тем неточнее прилетает весь объект выстрела
        this.speed = speed;
        this.arrowsAmount = arrowsAmount;
        this.damage = damage;
        this.accuracy = accuracy;
        gameObject.tag = tag;

        this.radius = radius;
        offsetPosition = new Vector3(radius/1.8f, radius/5f , radius / 1.8f);

        CreateVisual();

        Debug.Log(target);

        // This is one divided by the total flight duration, to help convert it to 0-1 progress.
        _stepScale = speed / distance;
    }
    private void CreateVisual() {
        for (int i = 0; i < arrowsAmount; i++) {
           GameObject shotVisual= Instantiate(pfVisual, MathUtilities.RandomOffset(transform.position, offsetPosition), pfVisual.transform.rotation, transform);
            shotVisual.transform.localRotation = pfVisual.transform.rotation;
            shotVisual.gameObject.SetActive(false);
        }
    }
    private void Update() {

        if (!arrived)
        {
            float speedCoef = 1f + (_progress - 0.5f) * (_progress - 0.5f) * 2;
            // Increment our progress from 0 at the start, to 1 when we arrive.
            _progress = Mathf.Min(_progress + Time.deltaTime * _stepScale * speedCoef, 1.0f);

            // Turn this 0-1 value into a parabola that goes from 0 to 1, then back to 0.






            // Travel in a straight line from our start position to the target.

            Vector3 nextPos = Vector3.Lerp(_startPosition, target, _progress);

            float parabola = 0;
            // Then add a vertical arc in excess of this.
            if (distance > 20f)
            {
                parabola = 1.0f - 20.0f * (_progress - 0.5f) * (_progress - 0.5f);
            }
            else
            {
                parabola = 0.3f - 9.0f * (_progress - 0.5f) * (_progress - 0.5f);

            }
            

            nextPos.y += parabola + (arcHeight * (distance / 50f));
       

            
            if (_progress > 0.01f)
            {
                if (!shown)
                {
                    shown = true;
                    for (int i = 0; i < arrowsAmount; i++)
                    {

                        transform.GetChild(i).gameObject.SetActive(true);
                    }

                }
                else { }
            }

            Vector3 rot = nextPos - transform.position;
            transform.rotation = Quaternion.LookRotation(rot.normalized);
            transform.position = nextPos;



            if (_progress == 1.0f)
            {
                Damage();
                Destroy(gameObject, 5f);

                ShotArrived();
                arrived = true;


            }
        }
    }

    private void ShotArrived()
    {
        transform.position = new Vector3(transform.position.x, target.y, transform.position.z);
        transform.rotation = Quaternion.Euler(0f, transform.rotation.y, 0f);

        for (int i = 0; i < arrowsAmount; i++)
        {
            Transform arrow = transform.GetChild(i);

            arrow.rotation = Quaternion.Euler(Random.Range(100f,180f),0,0);
            arrow.localPosition = new Vector3(arrow.localPosition.x, 0 + offsetArrivedArrows, arrow.localPosition.z);
        }
        GameObject arrivedFX = Instantiate(fxVisual, transform.position, fxVisual.transform.rotation,transform);

        arrivedFX.transform.localScale = new Vector3(radius/1.5f, radius / 1.5f, radius / 1.5f);
    }

        private void Damage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, shotMask);

        

        foreach (var hitCollider in hitColliders)
        {


            if ((tag == SQUAD_TAG && hitCollider.gameObject.tag == ENEMY_TAG) || (tag == ENEMY_TAG && hitCollider.gameObject.tag == SQUAD_TAG))
            {

                SquadController enController = hitCollider.transform.parent.GetComponent<SquadController>();

                for (int i = 0; i < arrowsAmount/ hitColliders.Length; i++) // равномерно распределятеся по всем отрядам
                {
                    float min = 0f - (enController.defenceSquad * 0.1f) - (enController.defenceCoef) - (-enController.unitArray.Count * 0.1f) - (distance * 0.4f);

                    float max = 10f + ((damage - enController.defenceSquad) * 1.1f) + (accuracy * 0.5f);
                   

                    float r = Random.Range(min, max);

                    if (r > 8.8f)
                    {

                        enController.GetUnitDie(0, -2);
                    }
                    else
                    {
                        if (r > 5f)
                        {
                            enController.GetDamage(arrowsAmount / hitColliders.Length);
                        }
                    }
                    //enController.MoraleChange(-enController.lostMoraleThenDie / 5f); 
                }


             }
            else // для своих дамаг чуть меньше !!!
            {
                SquadController enController = hitCollider.transform.parent.GetComponent<SquadController>();

                

                for (int i = 0; i < arrowsAmount / hitColliders.Length; i++) // равномерно распределятеся по всем отрядам
                {
                    float min = 0f - (enController.defenceSquad * 0.1f) - (enController.defenceCoef) - (-enController.unitArray.Count * 0.1f) - (distance * 0.4f);

                    float max = 10f + ((damage - enController.defenceSquad) * 1.1f) + (accuracy * 0.5f);
                    Debug.Log("min " + min + " max " + max);

                    float r = Random.Range(min, max);

                    if (r > 10f)
                    {

                        enController.GetUnitDie(0, -2);
                        enController.SpawnFriendlyFireFX();
                    }
                    else
                    {
                        if (r > 5f)
                        {
                            enController.GetDamage((arrowsAmount / hitColliders.Length)*2f); // c множителем шоб по своим не стрелял даун
                            enController.SpawnFriendlyFireFX();
                        }

                       
                    }
                    //enController.MoraleChange(-enController.lostMoraleThenDie / 5f); 
                }

            }
        }
    }
}
