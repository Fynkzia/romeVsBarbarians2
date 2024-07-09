using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMovement : MonoBehaviour
{


    public static void Create(GameObject pfArrow,Vector3 spawnPosition,Vector3 target, float speed, float arrowsAmount, float damage, float accuracy, string tag) {
        float y = MathUtilities.AngleBetweenTwoPoints(spawnPosition, target);
        GameObject shot = Instantiate(pfArrow, spawnPosition, Quaternion.Euler(new Vector3(0f, y, 0f)));
        Debug.Log(shot.transform.position);
        ShotMovement shotMovement = shot.GetComponent<ShotMovement>();
        shotMovement.Setup(target, speed, arrowsAmount, damage, accuracy,tag);
        

        //Debug.Log("Create", shot);

    }
    public float speed;
    public Vector3 target;
    public float damage;
    public float accuracy;
    
    public float arrowsAmount;
    public float radius;

    [SerializeField] private GameObject pfVisual;
    [SerializeField] private Vector3 offsetPosition;
    [SerializeField] public LayerMask shotMask;

    private Vector3 _startPosition;
    private float _stepScale;
    private float _progress;
    [SerializeField] private float arcHeight = 3;
    private float distance;
    private bool shown = false;

    private const string ENEMY_TAG = "Enemy";
    private const string SQUAD_TAG = "Squad";


    private void Setup(Vector3 target, float speed, float arrowsAmount,float damage, float accuracy, string tag) {
        _startPosition = transform.position;

        distance = Vector3.Distance(_startPosition, target);
        this.target = target + new Vector3 (Random.Range(-0.4f,0.4f) * (distance /accuracy ), 0, Random.Range(-0.4f, 0.4f) * (distance / accuracy)); // разброс снарядов
        this.speed = speed;
        this.arrowsAmount = arrowsAmount;
        this.damage = damage;
        this.accuracy = accuracy;
        gameObject.tag = tag;
        CreateVisual();

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

        float speedCoef = 1f + (_progress - 0.5f) * (_progress - 0.5f)*2;
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
        //Debug.Log("parabola " + parabola + "distance " + distance);

        nextPos.y += parabola + (arcHeight * (distance / 50f));
        //Debug.Log("distance " + distance);

        // Continue as before.
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
        }

        Vector3 rot = nextPos - transform.position;
            transform.rotation = Quaternion.LookRotation(rot.normalized);

       
        
        transform.position = nextPos;
        // I presume you disable/destroy the arrow in Arrived so it doesn't keep arriving.
        if (_progress == 1.0f)
        {
            Damage();
            Destroy(gameObject);
        }
    }

    private void Damage()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius, shotMask);

        

        foreach (var hitCollider in hitColliders)
        {


            if ((tag == SQUAD_TAG && hitCollider.gameObject.tag == ENEMY_TAG) || (tag == ENEMY_TAG && hitCollider.gameObject.tag == SQUAD_TAG))
            {

                SquadController enController = hitCollider.transform.parent.GetComponent<SquadController>();

                for (int i = 0; i < arrowsAmount; i++)
                {
                    float min = 0f - (enController.defenceSquad * 0.1f) - (enController.defenceCoef) - (-enController.unitArray.Count * 0.1f) - (distance * 0.4f);

                    float max = 10f + ((damage - enController.defenceSquad) * 1.1f) + (accuracy * 0.5f);
                    //Debug.Log("min " + min + " max " + max);

                    float r = Random.Range(min, max);

                    if (r > 8.8f)
                    {

                        enController.DieRandomUnit();
                    }
                    enController.MoraleChange(-enController.lostMoraleThenDie / 5f); 
                }


             }
            else // для своих дамаг чуть меньше !!!
            {
                SquadController enController = hitCollider.transform.parent.GetComponent<SquadController>();

                for (int i = 0; i < arrowsAmount; i++)
                {
                    float min = 0f - (enController.defenceSquad * 0.1f) - (enController.defenceCoef) - (-enController.unitArray.Count * 0.1f) - (distance * 0.4f);

                    float max = 10f + ((damage - enController.defenceSquad) * 1.1f) + (accuracy * 0.5f);
                    Debug.Log("min " + min + " max " + max);

                    float r = Random.Range(min, max);

                    if (r > 9.8f)
                    {

                        enController.DieRandomUnit();
                    }
                    enController.MoraleChange(-enController.lostMoraleThenDie / 5f); 
                }

            }
        }
    }
}
