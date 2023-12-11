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
    private Animator animator;
    private GameObject body;
    private float currentTime;
    private float flashTimer;
    private void Start() {
        currentTime = 0;
        flashTimer = 0;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        body = gameObject.transform.GetChild(0).gameObject;
        animator.SetBool("Moving", true);

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        GetComponent<SphereCollider>().center = new Vector3(0f,-0.5f,0f);
    }

    private void FixedUpdate() {
        if(currentTime<timeToDisappear) {
            currentTime += Time.fixedDeltaTime;
            if(currentTime>timeToDisappear-lastSeconds) {
                Flashing();
            }
            Vector3 direction = new Vector3(Random.Range(randomX.x,randomX.y), transform.position.y, Random.Range(randomZ.x, randomZ.y));
            Vector3 targetPos = Vector3.MoveTowards(transform.position, direction, speed * Time.fixedDeltaTime);
            rb.MovePosition(targetPos);
            LookOn(targetPos);
        } else {
            Destroy(gameObject);
        }
    }
    private void LookOn(Vector3 target) {
        float y = AngleBetweenTwoPoints(transform.position, target);
        transform.rotation = Quaternion.Euler(new Vector3(0f, y, 0f));
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
}
