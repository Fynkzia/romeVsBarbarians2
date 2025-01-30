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
                Flashing(2);
            }

            if (animTime > animUpdateTime)
            {
                RunAnimation();
                animTime = 0;
            }



            Vector3 targetPos = transform.position + direction * speed * Time.fixedDeltaTime;
            rb.MovePosition(targetPos);
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
        float directionz = direction.z ;
        float directionx = direction.x ;

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
