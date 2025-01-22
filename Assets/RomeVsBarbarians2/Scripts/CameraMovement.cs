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
    [SerializeField] private GameObject cinemachineFollowObject;
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

  
    [SerializeField] private int prevFovIndex;
    [SerializeField] float zoomCurrentTime = 0f;
    
    [SerializeField]private bool zoomFog = false;
    [SerializeField] private bool isZoom = false;


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

        if (isZoom)
        {
            if (zoomFog) /// ебанутиший кастыль
            {
                if (zoomCurrentTime < zoomFovTime[2])  /// ебанутиший кастыль
                {


                    RenderSettings.fogEndDistance = Mathf.Lerp(RenderSettings.fogEndDistance, 3000, Time.deltaTime * 200f);
                    RenderSettings.fogStartDistance = Mathf.Lerp(RenderSettings.fogStartDistance, 2000, Time.deltaTime * 200f);

                }
                else
                {


                    zoomFog = false;
                    zoomCurrentTime = 0;


                    DOTween.To(
                        () => RenderSettings.fogEndDistance,
                        x => RenderSettings.fogEndDistance = x,
                        fogEnds[targetFOVIndex],
                         zoomTime[targetFOVIndex] - zoomFovTime[2]
                    );
                    DOTween.To(
                        () => RenderSettings.fogStartDistance,
                        x => RenderSettings.fogStartDistance = x,
                        fogStart[targetFOVIndex],
                         zoomTime[targetFOVIndex] - zoomFovTime[2]
                    );

                }



            }
            else
            {
                if (zoomCurrentTime > zoomTime[targetFOVIndex])  /// ебанутиший кастыль
                {
                    zoomCurrentTime = 0;
                    isZoom = false;

                    //var composer = cams[targetFOVIndex].GetCinemachineComponent<CinemachineComposer>();

                    //if (composer != null)
                    //{
                    //    Destroy(composer);
                    //}
                }
            }

            zoomCurrentTime += Time.deltaTime;
        }
    }

    private void HandleCameraMovement() {
      
        if (Input.GetMouseButtonDown(0)) {
            lastMousePosition = Input.mousePosition;
        }

      
        if (Input.GetMouseButtonUp(0)) {
            lastMousePosition = Vector3.zero;
        }

      
        if (Input.GetMouseButton(0)) {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            Vector3 panVector = new Vector3(-mouseDelta.x / screenWidth, 0, -mouseDelta.y / screenHeight) * panSpeed[targetFOVIndex] * Time.deltaTime;

            panVector =  Rotation * panVector;
          
            Vector3 newPosition = Vector3.Lerp(cinemachineFollowObject.transform.position, cinemachineFollowObject.transform.position + panVector , 1f);
            
          
            newPosition.x = Mathf.Clamp(newPosition.x, panLimitX[0], panLimitX[1]);
            newPosition.z = Mathf.Clamp(newPosition.z, panLimitZ[0], panLimitZ[1]);


            cinemachineFollowObject.transform.position = newPosition;

            lastMousePosition = Input.mousePosition;
        }
    }

    private void HandleCameraZoom() {
        prevFovIndex = targetFOVIndex;
        if (Input.GetKeyDown(KeyCode.RightArrow) && targetFOVIndex < fieldsOfView.Length - 1) {
            

            targetFOVIndex++;
            

             CamZoom();

        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && targetFOVIndex > 0) {
            prevFovIndex = targetFOVIndex;


            targetFOVIndex--;
            

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

        prevFovIndex = targetFOVIndex;

        if (!setDeltaTouch && !isZoom) {

            if (deltaMagnitudeDiff > 0 && targetFOVIndex < fieldsOfView.Length - 1) {
                targetFOVIndex++;
                setDeltaTouch = true;

                CamZoom();


            }
            if (deltaMagnitudeDiff < 0 && targetFOVIndex > 0) {
                targetFOVIndex--;
                setDeltaTouch = true;

                CamZoom();


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
        isZoom = true;
        zoomCurrentTime = 0;

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

       

        DOTween.To(
                    () => RenderSettings.fogEndDistance,      
                    x => RenderSettings.fogEndDistance = x,   
                    fogEnds[targetFOVIndex],            
                     zoomTime[indexSpeed]                   
                );
                DOTween.To(
                    () => RenderSettings.fogStartDistance,    
                    x => RenderSettings.fogStartDistance = x,    
                    fogStart[targetFOVIndex],         
                     zoomTime[indexSpeed]         
                );



        DOTween.To(
            () => renderTextureCamera.fieldOfView,      
            x => renderTextureCamera.fieldOfView = x,    
            fieldsOfView[targetFOVIndex],             
            zoomFovTime[indexSpeed]                   
        );
        DOTween.To(
            () => renderTerrainCamera.fieldOfView,      
            x => renderTerrainCamera.fieldOfView = x,   
            fieldsOfView[targetFOVIndex],            
            zoomFovTime[indexSpeed]                
        );
    




        for (int i = 0; i < cams.Length; i++)
        {
            cams[i].gameObject.SetActive(false);

            //var composer = cams[i].GetCinemachineComponent<CinemachineComposer>();
            //if (composer != null)
            //{
            //    Destroy(composer);
            //}
        }

        cams[targetFOVIndex].gameObject.SetActive(true);
       // cams[targetFOVIndex].AddCinemachineComponent<CinemachineComposer>();
        

        renderTextureCamera.targetTexture = cameraTextures[targetFOVIndex];
        renderTerrainCamera.targetTexture = cameraTextures[targetFOVIndex];

        mainRenderTexture.texture = renderTextureCamera.targetTexture;

        



    }

    
}