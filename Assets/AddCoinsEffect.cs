using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;


public class AddCoinsEffect : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
   [SerializeField] private TextMeshPro coinsText;
    // Start is called before the first frame update
    public void SetEffect(int coins)
    {
        coinsText.text = "+" + coins;

        for (int i = 0; i < coins; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform);


            coin.transform.position = transform.position;
            coin.transform.rotation = Quaternion.Euler(0f, Random.Range(0, 360f), 0f);
        }

    }

   
}
