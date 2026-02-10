using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;


public class WinScreenUIPanel : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI totalKills;
    [SerializeField] public TextMeshProUGUI totalLoses;

    [SerializeField] public TextMeshProUGUI totalXP;
    [SerializeField] public TextMeshProUGUI totalCoins;


    [SerializeField] public Button exitButton;

    public GameObject winText;
    public GameObject loseText;

    public void PanelInit(bool win ,int kills, int loses, int xp, int coins)
    {
        gameObject.SetActive(true);
        totalKills.text = "" + kills;
        totalLoses.text = "" + loses;
        totalXP.text = "+" + xp;
        totalCoins.text = "+" + coins;

        if (win)
        {
            winText.gameObject.SetActive(true);
            loseText.gameObject.SetActive(false);
        }
        else
        {
            winText.gameObject.SetActive(false);
            loseText.gameObject.SetActive(true);
        }
    }
}
