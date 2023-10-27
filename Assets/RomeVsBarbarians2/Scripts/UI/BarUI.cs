using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BarType { Stamina, Morale };
public class BarUI : MonoBehaviour
{
    [SerializeField] private GameObject squad;
    [SerializeField] private Image barImage;
    public BarType barType;
    private SquadController squadController;

    private void Awake() {
        squadController = squad.GetComponent<SquadController>();
    }

    private void Update() {
        if (squadController != null) {
            if (barType == BarType.Stamina) { 
                ChangeProgress(squadController.currentStamina, squadController.maxStamina);
            }
            if (barType == BarType.Morale) {
                ChangeProgress(squadController.currentMorale, squadController.moraleSquad);
            }
        }
    }

    public void ChangeProgress(float current, float maximum) { 
        barImage.fillAmount = current/maximum;
    }
}
