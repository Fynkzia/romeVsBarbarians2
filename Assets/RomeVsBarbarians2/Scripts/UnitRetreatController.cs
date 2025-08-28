using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitRetreatController : MonoBehaviour {
    [SerializeField] private float speed;
    [SerializeField] private float timeToDisappear;
    [SerializeField] private float lastSeconds;
    [SerializeField] private float flashingTime;
    [SerializeField] public Vector3 enemyPos;


    [SerializeField] public float shakeIntensity = 0.5f; // Амплитуда тряски
    [SerializeField] public float shakeFrequency = 200f; // Частота тряски

    private Vector3 initialPosition; // Начальная позиция объекта
    private float shakeOffset; // Временной сдвиг для перлин шума

    private Rigidbody rb;
    
    private GameObject body;
    private SphereCollider colider;
    private float currentTime;
    private float flashTimer;
    [SerializeField]private Vector3 direction;

    [Header("Animation Settings")]
    [Space(10)]
    private MeshRenderer mesh;
    [SerializeField] private float animUpdateTime;
    [SerializeField] private float animTime;
    [SerializeField] private AnimationController animationController;
    private int runFrame;

    private void Start() {
        currentTime = 0;
        flashTimer = 0;
        rb = GetComponent<Rigidbody>();
       
        body = gameObject.transform.GetChild(0).gameObject;

        mesh = GetComponentInChildren<MeshRenderer>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        colider = GetComponent<SphereCollider>();


        colider.center = new Vector3(0f, 0.65f, 0f);
        colider.radius = 0.3f;

        gameObject.layer = 10; // retreat layer
       

        direction = (transform.position - enemyPos).normalized;
        direction += new Vector3(Random.Range(-0.1f,0.1f),0f, Random.Range(-0.1f, 0.1f));

        initialPosition = mesh.transform.localPosition; // Сохраняем начальную позицию
        shakeOffset = Random.Range(40.1f, 60.9f); // Уникальный сдвиг для объекта


        Rotate();


    }

    private void FixedUpdate() {
        if(currentTime<timeToDisappear) {
            currentTime += Time.fixedDeltaTime;
            animTime += Time.fixedDeltaTime;

            if (currentTime>timeToDisappear-lastSeconds) {
                Flashing(1);
            }
            else
            {
               // Flashing(2);
            }

            if (animTime > animUpdateTime)
            {
                RunAnimation();
                animTime = 0;
            }



            Vector3 targetPos = transform.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);

            float time = Time.time * shakeFrequency + shakeOffset;

            // Генерация шума по X и Y
            float offsetX = (Mathf.PerlinNoise(time, 0f) - 0.5f) * shakeIntensity * 2f;
            float offsetY = (Mathf.PerlinNoise(0f, time) - 0.5f) * shakeIntensity * 2f;

            // Применяем тряску
            mesh.transform.localPosition = initialPosition + new Vector3(offsetX, 0, offsetY);


        } else {
            Destroy(gameObject);
        }
    }

    private void RunAnimation()
    {
        animationController.SpriteAnimationChange(1+runFrame);
         runFrame++;
        if(2 < runFrame)
        {
            runFrame = 0;
        }

    }

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return 180f - Mathf.Atan2(a.z - b.z, a.x - b.x) * Mathf.Rad2Deg;
    }
    private void Flashing(float time) {
        flashTimer += Time.fixedDeltaTime * time;
        if(flashTimer>flashingTime) {
            flashTimer -= flashingTime;
            if(body.activeSelf) { 
                body.SetActive(false);
            } else {
                body.SetActive(true);
            }
        }
    }

    void Rotate()
    {
        float directionz = direction.z;
        float directionx = direction.x;

        Vector2 lineVec = new Vector2(-1, 1) - new Vector2(1, -1);

        float crossProduct = lineVec.x * directionz - lineVec.y * directionx;

        if (crossProduct < 0)
        {

            
               
                if (mesh.transform.localScale.x > 0)
                    mesh.transform.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
      

        }
        else
        {

            
              
                if (mesh.transform.localScale.x < 0)
                    mesh.transform.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
            
        }
    }
 }
