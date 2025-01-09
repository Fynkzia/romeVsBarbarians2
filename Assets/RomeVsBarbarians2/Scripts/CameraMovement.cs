using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;

public class CameraMovement : MonoBehaviour {

    [SerializeField] private Camera gameCamera;
    [SerializeField] private Camera renderTextureCamera;
    [SerializeField] private Camera renderTerrainCamera;
    [SerializeField] private GameObject cinemachineObject;
    public CinemachineVirtualCamera[] cams;
    [SerializeField] private float[] fogStart;
    [SerializeField] private float[] fogEnds;
    [SerializeField] private float[] panSpeed;  // Speed of panning.
    [SerializeField] private float[] panLimitZ; //Limits by Y
    [SerializeField] private float[] panLimitX; //Limits by X
    [SerializeField] private float[] zoomTime;
    [SerializeField] private float[] zoomFovTime;
    [SerializeField] private RenderTexture[] cameraTextures;
    [SerializeField] private RawImage mainRenderTexture;

    [SerializeField] private Quaternion Rotation;


    public float[] fieldsOfView;

    private Vector3 lastMousePosition;
    private float prevTouchDeltaMag;
    private int mainFOVIndex = 1;
    private float screenWidth;
    private float screenHeight;
    private int targetFOVIndex;
    private bool setDeltaTouch = false;

    float deltaFov;
    [SerializeField] private int prevFovIndex;
    [SerializeField] float zoomCurrentTime = 0f;
    private bool zoomTransform = false;
    private bool zoomFog = false;
    private bool zoomFov = false;

    private float deltaMagnitudeDiff;

    public event Action<int> OnZoomChanged;


    private void Start () {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
    }
    private void Awake() {
        //mainFOVIndex = (int)Mathf.Ceil((fieldsOfView.Length - 1) / 2);
        targetFOVIndex = mainFOVIndex;
        gameCamera.fieldOfView = fieldsOfView[mainFOVIndex];
        renderTextureCamera.fieldOfView = fieldsOfView[mainFOVIndex];
        renderTerrainCamera.fieldOfView = fieldsOfView[mainFOVIndex];

        renderTextureCamera.targetTexture = cameraTextures[targetFOVIndex];
        renderTextureCamera.targetTexture = cameraTextures[targetFOVIndex];
        //cams[mainFOVIndex].Priority = 100;

        for (int i = 0; i < cams.Length; i++)
        {
            cams[i].m_Lens.FieldOfView = fieldsOfView[i];

        }

        cams[targetFOVIndex].gameObject.SetActive(true);
    }

    private void Update() {
        if (Input.touchCount == 2) {
            HandleCameraZoomTouch();
        } else {
           if (!SquadControlManager.Instance.HasHitSquad()) {
                HandleCameraMovement();
            }
            HandleCameraZoom();   
        }
        //CamZoom();

        if (zoomFog)
        {
            if (zoomCurrentTime < zoomFovTime[2])
            {

                //float offset = renderTextureCamera.fieldOfView - deltaFov;

                //offset *= 50f;

                RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, 3000, Time.deltaTime * 200f );
                RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, 2000, Time.deltaTime * 200f);
               // RenderSettings.fog = false;
            }
            else
            {

                
                zoomFog = false;
                zoomCurrentTime = 0;
                deltaFov = 0;

                DOTween.To(
                    () => RenderSettings.fogEndDistance,       // Getter: что изменяем
                    x => RenderSettings.fogEndDistance = x,    // Setter: куда записываем изменённое значение
                    fogEnds[targetFOVIndex],              // Конечное значение
                     zoomTime[targetFOVIndex] - zoomFovTime[2]                // Время изменения
                );
                DOTween.To(
                    () => RenderSettings.fogStartDistance,       // Getter: что изменяем
                    x => RenderSettings.fogStartDistance = x,    // Setter: куда записываем изменённое значение
                    fogStart[targetFOVIndex],              // Конечное значение
                     zoomTime[targetFOVIndex] - zoomFovTime[2]         // Время изменения
                );

            }
            zoomCurrentTime += Time.deltaTime;
            deltaFov = renderTerrainCamera.fieldOfView;
            
        }
    }
    private void HandleCameraMovement() {
        // Check for mouse button click to start panning.
        if (Input.GetMouseButtonDown(0)) {
            lastMousePosition = Input.mousePosition;
        }

        // Check for mouse button release to stop panning.
        if (Input.GetMouseButtonUp(0)) {
            lastMousePosition = Vector3.zero;
        }

        // If the left mouse button is held down, pan the camera.
        if (Input.GetMouseButton(0)) {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            Vector3 panVector = new Vector3(-mouseDelta.x / screenWidth, 0, -mouseDelta.y / screenHeight) * panSpeed[targetFOVIndex] * Time.deltaTime;

            panVector =  Rotation * panVector;
            // Find new camera position
            Vector3 newPosition = Vector3.Lerp(cinemachineObject.transform.position, cinemachineObject.transform.position + panVector , 1f);
            
            // Look for boundaries
            newPosition.x = Mathf.Clamp(newPosition.x, panLimitX[0], panLimitX[1]);
            newPosition.z = Mathf.Clamp(newPosition.z, panLimitZ[0], panLimitZ[1]);


            cinemachineObject.transform.position = newPosition;

            // Update the last mouse position for the next frame.
            lastMousePosition = Input.mousePosition;
        }
    }

    private void HandleCameraZoom() {
        
        if (Input.GetKeyDown(KeyCode.RightArrow) && targetFOVIndex < fieldsOfView.Length - 1) {
            prevFovIndex = targetFOVIndex;

            targetFOVIndex++;
            zoomCurrentTime = 0;

            for (int i = 0; i < cams.Length; i++)
            {
                cams[i].gameObject.SetActive(false);
            }
            cams[targetFOVIndex].gameObject.SetActive(true);

            CamZoom();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && targetFOVIndex > 0) {
            prevFovIndex = targetFOVIndex;


            targetFOVIndex--;
            zoomCurrentTime = 0;

            for (int i = 0; i < cams.Length; i++)
            {
                cams[i].gameObject.SetActive(false);
            }
            cams[targetFOVIndex].gameObject.SetActive(true);

            CamZoom();

           
        }
        OnZoomChanged?.Invoke(targetFOVIndex);

       
    }


    private void HandleCameraZoomTouch() {

        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
        Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

        prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;

        float touchDeltaMag = (touch1.position - touch2.position).magnitude;

        deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;


        if (!setDeltaTouch) {

            if (deltaMagnitudeDiff > 0 && targetFOVIndex < fieldsOfView.Length - 1) {
                targetFOVIndex++;
                setDeltaTouch = true;

                for (int i = 0; i < cams.Length; i++)
                {
                    cams[i].gameObject.SetActive(false);
                }
                cams[targetFOVIndex].gameObject.SetActive(true);

            }
            if (deltaMagnitudeDiff < 0 && targetFOVIndex > 0) {
                targetFOVIndex--;
                setDeltaTouch = true;

                for (int i = 0; i < cams.Length; i++)
                {
                    cams[i].gameObject.SetActive(false);
                }
                cams[targetFOVIndex].gameObject.SetActive(true);

            }
          


        }

        if(touch1.phase == TouchPhase.Ended) {
            setDeltaTouch = false;
            lastMousePosition = touch2.position;
        }

        if (touch2.phase == TouchPhase.Ended) {
            setDeltaTouch = false;
            lastMousePosition = touch1.position;
        }

       
    }

    private void CamZoom()
    {
       

        if (prevFovIndex == 2) /// ебанутиший кастыль
            {
            zoomFog = true;
        }

        int indexSpeed = 0;

        if (prevFovIndex > targetFOVIndex) 
        {
            indexSpeed = prevFovIndex;
        }
        else
        {
            indexSpeed = targetFOVIndex;
        }

        Debug.Log("indexSpeed " + indexSpeed);

        gameCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = zoomTime[indexSpeed];

        //RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, fogEnds[targetFOVIndex], Time.deltaTime );
        //RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, fogStart[targetFOVIndex], Time.deltaTime );

        DOTween.To(
                    () => RenderSettings.fogEndDistance,       // Getter: что изменяем
                    x => RenderSettings.fogEndDistance = x,    // Setter: куда записываем изменённое значение
                    fogEnds[targetFOVIndex],              // Конечное значение
                     zoomTime[indexSpeed]                   // Время изменения
                );
                DOTween.To(
                    () => RenderSettings.fogStartDistance,       // Getter: что изменяем
                    x => RenderSettings.fogStartDistance = x,    // Setter: куда записываем изменённое значение
                    fogStart[targetFOVIndex],              // Конечное значение
                     zoomTime[indexSpeed]         // Время изменения
                );



        //renderTextureCamera.fieldOfView = Mathf.Lerp(renderTextureCamera.fieldOfView, fieldsOfView[targetFOVIndex], Time.deltaTime );
        //renderTerrainCamera.fieldOfView = Mathf.Lerp(renderTextureCamera.fieldOfView, fieldsOfView[targetFOVIndex], Time.deltaTime );
        DOTween.To(
            () => renderTextureCamera.fieldOfView,       // Getter: что изменяем
            x => renderTextureCamera.fieldOfView = x,    // Setter: куда записываем изменённое значение
            fieldsOfView[targetFOVIndex],              // Конечное значение
            zoomFovTime[indexSpeed]                   // Время изменения
        );
        DOTween.To(
            () => renderTerrainCamera.fieldOfView,       // Getter: что изменяем
            x => renderTerrainCamera.fieldOfView = x,    // Setter: куда записываем изменённое значение
            fieldsOfView[targetFOVIndex],              // Конечное значение
            zoomFovTime[indexSpeed]                   // Время изменения
        );
            //}
            
            
        //}
       

       

        renderTextureCamera.targetTexture = cameraTextures[targetFOVIndex];
        renderTerrainCamera.targetTexture = cameraTextures[targetFOVIndex];

        mainRenderTexture.texture = renderTextureCamera.targetTexture;

        



    }

    
}