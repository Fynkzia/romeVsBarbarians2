using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Camera camera;
    private CameraMovement cameraMovement;
    private CameraMapMovement cameraMapMovement;

    [SerializeField]private bool battleInfo;
    [SerializeField] private bool mapInfo;


    [SerializeField] private float scaleFactor;

    [SerializeField] public float[] scales;

    public bool isInit = false;


    private void Awake() {

        camera = Camera.main;


        if (!battleInfo && !mapInfo)
        {
            isInit = true;
        }


        if (mapInfo)
        {
            cameraMapMovement = GameObject.Find("MapControlManager").GetComponent<CameraMapMovement>();
            InitMapInfo(Camera.main, cameraMapMovement);
        }

    }

    public void Reset()
    {
       
            camera = null;

        if (cameraMapMovement != null)
        {
            cameraMapMovement.OnZoomChanged -= HandleZoomChanged;
            cameraMapMovement = null;
        }
        if (cameraMovement != null)
        {
            cameraMovement.OnZoomChanged -= HandleZoomChanged;
            cameraMovement = null;
        }


        isInit = false;
    }

    public void InitMapInfo(Camera cam, CameraMapMovement camMov)
    {
        if (mapInfo && !isInit)
        {
            camera = cam;
            cameraMapMovement = camMov;
            cameraMapMovement.OnZoomChanged += HandleZoomChanged;
        }
        isInit = true;
    }

    public void InitBattleInfo(Camera cam, CameraMovement camMov)
    {
        if (battleInfo && !isInit)
        {
            camera = cam;
            cameraMovement = camMov;
            cameraMovement.OnZoomChanged += HandleZoomChanged;
        }
        isInit = true;

        HandleZoomChanged(cameraMovement.mainFOVIndex);
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
        if (cameraMapMovement != null)
        {
            cameraMapMovement.OnZoomChanged -= HandleZoomChanged; // Отписка от события
        }
    }

    void OnDestroy()
    {
        if (cameraMovement != null)
        {
            cameraMovement.OnZoomChanged -= HandleZoomChanged; // Отписка от события при уничтожении
        }
        if (cameraMapMovement != null)
        {
            cameraMapMovement.OnZoomChanged -= HandleZoomChanged; // Отписка от события
        }
    }
}
