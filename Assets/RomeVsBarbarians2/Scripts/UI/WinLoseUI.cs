using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinLoseUI : MonoBehaviour
{
    [SerializeField] private GameObject gameUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject winUI;


    private void Start() {
        gameObject.SetActive(true);
        loseUI.SetActive(false);
        winUI.SetActive(false);
    }

    public void ShowLose() {
        loseUI.SetActive(true);
        HideAnother();
    }

    public void ShowWin() {
        winUI.SetActive(true);
        HideAnother();
        Time.timeScale = 0f;
    }

    private void HideAnother() { 
        gameUI.SetActive(false);
        pauseUI.SetActive(false);
    }

}
