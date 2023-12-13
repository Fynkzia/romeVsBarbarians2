using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseManager : MonoBehaviour
{
    [SerializeField] private GameObject playerSquads;
    [SerializeField] private GameObject enemySquads;
    [SerializeField] private WinLoseUI winLoseUI;

    public int playerSquadsCount;
    public int enemySquadsCount;

    private void Awake() {
        playerSquadsCount = playerSquads.transform.childCount;
        enemySquadsCount = enemySquads.transform.childCount;
    }

    private void Update() {
        if (playerSquadsCount == 0) {
            winLoseUI.ShowLose();
        }
        if (enemySquadsCount == 0) { 
            winLoseUI.ShowWin();
        }
    }
}
