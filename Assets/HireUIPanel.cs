using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HireUIPanel : MonoBehaviour
{
    [SerializeField] public GameObject toDoPanel;

    [SerializeField] public TextMeshProUGUI nowDoCount;

    [SerializeField] public GameObject doCountPanel;

    [SerializeField] public List<Button> queueButtons;

    [SerializeField] public Button houseButton;
    [SerializeField] public Button[] hireButton;
    [SerializeField] public Transform nowToDoPanel;

    [SerializeField] public TextMeshProUGUI nowUnits;
    [SerializeField] public TextMeshProUGUI nowCoins;
    [SerializeField] public Button nowToDoButton;

    [SerializeField] public Button plusDoButton;
    [SerializeField] public Button minusDoButton;
    [SerializeField] public Button closePanelButton;

    [SerializeField] public Image progressImage;
   // Start is called before the first frame update
   void Start()
    {
        
    }
    public void SetQueue(int[] queue, CityToDoController toDoController)
    {

        queueButtons.Clear();

        for (int i = 0; i < nowToDoPanel.childCount; i++)
        {
            Destroy(nowToDoPanel.GetChild(i).gameObject);
        }


            for (int i = 0; i < queue.Length; i++)
        {
            if (queue[i] == -1)
            {
                CreateToDoHouse();
            }
            else if (queue[i] >= 0)
            {
                CreateToDoSquad(queue[i]);
            }
        }


        for (int i = 0; i < queue.Length; i++)
        {
            int index = i;
                queueButtons[i].onClick.RemoveAllListeners();
               queueButtons[i].onClick.AddListener(() => toDoController.CencelToDo(index));

            
        }


        Button newTodDo = Instantiate(nowToDoButton, nowToDoPanel.transform);

        newTodDo.gameObject.SetActive(true);


    }


    // Update is called once per frame
    public void CreateToDoSquad(int index)
    {
        Button newTodDo = Instantiate(hireButton[index], nowToDoPanel.transform);

        //newTodDo.transform.position = nowToDoPos.transform.position;
        queueButtons.Add(newTodDo);


       // nowToDoButton = newTodDo;
    }

    public void CreateToDoHouse()
    {
        Button newTodDo = Instantiate(houseButton, nowToDoPanel.transform);

        queueButtons.Add(newTodDo);
        //newTodDo.transform.position = nowToDoPos.transform.position;


        //nowToDoButton = newTodDo;
    }

   

    public void OpenToDoPanel()
    {
        if (toDoPanel.activeSelf)
        {
            toDoPanel.SetActive(false);
        }
        else
        {
            toDoPanel.SetActive(true);
        }
    }

    public void CloseToDoPanel()
    {
        toDoPanel.SetActive(false);
    }

    public void UpdateUI(int doCount)
    {
 


    nowDoCount.text = "x" + doCount;

        if(doCount > 0)
        {
            doCountPanel.gameObject.SetActive(true);
            minusDoButton.gameObject.SetActive(true);
            plusDoButton.gameObject.SetActive(true);
        }
        else
        {
            doCountPanel.gameObject.SetActive(false);
            minusDoButton.gameObject.SetActive(false);
            plusDoButton.gameObject.SetActive(false);
        }

    }

    public void DoProgress( float doProgress)
    {

        progressImage.fillAmount = doProgress;


      

    }
}
