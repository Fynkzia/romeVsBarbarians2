using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Difficulty {
    Easy,
    Medium,
    Hard
}

[System.Serializable]
public class Level {
    public string name;
    public string description;
    public Scene[] scenes;
};
public class LevelController : MonoBehaviour
{
    [SerializeField] private Level[] levels;
    [SerializeField] private TextMeshProUGUI nameLevel;
    [SerializeField] private TextMeshProUGUI description;

    public void LevelClick(int index) { 
        nameLevel.text = levels[index].name;
        description.text = levels[index].description;
    }
}
