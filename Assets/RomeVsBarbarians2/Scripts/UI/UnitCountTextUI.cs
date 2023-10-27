using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitCountTextUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    [SerializeField] private GameObject squad;
    private SquadController squadController;

    private void Awake() {
        squadController = squad.GetComponent<SquadController>();
    }
    private void Update() {
        text.text = squadController.unitArray.Count.ToString();
    }
}
