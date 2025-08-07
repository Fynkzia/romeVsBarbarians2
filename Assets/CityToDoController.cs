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

    public HireUIPanel hireUIPanel;
    public MapControlManager mapControlManager;

    void Start()
    {
        city = GetComponent<CityController>();

        mapControlManager = GameObject.Find("MapControlManager").GetComponent<MapControlManager>();
        hireUIPanel = mapControlManager.uiManager.hireUIPanel;

        

       
    }

    void Update()
    {
        if (doHouse || doHire)
        {
            curentToDoTime += Time.deltaTime;

            
        }

        if (doHouse)
        {
            hireUIPanel.DoProgress(1f-curentToDoTime/ doHouseTime);

            if (curentToDoTime > doHouseTime)
            {

                city.AddBuildings();

                doCount--;
                curentToDoTime = 0;

                if(doCount <= 0)
                {
                    doHouse = false;

                    hireUIPanel.DeleteToDoPanel();
                }
                hireUIPanel.UpdateUI(doCount);
            }
        }

        if (doHire)
        {
            hireUIPanel.DoProgress(1f - curentToDoTime / doHireTime);

            if (curentToDoTime > doHireTime)
            {
                AddSquad();

                doCount--;
                curentToDoTime = 0;

                if (doCount <= 0)
                {
                    doHire = false;

                    hireUIPanel.DeleteToDoPanel();


                    toDoSquad = null;

                    toDoSquadIndex = -1;
                }

                hireUIPanel.UpdateUI(doCount);

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

            ArmyController newArmy = Instantiate(city.armyObject.gameObject, city.armyPoint).GetComponent<ArmyController>();
            city.armyInCity = newArmy;
            city.armyInCity.inCity = true;

            city.armyInCity.city = city;

            if (isSelected)
            {
                city.armyInCity.ArmySelected();
            }

            newArmy.gameObject.SetActive(true);

            city.armyInCity.AddNewSquad(toDoSquad);


           

            
        }


        


       

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

                hireUIPanel.CreateToDoHouse();
            }

            hireUIPanel.UpdateUI(doCount);
            resourceManager.ChangeAmountOfCoins(-doHouseCost);
           
        }

    }

        public void AddToDoSquad(SquadController squad, int index)
    {
        if (squad.coinsNeed <= resourceManager.AmountOfCoins())
        {

            if (squad.unitsNeed <= city.cityUnits)
            {
                city.cityUnits -= squad.unitsNeed;
                resourceManager.ChangeAmountOfCoins(-squad.coinsNeed);

                

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

                    hireUIPanel.CreateToDoSquad(index);
                }





                hireUIPanel.UpdateUI(doCount);
             

            }
        }
    

    }

    public void CitySelected()
    {
        hireUIPanel.gameObject.SetActive(true);

        hireUIPanel.houseButton.onClick.RemoveAllListeners();
        hireUIPanel.cancelToDoButton.onClick.RemoveAllListeners();

        for (int i = 0; i < hireUIPanel.hireButton.Length; i++)
        {
            int index = i;
            hireUIPanel.hireButton[i].onClick.RemoveAllListeners();
            hireUIPanel.hireButton[i].onClick.AddListener(() => this.AddToDoSquad(this.squadHireListInCity[index], index));

            SquadMapUIPanel squadPanel = hireUIPanel.hireButton[i].gameObject.GetComponent<SquadMapUIPanel>();

            squadPanel.UpdateToDoCosts(squadHireListInCity[index].unitsNeed, squadHireListInCity[index].coinsNeed);


            //squadPanel.UpdateToDoPriceColors(squadHireListInCity[index].unitsNeed, squadHireListInCity[index].coinsNeed);
        }
    

        hireUIPanel.houseButton.onClick.AddListener(() => AddToDoHouse());
        hireUIPanel.cancelToDoButton.onClick.AddListener(() => CencelToDo());

        if (toDoSquad != null)
        {
            hireUIPanel.CreateToDoSquad(toDoSquadIndex);
        }

        if (doHouse)
        {
            hireUIPanel.CreateToDoHouse();
        }



        isSelected = true;

    }

    public void CencelToDo()
    {
        if (doHouse || doHire)
        {
            doHouse = false;
            doHire = false;
            doTower = false;
            doWall = false;

            hireUIPanel.DeleteToDoPanel();

            doCount = 0;
            curentToDoTime = 0;

           

            toDoSquad = null;

            toDoSquadIndex = -1;

            hireUIPanel.UpdateUI(doCount);

            hireUIPanel.DoProgress(0f);
        }


        }
    public void CityDeselect()
    {
        isSelected = false;


        if (toDoSquad != null)
        {
            hireUIPanel.DeleteToDoPanel();
        }
        hireUIPanel.gameObject.SetActive(false);
       
        //hireUIPanel = null;
    }
}
