using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OfficerUIPanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public Button panelButton;

    [SerializeField] public Image officerIcon;
    [SerializeField] public TextMeshProUGUI officerName;

    [SerializeField] public Image squadTipeIcon;
    [SerializeField] public TextMeshProUGUI squadCount;

    [SerializeField] public Button skillButton;
    [SerializeField] public Image skillimage;
    [SerializeField] public Image cdImage;

    [SerializeField] public Animator panelAnimation;

    public SquadController squadController;

     void Start()
    {
        squadController.OnUnitsCountChange += UpdateSquadCunt;
        squadController.OnSquadDie += DisablePanel;
    }

    public void UpdateCDImage(float fill)
    {
        cdImage.fillAmount = fill;
    }

    public void SkillRedy(bool redy)
    {
        panelAnimation.SetBool("SkillRedy", redy);

        if(redy == true)
        {
            panelAnimation.SetTrigger("UpdateSkill");
        }
    }

    public void OfficerSelect(bool select)
    {
        panelAnimation.SetBool("Select", select);
        panelAnimation.SetTrigger("UpdateSelect");

    }

    public void UpdateSquadCunt(int currentCount)
    {

        squadCount.text = "" + currentCount;
    }

    public void DisablePanel()
    {

        gameObject.SetActive(false);
    }



}
