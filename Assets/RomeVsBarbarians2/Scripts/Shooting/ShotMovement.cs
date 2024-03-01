using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotMovement : MonoBehaviour
{


    public static void Create(GameObject pfArrow,Vector3 spawnPosition,Vector3 target, float speed, float arrowsAmount) {
        float y = MathUtilities.AngleBetweenTwoPoints(spawnPosition, target);
        GameObject shot = Instantiate(pfArrow, spawnPosition, Quaternion.Euler(new Vector3(0f, y, 0f)));
        Debug.Log(shot.transform.position);
        ShotMovement shotMovement = shot.GetComponent<ShotMovement>();
        shotMovement.Setup(target, speed, arrowsAmount);

       // Debug.Log("Create", shot);

    }
    public float speed;
    public Vector3 target;
    public float arrowsAmount;
    [SerializeField] private GameObject pfVisual;
    [SerializeField] private Vector3 offsetPosition;

    private Vector3 _startPosition;
    private float _stepScale;
    private float _progress;
    private float arcHeight = 2;
    private void Setup(Vector3 target, float speed, float arrowsAmount) {
        _startPosition = transform.position;

        float distance = Vector3.Distance(_startPosition, target);
        this.target = target;
        this.speed = speed;
        this.arrowsAmount = arrowsAmount;
        CreateVisual();

        // This is one divided by the total flight duration, to help convert it to 0-1 progress.
        _stepScale = speed / distance;
    }
    private void CreateVisual() {
        for (int i = 0; i < arrowsAmount; i++) {
           GameObject shotVisual= Instantiate(pfVisual, MathUtilities.RandomOffset(transform.position, offsetPosition), pfVisual.transform.rotation, transform);
            shotVisual.transform.localRotation = pfVisual.transform.rotation;
        }
    }
    private void Update() {
        // Increment our progress from 0 at the start, to 1 when we arrive.
        _progress = Mathf.Min(_progress + Time.deltaTime * _stepScale, 1.0f);

        // Turn this 0-1 value into a parabola that goes from 0 to 1, then back to 0.
        float parabola = 1.0f - 20.0f * (_progress - 0.5f) * (_progress - 0.5f);

        // Travel in a straight line from our start position to the target.        
        Vector3 nextPos = Vector3.Lerp(_startPosition, target, _progress);

        // Then add a vertical arc in excess of this.
        nextPos.y += parabola + arcHeight;

        // Continue as before.
        transform.LookAt(nextPos, transform.forward);
        transform.position = nextPos;

        // I presume you disable/destroy the arrow in Arrived so it doesn't keep arriving.
        if (_progress == 1.0f)
            Destroy(gameObject);
    }
}
