using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSceneManager : MonoBehaviour
{
    public CameraMapMovement cameraMapMovement;
    public MapControlManager controlController;
    public Camera gameCamera;
    public GameObject cinemachineGameObject;
    public GameObject lightGameObject;

    public UIManager uIManager;

    public CampainManager campainManager;

    public Transform playerArmiesObject;
    public Transform enemiesArmiesObject;

    public Transform citiesObject;

    public GridManager gridManager;



    private void Start()
    {
        
    }

    public void EnterMapScene()
    {
        gameCamera.gameObject.SetActive(true);
        lightGameObject.gameObject.SetActive(true);
        cinemachineGameObject.gameObject.SetActive(true);

        uIManager.MapUiActivation(true);

        controlController.gameObject.SetActive(true);


        controlController.CencelSelection();
        cameraMapMovement.ExitFromBattle();

       

    }
    public void ExitMapScene()
    {
        Debug.Log("ExitMapScene");



        // uIManager.gameObject.SetActive(false);

        uIManager.MapUiActivation(false);

        gameCamera.gameObject.SetActive(false);
        lightGameObject.gameObject.SetActive(false);
        cinemachineGameObject.gameObject.SetActive(false);

        controlController.gameObject.SetActive(false);

        

    }

    public void ExitMapSceneAnimation()
    {
        uIManager.TransitionAnimation(true);
        cameraMapMovement.EnterToBattle();
    }

    public void EnterMapSceneAnimation()
    {
        gameCamera.gameObject.SetActive(true);
        cinemachineGameObject.gameObject.SetActive(true);
        cameraMapMovement.EnterToBattle();
    }


}
