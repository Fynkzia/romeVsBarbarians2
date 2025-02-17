using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficerSystem : MonoBehaviour
{
    [SerializeField] public Transform playerSquadsContainer;

    [SerializeField] public List<SquadController> allPlayerList;

    [SerializeField] public List<OfficerController> allOfficersList;

    [SerializeField] public Transform uiContainer;

    [SerializeField] public OfficerUIPanel officerUIPrefub;

    [SerializeField] public List<OfficerUIPanel> allOfficersUIPanelList;

    int officersCount = 0;

    // Start is called before the first frame update
    public void Init()
    {
        SetAllPlayersList();
        //SetAllOfficerPanels();


    }

    

    public void SetAllPlayersList()
    {
       // allOfficersList.Clear();
        allPlayerList = new List<SquadController>();

        for (int i = 0; i < playerSquadsContainer.transform.childCount; i++)
        {
            allPlayerList.Add(playerSquadsContainer.transform.GetChild(i).GetComponent<SquadController>());

            if (playerSquadsContainer.transform.GetChild(i).TryGetComponent(out OfficerController myComponent))
            {
                //myComponent = gameObject.GetComponent<OfficerController>();
                //allOfficersList.Add(myComponent);

                officersCount++;
            }
        }


    }

    public void SetAllOfficerPanels()
    {

        allOfficersUIPanelList.Clear();

        for (int i = 0; i < allOfficersList.Count; i++)
        {
            OfficerUIPanel newPanel = Instantiate(officerUIPrefub, uiContainer).GetComponent<OfficerUIPanel>();

           

           allOfficersList[i].officerPanel = newPanel;
            allOfficersUIPanelList.Add(newPanel);


            allOfficersUIPanelList[i].officerIcon.sprite = allOfficersList[i].officerSpecs.officerIcon;
            allOfficersUIPanelList[i].officerName.text = allOfficersList[i].officerSpecs.officerName;

            allOfficersUIPanelList[i].squadCount.text = ""+allOfficersList[i].squadController.currentAmountUnits;
            allOfficersUIPanelList[i].skillimage.sprite = allOfficersList[i].officerSpecs.skillIcon;

            int index = i;
            allOfficersUIPanelList[index].skillButton.onClick.AddListener(() => allOfficersList[index].SkillActivation()) ;
            allOfficersUIPanelList[index].panelButton.onClick.AddListener(() => allOfficersList[index].OfficerPanelSelect());

            allOfficersUIPanelList[i].squadController = allOfficersList[i].squadController;
        }


    }

    public void OfficerInit(OfficerController newOfficer)
    {
        allOfficersList.Add(newOfficer);

        if(allOfficersList.Count == officersCount)
        {
            SetAllOfficerPanels();
        }
    }
}
