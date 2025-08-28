using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityToDoController : MonoBehaviour
{
    public bool isSelected;

    CityController city;
    public ResourceManager resourceManager;

    public int doCount;

    public bool doHire;
    public SquadController[] squadHireListInCity;
    public SquadController toDoSquad;
    private int toDoSquadIndex;
    public float doHireTime;

    public bool doHouse;
    public float doHouseTime;
    public int doHouseCost;

    public bool doWall;
    public float doWallTime;
    public int doWallCost;

    public bool doTower;
    public float doTowerTime;
    public int doTowerCost;



    private float curentToDoTime;

   
    public MapControlManager mapControlManager;
    public MapSceneManager mapSceneManager;

    void Start()
    {
        city = GetComponent<CityController>();

        mapSceneManager = GameObject.Find("MapSceneManager").GetComponent<MapSceneManager>();
        mapControlManager = mapSceneManager.controlController;
        resourceManager  = GameObject.Find("ResourceManager").GetComponent<ResourceManager>();

        mapSceneManager.uIManager.hireUIPanel.DoProgress(0f);



    }

    void Update()
    {
        if (doHouse || doHire)
        {
            curentToDoTime += Time.deltaTime;

            
        }

        if (doHouse)
        {
           

            if (curentToDoTime > doHouseTime)
            {

                city.AddBuildings();

                doCount--;
                curentToDoTime = 0;

                if(doCount <= 0)
                {
                    doHouse = false;

                    if (isSelected)
                    {
                        mapSceneManager.uIManager.hireUIPanel.DeleteToDoPanel();
                    }
                }
              
            }

             if (isSelected)
            {
                mapSceneManager.uIManager.hireUIPanel.DoProgress(1f - curentToDoTime / doHouseTime);
                mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);
            }
        }

        if (doHire)
        {

            
            if (curentToDoTime > doHireTime)
            {
                AddSquad();

                doCount--;
                curentToDoTime = 0;

                if (doCount <= 0)
                {
                    doHire = false;

                    if (isSelected)
                    {
                        mapSceneManager.uIManager.hireUIPanel.DeleteToDoPanel();
                    }


                    toDoSquad = null;

                    toDoSquadIndex = -1;
                }

               

            }
            if (isSelected)
            {
                mapSceneManager.uIManager.hireUIPanel.DoProgress(1f - curentToDoTime / doHireTime);
                mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);
            }


        }
    }

    void AddSquad()
    {
        if (city.armyInCity != null)
        {
            city.armyInCity.AddNewSquad(toDoSquad);
        }
        else
        {
            ArmyController newArmy = null;
            if (city.isPlayer)
            {
                newArmy = Instantiate(city.armyObject.gameObject, mapSceneManager.playerArmiesObject).GetComponent<ArmyController>();
            }
            else
            {
                newArmy = Instantiate(city.armyEnemyObject.gameObject, mapSceneManager.enemiesArmiesObject).GetComponent<ArmyController>();
            }
            newArmy.transform.position = city.armyPoint.position;
            city.armyInCity = newArmy;
            city.armyInCity.inCity = true;

           

            city.armyInCity.city = city;

            if (isSelected)
            {
                city.armyInCity.ArmySelected();
            }

            newArmy.gameObject.SetActive(true);
            newArmy.mapSceneManager = mapSceneManager;

            city.armyInCity.AddNewSquad(toDoSquad);


           

            
        }


        


       

    }

    public void AiDoHouse() {

        CencelToDo();
        doHouse = true;

        doCount = 1;
    }

    public void AddToDoHouse()
    {
        if (doHouseCost <= resourceManager.AmountOfCoins())
        {
            

           

            if (doHouse)
            {
                doCount++;
            }
            else
            {

                if (doHire)
                {
                    CencelToDo();
                }

                doHouse = true;

                doCount = 1;


                if (isSelected)
                {
                    mapSceneManager.uIManager.hireUIPanel.CreateToDoHouse();
                }
            }

            if (isSelected)
            {
                mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);


                resourceManager.ChangeAmountOfCoins(-doHouseCost);
            }
           
        }

    }


    public void AiToDoSquad(SquadController squad,int index)
    {

        CencelToDo();
        doHire = true;

        doCount = 1;

        toDoSquad = squad;
        toDoSquadIndex = index;
    }

    public void AddToDoSquad(SquadController squad, int index)
    {
        if (squad.coinsNeed <= resourceManager.AmountOfCoins())
        {

            if (squad.unitsNeed <= city.cityUnits)
            {
                if (isSelected)
                {
                    city.cityUnits -= squad.unitsNeed;
                    resourceManager.ChangeAmountOfCoins(-squad.coinsNeed);
                    mapControlManager.uiManager.cityUIPanel.UpdateCityUIPanel(city);
                }


                if (doHire && toDoSquad == squad)
                {
                    doCount++;
                }
                else
                {

                    
                        CencelToDo();
                    

                    doHire = true;

                    toDoSquad = squad;
                    toDoSquadIndex = index;

                    doCount = 1;

                    if (isSelected)
                    {
                        mapSceneManager.uIManager.hireUIPanel.CreateToDoSquad(index);
                    }
                }




                if (isSelected)
                {
                    mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);
                }
             

            }
        }
    

    }
    


        public void ChangeDoCount( int addCount)
    {
        

        if (doHire)
        {
            if(addCount > 0)
            {
                AddToDoSquad(toDoSquad, toDoSquadIndex);
            }
            else
            {
                city.cityUnits += toDoSquad.unitsNeed;
                resourceManager.ChangeAmountOfCoins(toDoSquad.coinsNeed);
                doCount += addCount;

                mapSceneManager.uIManager.cityUIPanel.UpdateCityUIPanel(city);
            }


        }

        if (doHouse)
        {

            if (addCount > 0)
            {
                AddToDoHouse();
            }
            else
            {
                resourceManager.ChangeAmountOfCoins(doHouseCost);
                doCount += addCount;
            }

        }

        if (doCount <= 0)
        {

            CencelToDo();
        }

        
      


    }

        public void CitySelected()
    {
        mapSceneManager.uIManager.hireUIPanel.gameObject.SetActive(true);

        mapSceneManager.uIManager.hireUIPanel.houseButton.onClick.RemoveAllListeners();
        mapSceneManager.uIManager.hireUIPanel.plusDoButton.onClick.RemoveAllListeners();
        mapSceneManager.uIManager.hireUIPanel.minusDoButton.onClick.RemoveAllListeners();

        for (int i = 0; i < mapSceneManager.uIManager.hireUIPanel.hireButton.Length; i++)
        {
            int index = i;
            mapSceneManager.uIManager.hireUIPanel.hireButton[i].onClick.RemoveAllListeners();
            mapSceneManager.uIManager.hireUIPanel.hireButton[i].onClick.AddListener(() => this.AddToDoSquad(this.squadHireListInCity[index], index));

            SquadMapUIPanel squadPanel = mapSceneManager.uIManager.hireUIPanel.hireButton[i].gameObject.GetComponent<SquadMapUIPanel>();

            squadPanel.UpdateToDoCosts(squadHireListInCity[index].unitsNeed, squadHireListInCity[index].coinsNeed);


            //squadPanel.UpdateToDoPriceColors(squadHireListInCity[index].unitsNeed, squadHireListInCity[index].coinsNeed);
        }


        mapSceneManager.uIManager.hireUIPanel.houseButton.onClick.AddListener(() => AddToDoHouse());

        mapSceneManager.uIManager.hireUIPanel.plusDoButton.onClick.AddListener(() => ChangeDoCount(1));
        mapSceneManager.uIManager.hireUIPanel.minusDoButton.onClick.AddListener(() => ChangeDoCount(-1));

        if (toDoSquad != null)
        {
            mapSceneManager.uIManager.hireUIPanel.CreateToDoSquad(toDoSquadIndex);
        }

        if (doHouse)
        {
            mapSceneManager.uIManager.hireUIPanel.CreateToDoHouse();
        }



        isSelected = true;

    }

    public void CencelToDo()
    {
        if (isSelected)
        {
            if (doHouse)
            {
                resourceManager.ChangeAmountOfCoins(doHouseCost * doCount);
            }

            if (doHire)
            {
                city.cityUnits += toDoSquad.unitsNeed * doCount;
                resourceManager.ChangeAmountOfCoins(toDoSquad.coinsNeed * doCount);
            }

            mapSceneManager.uIManager.hireUIPanel.DeleteToDoPanel();
            mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);

            mapSceneManager.uIManager.hireUIPanel.DoProgress(0f);
        }

        if (doHouse || doHire)
        {
            doHouse = false;
            doHire = false;
            doTower = false;
            doWall = false;

            

            doCount = 0;
            curentToDoTime = 0;

           

            toDoSquad = null;

            toDoSquadIndex = -1;

           
        }

       

        }
    public void CityDeselect()
    {
        isSelected = false;


        if (toDoSquad != null)
        {
          
            mapSceneManager.uIManager.hireUIPanel.DeleteToDoPanel();
        }
        mapSceneManager.uIManager.hireUIPanel.gameObject.SetActive(false);
       
        //hireUIPanel = null;
    }
}
