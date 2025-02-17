using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HireController : MonoBehaviour
{
    CityController city;
    public CoinsController coinsController;

    public SquadController[] squadHireListInCity;
    public SquadController toDoSquad;
    private int toDoSquadIndex;

    public bool isSelected;

    public bool ToDo;
    public float ToDoTime;
    private float curentToDoTime;

    private int unitsNeed;
    private int coinsNeed;

    public HireUIPanel hireUIPanel;
    public MapControlManager mapControlManager;
    // Start is called before the first frame update
    void Start()
    {
        city = GetComponent<CityController>();
        coinsController = GameObject.Find("CoinsController").GetComponent<CoinsController>();
        mapControlManager = GameObject.Find("MapControlManager").GetComponent<MapControlManager>();
        hireUIPanel = mapControlManager.hireUIPanel.GetComponent<HireUIPanel>();
    }

    // Update is called once per frame
        void Update()
    {
        if (ToDo)
        {
            curentToDoTime += Time.deltaTime;

            if (curentToDoTime > ToDoTime)
            {
                if (unitsNeed > 0 && city.cityUnits > 0)
                {
                    unitsNeed--;
                    city.cityUnits--;
                }
                if (coinsNeed > 0)//&& coinsController.AmountOfCoins() > 0
                {
                    coinsNeed--;
                    //coinsController.ChangeAmountOfCoins(-1) ;
                }

                if (isSelected)
                {
                    if (hireUIPanel != null)
                    {
                        hireUIPanel.UpdateUI(unitsNeed, coinsNeed);
                    }

                }
                

                if(coinsNeed <= 0 && unitsNeed <= 0)
                {
                    if(toDoSquad != null)
                    {
                        AddSquad();
                        ToDo = false;
                        curentToDoTime = 0;
                    }
                }
                curentToDoTime = 0;

            }
        }
    }

    void AddSquad()
    {
        if(city.armyInCity != null)
        {
            city.armyInCity.AddSquad(toDoSquad, toDoSquadIndex);
        }
        else
        {
            
                ArmyController newArmy = Instantiate(city.armyObject.gameObject, transform).GetComponent<ArmyController>();
                city.armyInCity = newArmy;

                newArmy.gameObject.SetActive(false);
            
            city.armyInCity.AddSquad(toDoSquad, toDoSquadIndex);
            city.armyInCity.UpdateSquadsList();
        }


       

       
            hireUIPanel.DeleteToDoPanel();
        

        toDoSquad = null;

        toDoSquadIndex = -1;

    }

    public void AddToDoSquad(SquadController squad, int index)
    {
        toDoSquad = squad;
        coinsNeed = squad.coinsNeed;
        unitsNeed = squad.unitsNeed;

        hireUIPanel.CreateToDo(index);
        hireUIPanel.UpdateUI(unitsNeed, coinsNeed);

        toDoSquadIndex = index;

        ToDo = true;

    }

    public void CitySelected()
    {
        for (int i = 0; i < hireUIPanel.hireButton.Length; i++)
        {
            int index = i;
            hireUIPanel.hireButton[i].onClick.AddListener(() => this.AddToDoSquad(this.squadHireListInCity[index], index));
            Debug.Log("hireButton index"+ i, hireUIPanel.hireButton[i].gameObject);
                }

        if (toDoSquad != null)
        {
            hireUIPanel.CreateToDo(toDoSquadIndex);
        }
         

        
        isSelected = true;

    }

    public void CityDeselect()
    {
        isSelected = false;
        

        if (toDoSquad != null)
        {
            hireUIPanel.DeleteToDoPanel() ;
        }
        //hireUIPanel = null;
    }
}
