using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;


public class AddCoinsEffect : MonoBehaviour
{
    [SerializeField] private GameObject coinPrefab;
   [SerializeField] private TextMeshPro coinsText;

    [SerializeField] private float spawnTime;
    [SerializeField] private float groupDelay;
    [SerializeField] private float groupCount;
    // Start is called before the first frame update
    public void SetEffect(int coins)
    {
        coinsText.text = "+" + coins;

        StartCoroutine(SpawnCoins(coins));

    }


    IEnumerator SpawnCoins(int count)
    {

        for (int i = 0; i < count; i++)
        {
            GameObject coin = Instantiate(coinPrefab, transform);


            coin.transform.position = transform.position;
            coin.transform.rotation = Quaternion.Euler(0f, Random.Range(0, 360f), 0f);

            yield return new WaitForSeconds(spawnTime / (float)count);

            //if (i % groupCount == 0)
            //{
            //    yield return new WaitForSeconds(groupDelay);
            //}
        }

        Destroy(gameObject,3f);
       
    }

}
