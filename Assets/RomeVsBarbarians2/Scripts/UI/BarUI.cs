using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BarType { Stamina, Morale };
public class BarUI : MonoBehaviour
{
    [SerializeField] private bool isUiBar;
    [SerializeField] private GameObject squad;
    [SerializeField] private Image barImage;
    [SerializeField] private Image backImage;
    
    [SerializeField] private Gradient gradient;
    private SquadController squadController;

    private void Awake() {
        if (!isUiBar)
        {
            squadController = squad.GetComponent<SquadController>();

            Vector2 size = barImage.rectTransform.sizeDelta;
            Vector2 offset = new Vector2(0f, 0f);
            size.x += Mathf.Pow(squadController.maxMorale / 3f, 3f);
            //size.y = 0.4f + Mathf.Pow(squadController.maxMorale / 35f,1.3f);

            barImage.rectTransform.sizeDelta = size;
            backImage.rectTransform.sizeDelta = size + offset;
        }
    }

    

    public void ChangeProgress(float current, float maximum) { 
        barImage.fillAmount = current/maximum;
        barImage.color = gradient.Evaluate(current / maximum);
    }
}
