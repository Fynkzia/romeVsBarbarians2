using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    [SerializeField] private Camera cam;
    [SerializeField] private float panSpeed = 5f;  // Speed of panning.
    [SerializeField] private float[] panLimitZ; //Limits by Y
    [SerializeField] private float[] panLimitX; //Limits by X
    [SerializeField] private float zoomSpeed = 5f;

    public float fieldOfViewMain;
    public float fieldOfViewMin;
    public float fieldOfViewMax;

    private float deltaView = 2f; // Delta distance for field of view.
    private Vector3 lastMousePosition;
    private float fieldOfViewTarget;

    private void Awake() {
        cam.fieldOfView = fieldOfViewMain;
        fieldOfViewTarget = fieldOfViewMain;
    }

    private void Update() {
        HandleCameraMovement();
        HandleCameraZoom();
        //HandleCameraZoomTouch(); idk maybe it working
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
            Vector3 panVector = new Vector3(-mouseDelta.x, 0, -mouseDelta.y) * panSpeed * Time.deltaTime;

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
        if(Input.mouseScrollDelta.y > 0) {
            fieldOfViewTarget -= deltaView;
        }
        if(Input.mouseScrollDelta.y < 0) {
            fieldOfViewTarget += deltaView;
        }

        fieldOfViewTarget = Mathf.Clamp(fieldOfViewTarget, fieldOfViewMin, fieldOfViewMax);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView,fieldOfViewTarget, Time.deltaTime * zoomSpeed);
    }


    private void HandleCameraZoomTouch() {
        if (Input.touchCount == 2) {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);

            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
            Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

            float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
            float touchDeltaMag = (touch1.position - touch2.position).magnitude;

            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            float newFOV = cam.fieldOfView + deltaMagnitudeDiff * zoomSpeed * Time.deltaTime;

            // Clamp the FOV to the specified range.
            newFOV = Mathf.Clamp(newFOV, fieldOfViewMin, fieldOfViewMax);

            // Apply the new FOV to the camera.
            cam.fieldOfView = newFOV;
        }
    }
}
