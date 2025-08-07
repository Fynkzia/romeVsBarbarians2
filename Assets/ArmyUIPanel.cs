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

    public void UpdateSquadsInfo(SquadController[] squadList, float unitsCount, float powerCount)
    {
        Debug.Log("squadList count - " + squadList.Length);
        for (int i = 0; i < squadList.Length; i++)
        {
            squadMapUIPanelList[i].UpdateSquadUiPanel(squadList[i]);
        }

        armyUnitsCountText.text = "" + unitsCount;
        armyPowerCountText.text = "" + powerCount;

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
