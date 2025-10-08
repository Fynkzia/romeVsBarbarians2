using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SquadStatsUIPanel : MonoBehaviour
{
    [SerializeField] public GameObject SquadUIpanel;
    [SerializeField] public GameObject SquadUIpanelObject;
    [SerializeField] public TextMeshProUGUI typeText;

    [SerializeField] public Image typeIcon;

    [SerializeField] public String[] typeName;

    [SerializeField] public TextMeshProUGUI nameText;

    [SerializeField] public TextMeshProUGUI countText;
    [SerializeField] public TextMeshProUGUI powerText;
    [SerializeField] public TextMeshProUGUI defenceText;
    [SerializeField] public TextMeshProUGUI moraleText;
    [SerializeField] public TextMeshProUGUI speedText;

    [SerializeField] public TextMeshProUGUI powerBonusText;
    [SerializeField] public TextMeshProUGUI defenceBonusText;

    [SerializeField] public Button closeButton;


    // Start is called before the first frame update
   

    // Update is called once per frame
    public void UpdateStatUIPanle(SquadController squad)
    {
        countText.text = "" + squad.amountUnits;
        powerText.text = "" + squad.powerSquad;
        defenceText.text = "" + squad.defenceSquad;
        moraleText.text = "" + squad.maxMorale;
        speedText.text = "" + squad.movementSpeed;

        powerBonusText.text = "" + squad.powerlevelBonus;
        defenceBonusText.text = "" + squad.defencelevelBonus;

        nameText.text = "" + squad.squadName;

        if(SquadUIpanel != null)
        {
            Destroy(SquadUIpanel.gameObject);
        }

        GameObject newSquad = Instantiate(squad.mapUIPanel, transform).gameObject;

        newSquad.transform.position = SquadUIpanelObject.transform.position;

        SquadUIpanel = newSquad;

        typeIcon.sprite = squad.squadTypeSprite;
        typeText.text = typeName[(int)squad.type];
    }

   

}
