using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxPart : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private Vector3 startForce;
    [SerializeField] private Vector3 minRandomForce;
    [SerializeField] private Vector3 maxRandomForce;


    [SerializeField] private float lifeTime;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        rb.AddTorque(startForce + new  Vector3(Random.Range(minRandomForce.x, maxRandomForce.x), Random.Range(minRandomForce.y, maxRandomForce.y), Random.Range(minRandomForce.z, maxRandomForce.z)));

        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
