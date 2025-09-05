using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityToDoController : MonoBehaviour
{
    public bool isSelected;

    CityController city;
    public ResourceManager resourceManager;

    public List<int> doQueue;

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
        if (doQueue.Count > 0)
        {
            curentToDoTime += Time.deltaTime;






            if (doQueue[0] == -1) // дом
            {


                if (curentToDoTime > doHouseTime)
                {

                    city.AddBuildings();

                    doQueue.RemoveAt(0);
                    curentToDoTime = 0;

                    if (isSelected)
                    {
                        mapSceneManager.uIManager.hireUIPanel.SetQueue(doQueue.ToArray(), this);

                        PriceUpdate();

                        if (doQueue.Count == 0)
                        {
                            mapSceneManager.uIManager.hireUIPanel.progressImage.gameObject.SetActive(false);
                        }
                    }


                    //if (isSelected)
                    //{
                    //    mapSceneManager.uIManager.hireUIPanel.DeleteToDoPanel();
                    //}


                }

                // if (isSelected)
                //{
                //    mapSceneManager.uIManager.hireUIPanel.DoProgress(1f - curentToDoTime / doHouseTime);
                //    mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);
                //}
            }

            if (doQueue[0] >= 0)
            {


                if (curentToDoTime > doHireTime)
                {
                    toDoSquadIndex = doQueue[0] ;
                    toDoSquad = squadHireListInCity[toDoSquadIndex];

                    AddSquad();

                    doQueue.RemoveAt(0);
                    curentToDoTime = 0;



                    if (isSelected)
                    {
                        mapSceneManager.uIManager.hireUIPanel.SetQueue(doQueue.ToArray(), this);

                        PriceUpdate();

                        if (doQueue.Count == 0)
                        {
                            mapSceneManager.uIManager.hireUIPanel.progressImage.gameObject.SetActive(false);
                        }
                    }


                    //toDoSquad = null;

                    //toDoSquadIndex = -1;
                }



            }
        }


            if (isSelected)
            {
                mapSceneManager.uIManager.hireUIPanel.DoProgress(1f - curentToDoTime / doHireTime);
                mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);
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
                //city.armyInCity.ArmySelected();
            }

            newArmy.gameObject.SetActive(true);
            newArmy.mapSceneManager = mapSceneManager;

            city.armyInCity.AddNewSquad(toDoSquad);


           

            
        }


        


       

    }

    public void AiDoHouse() {

       // CencelToDo();
        //doHouse = true;

        //doCount = 1;
    }

    public void AddToDoHouse()
    {
        if (doHouseCost <= resourceManager.AmountOfCoins())
        {


            doQueue.Add(-1);
            resourceManager.ChangeAmountOfCoins(-doHouseCost);

            mapSceneManager.uIManager.hireUIPanel.SetQueue(doQueue.ToArray(), this);

            PriceUpdate();

            if (isSelected && doQueue.Count <= 1)
            {
                mapSceneManager.uIManager.hireUIPanel.progressImage.gameObject.SetActive(true);
                mapSceneManager.uIManager.hireUIPanel.progressImage.transform.position = mapSceneManager.uIManager.hireUIPanel.nowToDoPanel.GetChild(0).position;
            }

            //if (doHouse)
            //{
            //    doCount++;
            //}
            //else
            //{

            //    if (doHire)
            //    {
            //        CencelToDo();
            //    }

            //    doHouse = true;

            //    doCount = 1;


            //    if (isSelected)
            //    {
            //        mapSceneManager.uIManager.hireUIPanel.CreateToDoHouse();
            //    }
            //}

            //if (isSelected)
            //{
            //    mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);


            //    
            //}

        }

    }


    public void AiToDoSquad(SquadController squad,int index)
    {

//        CencelToDo();
       // doHire = true;

       // doCount = 1;

        toDoSquad = squad;
        toDoSquadIndex = index;
    }

    public void AddToDoSquad( int index)
    {
        if (squadHireListInCity[index].coinsNeed <= resourceManager.AmountOfCoins())
        {

            if (squadHireListInCity[index].unitsNeed <= city.cityUnits)
            {

                doQueue.Add(index);

                mapSceneManager.uIManager.hireUIPanel.SetQueue(doQueue.ToArray(), this);

                city.cityUnits -= squadHireListInCity[index].unitsNeed;
                resourceManager.ChangeAmountOfCoins(-squadHireListInCity[index].coinsNeed);
                mapControlManager.uiManager.cityUIPanel.UpdateCityUIPanel(city);

                PriceUpdate();

                if (isSelected && doQueue.Count <= 1)
                {
                    mapSceneManager.uIManager.hireUIPanel.progressImage.gameObject.SetActive(true);
                    mapSceneManager.uIManager.hireUIPanel.progressImage.transform.position = mapSceneManager.uIManager.hireUIPanel.nowToDoPanel.GetChild(0).position;
                }

                //if (isSelected)
                //{
                //    city.cityUnits -= squad.unitsNeed;
                //   
                //    mapControlManager.uiManager.cityUIPanel.UpdateCityUIPanel(city);
                //}


                //if (doHire && toDoSquad == squad)
                //{
                //    doCount++;
                //}
                //else
                //{


                //        CencelToDo();


                //    doHire = true;

                //    toDoSquad = squad;
                //    toDoSquadIndex = index;

                //    doCount = 1;

                //    if (isSelected)
                //    {
                //        mapSceneManager.uIManager.hireUIPanel.CreateToDoSquad(index);
                //    }
                //}




                //if (isSelected)
                //{
                //    mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);
                //}


            }
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
            mapSceneManager.uIManager.hireUIPanel.hireButton[i].onClick.AddListener(() => this.AddToDoSquad(index));

            SquadMapUIPanel squadPanel = mapSceneManager.uIManager.hireUIPanel.hireButton[i].gameObject.GetComponent<SquadMapUIPanel>();

            squadPanel.UpdateToDoCosts(squadHireListInCity[index].unitsNeed, squadHireListInCity[index].coinsNeed);


           
        }
        PriceUpdate();

        mapSceneManager.uIManager.hireUIPanel.houseButton.onClick.AddListener(() => AddToDoHouse());

       
        mapSceneManager.uIManager.hireUIPanel.SetQueue(doQueue.ToArray(),this);

     if(doQueue.Count > 0)
        {
            mapSceneManager.uIManager.hireUIPanel.progressImage.gameObject.SetActive(true);
        }
        


            isSelected = true;

    }

    public void PriceUpdate()
    {
        bool lowCoins = false;
        bool lowUnits = false;

        for (int i = 0; i < mapSceneManager.uIManager.hireUIPanel.hireButton.Length; i++)
        {
           

            SquadMapUIPanel squadPanel = mapSceneManager.uIManager.hireUIPanel.hireButton[i].gameObject.GetComponent<SquadMapUIPanel>();

            if(squadHireListInCity[i].unitsNeed > city.cityUnits)
            {
                lowUnits = true;
            }
            else
            {
                lowUnits = false;
            }

            if (squadHireListInCity[i].coinsNeed > resourceManager.AmountOfCoins())
            {
                lowCoins = true;
            }
            else
            {
                lowCoins = false;
            }


            squadPanel.UpdateToDoPriceColors(lowUnits, lowCoins);
        }

    }
    public void CencelToDo(int index)
    {
        if (isSelected)
        {
            if (doQueue[index] == -1)
            {
                resourceManager.ChangeAmountOfCoins(doHouseCost);
            }

            if (doQueue[index] >= 0)
            {
                city.cityUnits += squadHireListInCity[doQueue[index]].unitsNeed ;
                resourceManager.ChangeAmountOfCoins(squadHireListInCity[doQueue[index]].unitsNeed);
            }

            doQueue.RemoveAt(index);


            mapSceneManager.uIManager.hireUIPanel.UpdateUI(doCount);

            mapSceneManager.uIManager.hireUIPanel.SetQueue(doQueue.ToArray(), this);

            //mapSceneManager.uIManager.hireUIPanel.DoProgress(0f);
        }

        if(index == 0)
        {
            curentToDoTime = 0;
        }

       

        }
    public void CityDeselect()
    {
        isSelected = false;


        if (toDoSquad != null)
        {

            //mapSceneManager.uIManager.hireUIPanel.DeleteToDoPanel();
        }
        mapSceneManager.uIManager.hireUIPanel.gameObject.SetActive(false);
       
        //hireUIPanel = null;
    }
}
