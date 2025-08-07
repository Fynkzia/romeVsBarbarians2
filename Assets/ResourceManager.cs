using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private int startCampainCoins;

    [SerializeField] private int coins;

    public event Action<int> CoinsAmmountUpdate;

    private void Start() //передалть на инит с int - для загрузгрузки сохров
    {
        coins = startCampainCoins;
        CoinsAmmountUpdate?.Invoke(coins);
    }

    public void ChangeAmountOfCoins(int change)
    {
        coins += change;

        CoinsAmmountUpdate?.Invoke(coins);
    }

    public int AmountOfCoins()
    {
        return coins;
    }
}
