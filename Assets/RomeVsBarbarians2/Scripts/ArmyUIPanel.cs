using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ArmyUIPanel : MonoBehaviour
{

    [SerializeField] public Transform squadPanel;
    

    [SerializeField] public List<SquadMapUIPanel> squadMapUIPanelList;

    [SerializeField] public TextMeshProUGUI armyUnitsCountText;
    [SerializeField] public TextMeshProUGUI armyPowerCountText;

    [SerializeField] public Button leavTheCityButton;

    [SerializeField] public TextMeshProUGUI armySquadCountText;

    public void PanelInit(ArmyController amry)
    {
        if (amry.inCity)
        {
            leavTheCityButton.gameObject.SetActive(true);

            leavTheCityButton.onClick.RemoveAllListeners();

            leavTheCityButton.onClick.AddListener(() => amry.ExitFromCity());
        }
        else
        {
            leavTheCityButton.gameObject.SetActive(false);
        }
    }

        // Start is called before the first frame update
        public SquadMapUIPanel AddSquad(SquadMapUIPanel panel)
    {

        SquadMapUIPanel newSquad = Instantiate(panel, squadPanel);
        squadMapUIPanelList.Add(newSquad);

        return newSquad;
    }

    public void UpdateArmyInfo( float unitsCount, float powerCount, float squadCount)
    {


        armyUnitsCountText.text = "" + unitsCount;
        armyPowerCountText.text = "" + powerCount;
        armySquadCountText.text = "" + squadCount + "/20";

    }


    public void UpdateSquadsInfo(SquadController[] squadList)
    {
//        Debug.Log("squadList count - " + squadList.Length);
        for (int i = 0; i < squadMapUIPanelList.Count; i++)
        {
            if(squadList.Length <= i) { return; }
            if (squadList[i] == null) { return; }
            if (squadMapUIPanelList.Count == 0) { return; }
            squadMapUIPanelList[i].UpdateSquadUiPanel(squadList[i]);
        }

       
    }
    public void UpdateSquadsIndicators(SquadController[] squadList)
    {
        for (int i = 0; i < squadMapUIPanelList.Count; i++)
        {
            if (squadList.Length <= i) { return; }
            if (squadList[i] == null) { return; }
            if (squadMapUIPanelList.Count == 0) { return; }
            squadMapUIPanelList[i].UpdateBattleIndicators(squadList[i]);


        }
    }

        public void Clear()
    {
        for (int i = 0; i < squadMapUIPanelList.Count; i++)
        {
            Destroy(squadMapUIPanelList[i].gameObject);
        }
        squadMapUIPanelList.Clear();
    }

}
