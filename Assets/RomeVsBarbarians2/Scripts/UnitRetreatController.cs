using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class UnitRetreatController : MonoBehaviour {
    [SerializeField] private float speed;
    [SerializeField] private float timeToDisappear;
    [SerializeField] private float lastSeconds;
    [SerializeField] private float flashingTime;
    [SerializeField] private Vector2 randomX;
    [SerializeField] private Vector2 randomZ;

    private Rigidbody rb;
    
    private GameObject body;
    private float currentTime;
    private float flashTimer;
    [SerializeField]private Vector3 direction;

    [Header("Animation Settings")]
    [Space(10)]
    private MeshRenderer mesh;
    [SerializeField] private float animUpdateTime;
    [SerializeField] private float animTime;
    [SerializeField] private AnimationController animationController;
    private int runFame;

    private void Start() {
        currentTime = 0;
        flashTimer = 0;
        rb = GetComponent<Rigidbody>();
       
        body = gameObject.transform.GetChild(0).gameObject;

        mesh = GetComponentInChildren<MeshRenderer>();


        gameObject.layer = 10;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<SphereCollider>().center = new Vector3(0f,-0.5f,0f);
        direction = new Vector3(Random.Range(randomX.x, randomX.y), transform.position.y, Random.Range(randomZ.x, randomZ.y));

        Rotate();


    }

    private void FixedUpdate() {
        if(currentTime<timeToDisappear) {
            currentTime += Time.fixedDeltaTime;
            animTime += Time.fixedDeltaTime;

            if (currentTime>timeToDisappear-lastSeconds) {
                Flashing();
            }

            if (animTime > animUpdateTime)
            {
                RunAnimation();
                animTime = 0;
            }



            Vector3 targetPos = Vector3.MoveTowards(transform.position, direction, speed * Time.fixedDeltaTime);
            rb.MovePosition(targetPos);
        } else {
            Destroy(gameObject);
        }
    }

    private void RunAnimation()
    {
        animationController.SpriteAnimationChange(1+runFame);
         runFame++;
        if(2 < runFame)
        {
            runFame = 0;
        }

    }

    private float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
        return 180f - Mathf.Atan2(a.z - b.z, a.x - b.x) * Mathf.Rad2Deg;
    }
    private void Flashing() {
        flashTimer += Time.fixedDeltaTime;
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
        float directionz = direction.z - transform.position.z;
        float directionx = direction.x - transform.position.x;

        //direction = Quaternion.EulerAngles(0f, -45f, 0f) * direction;
        Transform mesh = transform.GetChild(0).transform;

        if (directionx < 0 && directionz <= 0)
        {
            
               
                if (mesh.transform.localScale.x < 0)
                    mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
            
        }
        else if (directionx > 0 && directionz >= 0)
        {
           
               
                if (mesh.transform.localScale.x > 0)
                    mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
            
        }

        if (directionx < 0 && directionz >= 0)
        {
            
              
                if (mesh.transform.localScale.x > 0)
                    mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
            
        }
        else if (directionx > 0 && directionz <= 0)
        {
            
                if (mesh.transform.localScale.x > 0)
                    mesh.localScale = new Vector3(mesh.transform.localScale.x * -1, mesh.transform.localScale.y, mesh.transform.localScale.z);
            
        }
    }
}
