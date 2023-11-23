using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {
    [SerializeField] private string sceneName;

    public void LoadMainScene() {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
    }

}