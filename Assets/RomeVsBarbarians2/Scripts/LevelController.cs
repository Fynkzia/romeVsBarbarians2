using System.Collections;
using System.Collections.Generic;
using TMPro;
using ToonyColorsPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Difficulty {
    Easy,
    Medium,
    Hard
}

[System.Serializable]
public class Level {
    public string name;
    public string description;
    public int reward;
    public string[] scenes;
};
public class LevelController : MonoBehaviour
{
    [SerializeField] private Level[] levels;
    [SerializeField] private TextMeshProUGUI nameLevel;
    [SerializeField] private TextMeshProUGUI descriptionLevel;
    [SerializeField] private TextMeshProUGUI rewardLevel;

    [SerializeField] private GameObject menu;
    [SerializeField] private Button[] levelButtons;

    private int currentLevel;
    private int maxUnlockedLevel = 0;
    private int currentReward;
    private void Awake() {
        if (PlayerPrefs.HasKey("MaxUnlockedLevel")) {
            maxUnlockedLevel = PlayerPrefs.GetInt("MaxUnlockedLevel");
        }
        else {
            PlayerPrefs.SetInt("MaxUnlockedLevel", maxUnlockedLevel);
        }
    }
    private void Start() {
        for(int i = 0; i< levels.Length;i++ ) {
            if (i <= maxUnlockedLevel) {
                levelButtons[i].interactable = true;
            } else {
                levelButtons[i].interactable = false;
            }
        }
        LevelClick(maxUnlockedLevel);
    }

    public void LevelClick(int index) { 
        nameLevel.text = levels[index].name;
        descriptionLevel.text = levels[index].description;
        currentReward = levels[index].reward;
        rewardLevel.text = "Reward: " + currentReward;
        currentLevel = index;
    }

    public void StartBattle() {
        PlayerPrefs.SetInt("CurrentReward", currentReward);
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        string sceneToLoad = levels[currentLevel].scenes[0];//change after 0 to random
        PlayerPrefs.Save();
        SceneManager.LoadScene(sceneToLoad);
    }

    public void ToggleMenu(bool state) { 
        menu.SetActive(state);
    }
}
