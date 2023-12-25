using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    [SerializeField] private GameObject playerSquads;
    [SerializeField] private GameObject enemySquads;
    [SerializeField] private WinLoseUI winLoseUI;
    [SerializeField] private TextMeshProUGUI rewardText;

    public int playerSquadsCount;
    public int enemySquadsCount;
    public bool isGameOver = false;
    private int reward;
    private int currentLevel;
    private CoinsController coinsController;

    private void Awake() {
        playerSquadsCount = playerSquads.transform.childCount;
        enemySquadsCount = enemySquads.transform.childCount;
        coinsController = GameObject.Find("CoinsController").GetComponent<CoinsController>();
        reward = PlayerPrefs.GetInt("CurrentReward");
        currentLevel = PlayerPrefs.GetInt("CurrentLevel");
    }

    private void Update() {
        if (!isGameOver) { 
            if (playerSquadsCount == 0) {
                winLoseUI.ShowLose();
                isGameOver = !isGameOver;
                PlayerPrefs.Save();
            }
            if (enemySquadsCount == 0) { 
                winLoseUI.ShowWin();
                WinSave();
                isGameOver = !isGameOver;
                PlayerPrefs.Save();
            }
            
        }
    }

    private void WinSave() {
        rewardText.text += reward;
        coinsController.ChangeAmountOfCoins(reward);
        int maxUnlockedLevel = PlayerPrefs.GetInt("MaxUnlockedLevel");
        if (currentLevel == maxUnlockedLevel) {
            maxUnlockedLevel++;
            PlayerPrefs.SetInt("MaxUnlockedLevel", maxUnlockedLevel);
        }
    }
}
