using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinsController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinsCounter;
    public int coins;

    private void Update() {
        coinsCounter.text = "Coins: " + coins.ToString();
    }
}
