using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class CameraMapMovement : MonoBehaviour
{

    [SerializeField] private Camera gameCamera;

    [SerializeField] private GameObject cinemachineObject;
    [SerializeField] private GameObject cinemachineFollowObject;
    [SerializeField] public GameObject mapFollowObject;

    [SerializeField] private float panSpeed;  // Speed of panning.
    [SerializeField] private float[] panLimitZ; //Limits by Y
    [SerializeField] private float[] panLimitX; //Limits by X

    [SerializeField] private float followSpeed;
    [SerializeField] private Vector3 followTarget;

    [SerializeField] private Quaternion Rotation;

    [SerializeField] private Collider followCollider;
    [SerializeField] private bool isFollow;
    [SerializeField] private bool isArrived;
    [SerializeField] public bool ignoreMovement;


    private Vector3 lastMousePosition;

    
    private float screenWidth;
    private float screenHeight;

    [SerializeField] private GameObject mainCam;
    [SerializeField] private GameObject zoomedCam;
    [SerializeField] private GameObject toBattleCam;

    [SerializeField] private float defaultCamTime;
    [SerializeField] private float zoomCamTime;
    [SerializeField] private float battleCamTime;

    [SerializeField] private LayerMask cloudsMask;
    [SerializeField] private LayerMask mapMask;
    [SerializeField] private LayerMask terrainMask;
    //[SerializeField] private List< CloudsFade> clouds;
    [SerializeField] private CloudsFade toFadeIn;
    [SerializeField] private Collider toFadeInColl;

    public MapControlManager mapControlManager;

    public event Action<int> OnZoomChanged;

    public GraphicRaycaster raycaster;

    private void Start()
    {
        screenHeight = Screen.height;
        screenWidth = Screen.width;

        Zoom(false);
    }

    public void Zoom(bool zoom)
    {
        if (zoom) {

            mainCam.SetActive(false);
            zoomedCam.SetActive(true);

            OnZoomChanged?.Invoke(1);

            gameCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = zoomCamTime;
        }
        else
        {
            mainCam.SetActive(true);
            zoomedCam.SetActive(false);

            OnZoomChanged?.Invoke(0);

            gameCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = defaultCamTime;
        }

       

    }

    

    public void EnterToBattle()
    {
        //mainCam.SetActive(false);
        zoomedCam.SetActive(false);

        toBattleCam.SetActive(true);

        gameCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = battleCamTime;
    }

    public void ExitFromBattle()
    {
        mainCam.SetActive(false);
        zoomedCam.SetActive(true);

        toBattleCam.SetActive(false);

        gameCamera.GetComponent<CinemachineBrain>().m_DefaultBlend.m_Time = battleCamTime;
    }

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            
        }
        else
        {
            
                HandleCameraMovement();
            
            
        }

// выключаем облака

        Vector3 origin = Camera.main.transform.position;
        Vector3 direction = Camera.main.transform.forward;

       
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

 // выключаем облака

        if (isFollow)
        {
            

                //if (Physics.Raycast(origin, direction, out RaycastHit hit1, 3000f, mapMask))
                //{

                //}
                //else
                //{

                //    mapControlManager.CencelSelection();

                //    ignoreMovement = false;
                //    mapFollowObject = null;
                //}
            

            FollowObjectMove();
        }
        
    }

    private void HandleCameraMovement()
    {
        if (Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
        {
            // Пользователь прикасается к UI — игнорируем
            return;
        }

        

        if (!ignoreMovement)
        {

            if (IsPointerClickingOnUI())
            {
                return;
            }

            if (Input.GetMouseButtonDown(0) )
            {
                lastMousePosition = Input.mousePosition;
            }


            if (Input.GetMouseButtonUp(0))
            {
               // lastMousePosition = Vector3.zero;
            }


            if (Input.GetMouseButton(0))
            {
                Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
                Vector3 panVector = new Vector3(-mouseDelta.x / screenWidth, 0, -mouseDelta.y / screenHeight) * panSpeed * Time.deltaTime;

                panVector = Rotation * panVector;

                Vector3 newPosition = Vector3.Lerp(cinemachineFollowObject.transform.position, cinemachineFollowObject.transform.position + panVector, 1f);


                newPosition.x = Mathf.Clamp(newPosition.x, panLimitX[0], panLimitX[1]);
                newPosition.z = Mathf.Clamp(newPosition.z, panLimitZ[0], panLimitZ[1]);


                if (!isFollow && mapFollowObject == null)
                {
                    if (isArrived)
                    {
                        cinemachineFollowObject.transform.position = newPosition;


                        if (Vector3.Distance(cinemachineFollowObject.transform.position, followTarget) > 0.1f)
                        {
                            Debug.Log("CamToPoint - stop " + Vector3.Distance(cinemachineFollowObject.transform.position, followTarget));

                            isFollow = false;
                            Zoom(false);
                            mapFollowObject = null;

                            mapControlManager.CencelSelection();

                        }
                    }
                }
                else if (!isFollow && mapFollowObject != null)
                {
                    
                }
                else if (isFollow && mapFollowObject != null)
                {
                    if (isArrived)
                    {
                        cinemachineFollowObject.transform.position = newPosition;

                        if (Vector3.Distance(cinemachineFollowObject.transform.position, mapFollowObject.transform.position) > 0.1f)
                        {
                            Debug.Log("CamToPoint - stop " + Vector3.Distance(cinemachineFollowObject.transform.position, mapFollowObject.transform.position));

                            isFollow = false;
                            Zoom(false);
                            mapFollowObject = null;

                            mapControlManager.CencelSelection();

                        }
                    }
                }


                




                    lastMousePosition = Input.mousePosition;
            }
            else
            {

                if (mapFollowObject != null)
                {

                    followTarget = mapFollowObject.transform.position;

                }

            }
        }
    }




    void FollowObjectMove()
    {
        cinemachineFollowObject.transform.position = Vector3.MoveTowards(cinemachineFollowObject.transform.position, followTarget, followSpeed * Time.deltaTime);

        if (Vector3.Distance(cinemachineFollowObject.transform.position, followTarget) > 0.1f)
        {
            if (mapFollowObject == null)
            {
                ignoreMovement = true;
            }

         }
        else
        {
            if (mapFollowObject == null)
            {
                
                isFollow = false;
                
            }
            mapControlManager.cameraCentred = true;
            ignoreMovement = false;

            isArrived = true;
        }
    }
 

    

    public void CamToPoint( Vector3 point, GameObject followObject)
    {
        if (followObject == null)
        {

            if (mapControlManager.drawPath)
            {
                if (Vector3.Distance(cinemachineFollowObject.transform.position, point) > 18f)
                {

                    mapFollowObject = null;

                    isFollow = true;
                    followTarget = point;
                    isArrived = false;

                    
                }
                else
                {


                    mapControlManager.cameraCentred = true;
                    isFollow = false;
                    //followCollider = null;
                    isArrived = true;
                }
                return;
            }

           

            if (Vector3.Distance(cinemachineFollowObject.transform.position, point) > 1f)
            {
                
                
                mapFollowObject = null;

                isFollow = true;
                followTarget = point;
                isArrived = false;

               
            }
            else
            {

                
                mapControlManager.cameraCentred = true;
                isFollow = false;
                //followCollider = null;
                isArrived = true;
            }
        }
        else
        {
            //if (Vector3.Distance(cinemachineFollowObject.transform.position, point) > 0.1f)
            //{
            //    //cinemachineFollowObject.transform.position = point;

                if (followObject != null)
                {
                    mapFollowObject = followObject;


                }
                else
                {
                   
                }

                isFollow = true;
                followTarget = mapFollowObject.transform.position;
                isArrived = false;

                
            //}
            //else
            //{
            //    mapControlManager.cameraCentred = true;
            //    isFollow = false;
            //    //followCollider = null;
              //  isArrived = true;

                Debug.Log("followObject");
            //}
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
                Debug.Log("IsPointerClickingOnUI");

                return true;
            }
        }

        return false;
    }
}