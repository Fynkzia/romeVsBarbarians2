using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainManager : MonoBehaviour {
    [SerializeField] private string sceneName;
    [SerializeField] private GamePauseUI gamePauseUI;

    public void LoadMainScene() {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenu() {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
        if (gamePauseUI.isPaused) { 
            gamePauseUI.TogglePause();
        }
    }

    public void ReloadScene() {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}