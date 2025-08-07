using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CoinsInfo : MonoBehaviour
{
    [SerializeField] public ResourceManager resourceManager;

   

    public TextMeshProUGUI coinsCountText;

 void Start()
{
        if (resourceManager != null)
        {
            resourceManager.CoinsAmmountUpdate += UpdateCoinsCount;
        }
        
}

    public void Init(ResourceManager resource)
    {
        resourceManager = resource;
        resourceManager.CoinsAmmountUpdate += UpdateCoinsCount; // надо бы еще отписаться?
        UpdateCoinsCount(resourceManager.AmountOfCoins());
    }

    public void UpdateCoinsCount(int coins)
    {

        coinsCountText.text = "" + coins;
    }

    

    private void OnEnable()
    {
        if (resourceManager != null)
        {
            resourceManager.CoinsAmmountUpdate += UpdateCoinsCount;
            UpdateCoinsCount(resourceManager.AmountOfCoins());
        }
      
    }

    private void OnDisable()
    {
        if (resourceManager != null)
        {
            resourceManager.CoinsAmmountUpdate -= UpdateCoinsCount;
            
        }

    }


}

