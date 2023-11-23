using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseUI : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;
    private bool isPaused = false;

    private void Start() {
        Hide();
    }
    public void TogglePause() { 
        isPaused = !isPaused;
        if (isPaused) {
            Time.timeScale = 0f;
            Show();
        }
        else {
            Time.timeScale = 1f;
            Hide();
        }
    }
    private void Show() {
        gameObject.SetActive(true);
        gameUI.SetActive(false);
    }

    private void Hide() {
        gameObject.SetActive(false);
        gameUI.SetActive(true);
    }
}
