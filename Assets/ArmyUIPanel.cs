using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;

public class ArmyUIPanel : MonoBehaviour
{

    [SerializeField] public Transform squadPanel;
    

    [SerializeField] public List<SquadMapUIPanel> squadMapUIPanelList;

    
    // Start is called before the first frame update
    public SquadMapUIPanel AddSquad(SquadMapUIPanel panel)
    {
        SquadMapUIPanel newSquad = Instantiate(panel, squadPanel);
        squadMapUIPanelList.Add(newSquad);

        return newSquad;
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
