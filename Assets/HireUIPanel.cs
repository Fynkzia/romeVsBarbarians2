using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HireUIPanel : MonoBehaviour
{

    [SerializeField] public Button[] hireButton;
    [SerializeField] public Transform nowToDoPos;

    [SerializeField] public TextMeshProUGUI nowUnits;
    [SerializeField] public TextMeshProUGUI nowCoins;
    [SerializeField] public Button nowToDoButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void CreateToDo(int index)
    {
        Button newTodDo = Instantiate(hireButton[index], transform);

        newTodDo.transform.position = nowToDoPos.transform.position;

        nowUnits = newTodDo.GetComponent<SquadMapUIPanel>().nowUnits;
        nowCoins = newTodDo.GetComponent<SquadMapUIPanel>().nowCoins;

        nowToDoButton = newTodDo;
    }

    public void DeleteToDoPanel()
    {
        if(nowToDoButton != null)
        Destroy(nowToDoButton.gameObject);
    }

    public void UpdateUI(int units,int coins)
    {
        nowUnits.text = "" + units;
        nowCoins.text = "" + coins;

    }
}
