using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraMovement : MonoBehaviour {

    [SerializeField] private Camera cam;
    [SerializeField] private float[] panSpeed;  // Speed of panning.
    [SerializeField] private float[] panLimitZ; //Limits by Y
    [SerializeField] private float[] panLimitX; //Limits by X
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private RenderTexture[] cameraTextures;
    [SerializeField] private RawImage mainRenderTexture;

    public float[] fieldsOfView;

    private Vector3 lastMousePosition;
    private float prevTouchDeltaMag;
    private int mainFOVIndex = 1;
    private float screenWidth;
    private float screenHeight;
    private int targetFOVIndex;
    private bool setDeltaTouch = false;

    private float deltaMagnitudeDiff;
    
    
    private void Start () {
        screenHeight = Screen.height;
        screenWidth = Screen.width;
    }
    private void Awake() {
        //mainFOVIndex = (int)Mathf.Ceil((fieldsOfView.Length - 1) / 2);
        targetFOVIndex = mainFOVIndex;
        cam.fieldOfView = fieldsOfView[mainFOVIndex];
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
        CamZoom();
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

            // Find new camera position
            Vector3 newPosition = cam.transform.position + panVector;
            
            // Look for boundaries
            newPosition.x = Mathf.Clamp(newPosition.x, panLimitX[0], panLimitX[1]);
            newPosition.z = Mathf.Clamp(newPosition.z, panLimitZ[0], panLimitZ[1]);
            cam.transform.position = newPosition;

            // Update the last mouse position for the next frame.
            lastMousePosition = Input.mousePosition;
        }
    }

    private void HandleCameraZoom() {
        if (Input.GetKeyDown(KeyCode.RightArrow) && targetFOVIndex < fieldsOfView.Length - 1) {
            targetFOVIndex++;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) && targetFOVIndex > 0) {
            targetFOVIndex--;
        }
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
            }
            if (deltaMagnitudeDiff < 0 && targetFOVIndex > 0) {
                targetFOVIndex--;
                setDeltaTouch = true;
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

    private void CamZoom() {
       
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, fieldsOfView[targetFOVIndex], Time.deltaTime * zoomSpeed);
        //cam.targetTexture = cameraTextures[targetFOVIndex];
        //mainRenderTexture.texture = cam.targetTexture;
        
    }
}