using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsCounter;
    [SerializeField]private int coins;

    private void Awake() {
        if (PlayerPrefs.HasKey("Coins")) { 
            coins = PlayerPrefs.GetInt("Coins"); 
        } else {
            PlayerPrefs.SetInt("Coins", coins);
        }
    }
    private void Start() {
        coinsCounter.text = "Coins: " + coins.ToString();
    }
    public void ChangeAmountOfCoins(int change) {
        coins += change;
        coinsCounter.text = "Coins: " + coins.ToString();
    }

    public int AmountOfCoins() { 
        return coins;
    }
}
