    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapAICities : MonoBehaviour
{
    [SerializeField] public float enemyStartCoins;
    [SerializeField] public float enemyCoins;

   

    public SquadController[] squadHireListInCity;
    [Header("Other")]

    [SerializeField] MapSceneManager mapSceneManager;
    [SerializeField] MapAIGlobal aIGlobal;
    // Start is called before the first frame update
    void Start()
    {
        enemyCoins = enemyStartCoins;
    }

  

    public void AiCitiesAction(CityController bestCity, int cityActionIndex)
    {
       

        if(bestCity == null)
        {
            Debug.Log("bestCity == null");
        }

        if (cityActionIndex == 0)// ничего не делаем
        {


        }
        else
        if (cityActionIndex == -1)// делаем дом
        {
            if (enemyCoins > 10)
            {
                bestCity.toDoController.AiDoHouse();
                enemyCoins -= 10f;



            }

        }
        else
        if (cityActionIndex == 1)// делаем отряд 0
        {

            if (enemyCoins > 50 && bestCity.cityUnits > 50)
            {
                bestCity.toDoController.AiToDoSquad(0);

                enemyCoins -= 50f;
                bestCity.cityUnits -= 50;



            }

        }
        else
        if (cityActionIndex == 2)// делаем отряд 1
        {
            if (enemyCoins > 50f && bestCity.cityUnits > 50f)
            {
                bestCity.toDoController.AiToDoSquad(1);

                enemyCoins -= 50f;
                bestCity.cityUnits -= 50f;



            }


        }
        else
        if (cityActionIndex == 3)// делаем отряд 2
        {
            if (enemyCoins > 50f && bestCity.cityUnits > 50f)
            {
                bestCity.toDoController.AiToDoSquad(2);

                enemyCoins -= 50f;
                bestCity.cityUnits -= 50f;


            }


        }
        else
        {

        }










    }
}
