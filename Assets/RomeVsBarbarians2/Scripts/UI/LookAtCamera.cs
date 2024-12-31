using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Camera camera;
    private CameraMovement cameraMovement;

    [SerializeField]private bool banner;

    
    [SerializeField] private float scaleFactor;
    

    private void Awake() {
        camera = Camera.main;

        if (banner)
        {
            cameraMovement = GameObject.Find("CameraControlManager").GetComponent<CameraMovement>();
            cameraMovement.OnZoomChanged += HandleZoomChanged;
        }
    }
    private void LateUpdate() {
        transform.LookAt(camera.transform);
        transform.Rotate(0,180,0);
    }

    void HandleZoomChanged(int newZoom)
    {
        float scale = 0.7f + newZoom * scaleFactor;
        transform.localScale = new Vector3(scale, scale, scale);
    }

    void OnDisable()
    {
        if (cameraMovement != null)
        {
            cameraMovement.OnZoomChanged -= HandleZoomChanged; // Отписка от события
        }
    }

    void OnDestroy()
    {
        if (cameraMovement != null)
        {
            cameraMovement.OnZoomChanged -= HandleZoomChanged; // Отписка от события при уничтожении
        }
    }
}
