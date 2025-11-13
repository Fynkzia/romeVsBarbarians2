using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] private BattleSceneManager battleSceneManager;

    [SerializeField] private Camera gameCamera;
    [SerializeField] private Camera renderTextureCamera;
    [SerializeField] private Camera renderTerrainCamera;
    [SerializeField] private GameObject cinemachineObject;
    [SerializeField] private GameObject cinemachineFollowObject;
    public CinemachineVirtualCamera[] cams;
    public GameObject exitCam;
    public float exitCamTime;

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
    public int mainFOVIndex = 1;
    private float screenWidth;
    private float screenHeight;
    private int targetFOVIndex;
    private bool setDeltaTouch = false;
    private float deltaMagnitudeDiff;


    [SerializeField] private int prevFovIndex;
    [SerializeField] float zoomCurrentTime = 0f;

    [SerializeField] private bool zoomFog = false;
    [SerializeField] private bool isZoom = false;

    [SerializeField] private LayerMask cloudsMask;
    //[SerializeField] private List< CloudsFade> clouds;
    [SerializeField] private CloudsFade toFadeIn;
    [SerializeField] private Collider toFadeInColl;

    [SerializeField] SquadControlManager squadControlManager;

    

    public event Action<int> OnZoomChanged;
    public GraphicRaycaster raycaster;


    public void CameraSetEnterPoint()
    {
        cams[targetFOVIndex].gameObject.SetActive(false);
        exitCam.SetActive(true);
        gameCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = 0f;
    }

    public void CameraMoveToExit()
    {
        exitCam.SetActive(false);
        cams[targetFOVIndex].gameObject.SetActive(false);
        exitCam.SetActive(true);
        gameCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = exitCamTime;
    }

    //public void CameraEnterAnimation()
    //{
    //    CameraSetEnterPoint();



    //}

    public void UpdateRenderTexture()
    {

        renderTextureCamera.targetTexture = cameraTextures[targetFOVIndex];
        renderTerrainCamera.targetTexture = cameraTextures[targetFOVIndex];

        mainRenderTexture.texture = renderTextureCamera.targetTexture;


    }

    public void CameraMoveEnter()
    {
        

        targetFOVIndex = 2;

        CamZoom();
        gameCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = exitCamTime;
    }

    public void Init()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;

        panLimitX[0] = panLimitX[0] + transform.position.x;
        panLimitX[1] = panLimitX[1] + transform.position.x;

        panLimitZ[0] = panLimitZ[0] + transform.position.z;
        panLimitZ[1] = panLimitZ[1] + transform.position.z;

        OnZoomChanged?.Invoke(targetFOVIndex);

        mainRenderTexture = GameObject.Find("UIManager").GetComponent<UIManager>().rawBattleUiPanel;

        UpdateRenderTexture();

        Debug.Log("Init - ???");

        raycaster = GameObject.Find("UIManager").GetComponent<GraphicRaycaster>();
        //cams[targetFOVIndex].gameObject.SetActive(true);

        //        CamToPoint(battleSceneManager.playerSquads[0].transform.position);
    }
    private void Awake()
    {
        //mainFOVIndex = (int)Mathf.Ceil((fieldsOfView.Length - 1) / 2);
        //targetFOVIndex = mainFOVIndex;
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

        
    }

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            HandleCameraZoomTouch();
        }
        else
        {
            if (!squadControlManager.HasHitSquad())
            {
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

        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

        // Пускаем луч
        if (Physics.Raycast(origin, direction, out RaycastHit hit, 1000f, cloudsMask))
        {
            if (toFadeInColl == null)
            {
                Debug.Log("Попал в: " + hit.collider.gameObject.name);
                Debug.DrawLine(origin, hit.point, Color.red); // Визуализация в редакторе

                CloudsFade newCloud = hit.collider.GetComponent<CloudsFade>();

                toFadeIn = newCloud;
                toFadeInColl = hit.collider;
                toFadeIn.CloudsFadeSet(true);

                //newCloud.CloudsFadeSet(true);

                //clouds.Add(newCloud);
            }
            else if (toFadeInColl != hit.collider)
            {
                //toFadeIn.CloudsFadeSet(false);
                toFadeInColl = null;
                toFadeIn = null;
            }



        }
        else if (toFadeInColl != null)
        {
            //toFadeIn.CloudsFadeSet(false);
            toFadeInColl = null;
            toFadeIn = null;
        }


    }

    private void HandleCameraMovement()
    {
        

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;

            if (IsPointerClickingOnUI())
            {
                return;
            }
        }


        if (Input.GetMouseButtonUp(0))
        {
            lastMousePosition = Vector3.zero;
        }


        if (Input.GetMouseButton(0))
        {
            if (IsPointerClickingOnUI())
            {
                return;
            }

            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            Vector3 panVector = new Vector3(-mouseDelta.x / screenWidth, 0, -mouseDelta.y / screenHeight) * panSpeed[targetFOVIndex] * Time.deltaTime;

            panVector = Rotation * panVector;

            Vector3 newPosition = Vector3.Lerp(cinemachineFollowObject.transform.position, cinemachineFollowObject.transform.position + panVector, 1f);


            newPosition.x = Mathf.Clamp(newPosition.x, panLimitX[0], panLimitX[1]);
            newPosition.z = Mathf.Clamp(newPosition.z, panLimitZ[0], panLimitZ[1]);

            Debug.Log("HandleCameraMovement");
            cinemachineFollowObject.transform.position = newPosition;

            lastMousePosition = Input.mousePosition;
        }
    }

    private void HandleCameraZoom()
    {
        prevFovIndex = targetFOVIndex;
        if (Input.GetKeyDown(KeyCode.UpArrow) && targetFOVIndex < fieldsOfView.Length - 1)
        {


            targetFOVIndex++;


            CamZoom();


        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && targetFOVIndex > 0)
        {
            prevFovIndex = targetFOVIndex;


            targetFOVIndex--;


            CamZoom();



        }



    }


    private void HandleCameraZoomTouch()
    {

        Touch touch1 = Input.GetTouch(0);
        Touch touch2 = Input.GetTouch(1);

        Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
        Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

        prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;

        float touchDeltaMag = (touch1.position - touch2.position).magnitude;

        deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        prevFovIndex = targetFOVIndex;

        if (!setDeltaTouch && !isZoom)
        {

            if (deltaMagnitudeDiff > 0 && targetFOVIndex < fieldsOfView.Length - 1)
            {
                targetFOVIndex++;
                setDeltaTouch = true;

                CamZoom();


            }
            if (deltaMagnitudeDiff < 0 && targetFOVIndex > 0)
            {
                targetFOVIndex--;
                setDeltaTouch = true;

                CamZoom();


            }



        }

        if (touch1.phase == TouchPhase.Ended)
        {
            setDeltaTouch = false;
            lastMousePosition = touch2.position;
        }

        if (touch2.phase == TouchPhase.Ended)
        {
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



        OnZoomChanged?.Invoke(targetFOVIndex);

    }

    public void CamToPoint(Vector3 point)
    {
        if (Vector3.Distance(cinemachineFollowObject.transform.position, point) > 10f)
        {
            //Debug.Log("point " + point,gameObject);
            Vector3 newPoint = point;
            cinemachineFollowObject.transform.position = newPoint;
            //Debug.Log("point " + cinemachineFollowObject.transform.position, gameObject);
        }

    }

    public bool IsPointerClickingOnUI()
    {


        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();


        raycaster.Raycast(pointerData, results);

        foreach (var result in results)
        {
            // Фильтрация по тегу, имени или типу объекта
            if (result.gameObject.layer == 5)
            { // Задай нужным UI объектам этот тег
              //      Debug.Log("IsPointerClickingOnUI");

                return true;
            }
        }

        return false;
    }
}