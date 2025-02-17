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

    public Animator transitionAnimation;


    public void EnterMapScene()
    {
        gameCamera.gameObject.SetActive(true);
        lightGameObject.gameObject.SetActive(true);
        cinemachineGameObject.gameObject.SetActive(true);

        controlController.gameObject.SetActive(true);

        cameraMapMovement.ExitFromBattle();

        transitionAnimation.SetTrigger("Out");

    }
    public void ExitMapScene()
    {
        Debug.Log("ExitMapScene");

        


        gameCamera.gameObject.SetActive(false);
        lightGameObject.gameObject.SetActive(false);
        cinemachineGameObject.gameObject.SetActive(false);

        controlController.gameObject.SetActive(false);

        

    }

    public void ExitMapSceneAnimation()
    {
        transitionAnimation.SetTrigger("In");
        cameraMapMovement.EnterToBattle();
    }


    }
