using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BarUI : MonoBehaviour
{
    [SerializeField] private GameObject squad;
    [SerializeField] private Image barImage;
    [SerializeField] private Camera camera;
    private SquadController squadController;

    private void Awake() {
        squadController = squad.GetComponent<SquadController>();
    }

    private void Update() {
        if (squadController != null) {
            ChangeProgress(squadController.currentStamina, squadController.maxStamina);
        }
    }

    private void LateUpdate() {
        transform.LookAt(camera.transform);
    }

    public void ChangeProgress(float current, float maximum) { 
        barImage.fillAmount = current/maximum;
    }
}
