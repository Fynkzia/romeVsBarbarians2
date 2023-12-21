using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera camera;
    private void Awake() {
        camera = Camera.main;
    }
    private void LateUpdate() {
        transform.LookAt(camera.transform);
        transform.Rotate(0,180,0);
    }
}
