using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera camera;
    private CameraMovement cameraMovement;

    [SerializeField]private bool banner;

    
    [SerializeField] private float scaleFactor;

    [SerializeField] public float[] scales;

    bool isInit = false;


    private void Awake() {

        camera = Camera.main;


        if (!banner)
        {
            isInit = true;
        }

    }

    public void InitBanner(Camera cam, CameraMovement camMov)
    {
        if (banner && cameraMovement == null)
        {
            camera = cam;
            cameraMovement = camMov;
            cameraMovement.OnZoomChanged += HandleZoomChanged;
        }
        isInit = true;
    }


    private void LateUpdate() {
        if (isInit)
        {
            // Направление от объекта к камере
            Vector3 directionToCamera = camera.transform.position - transform.position;

            // Проецируем направление на плоскость XZ (убираем компонент по Y)
            directionToCamera.y = 0;

            // Если длина направления нулевая, избегаем ошибок
            if (directionToCamera.sqrMagnitude > 0.001f)
            {
                // Устанавливаем поворот объекта так, чтобы он смотрел на камеру
                transform.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }

    }

    public void HandleZoomChanged(int newZoom)
    {
        

        if (newZoom < scales.Length)
        {
            float scale = scales[newZoom];
            transform.localScale = new Vector3(-scale, scale, scale);
        }
       
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
