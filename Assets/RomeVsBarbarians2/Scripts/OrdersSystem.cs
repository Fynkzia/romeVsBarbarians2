using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum OrderType {
    None,
    Attack,
    Defence
}
public class OrdersSystem : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private CoinsController coinsController;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button defenceButton;

    [SerializeField] private Sprite activeButtonImage;
    [SerializeField] private Sprite selectedButtonImage;
    
    [SerializeField] private int costOfOrder; //For basic implementation


    [SerializeField]private OrderType currentOrder;
    [SerializeField]private bool orderSelected = false;
    [SerializeField]private bool prevCoinsState;
    [SerializeField]private bool currentCoinsState;
    private Image attackButtonImage;
    private Image defenceButtonImage;

    private void Start() {
        prevCoinsState = coinsController.AmountOfCoins() >= costOfOrder;
        attackButtonImage = attackButton.GetComponent<Image>();
        defenceButtonImage = defenceButton.GetComponent<Image>();
    }

    private void Update() {
        currentCoinsState = coinsController.AmountOfCoins() >= costOfOrder;
        if (currentCoinsState != prevCoinsState) {
            if (currentCoinsState) {
                attackButton.interactable = true;
                defenceButton.interactable = true;
            } else { 
                UnSelectButtons();
                attackButton.interactable = false;
                defenceButton.interactable = false;
            }
            prevCoinsState = currentCoinsState;
        }

        if (Input.GetMouseButtonDown(0)) {

            if (!currentCoinsState) {
                Debug.Log("Not enough coins"); 
                return; 
            }

            if (orderSelected) { 
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    
                    if (hit.collider.gameObject.tag == "Squad") {
                        OrderController currentUnit = hit.collider.transform.GetComponent<OrderController>();
                        currentUnit.DoOrder(currentOrder);
                        coinsController.ChangeAmountOfCoins(-costOfOrder);
                        
                        Debug.Log("Hit");
                    } else if (!EventSystem.current.IsPointerOverGameObject()) {
                        UnSelectButtons();
                    }
                }
            }
        }
    }

    public void AttackButtonClick() {
        if (orderSelected && currentOrder == OrderType.Attack) {
            orderSelected = false;
            attackButtonImage.sprite = activeButtonImage;
        } else { 
            currentOrder = OrderType.Attack;
            orderSelected = true;
            attackButtonImage.sprite = selectedButtonImage;
            defenceButtonImage.sprite = activeButtonImage;
        }
    }
    public void DefenceButtonClick() {
        if (orderSelected && currentOrder == OrderType.Defence) {
            orderSelected = false;
            defenceButtonImage.sprite = activeButtonImage;
        } else { 
            currentOrder = OrderType.Defence;
            orderSelected = true;
            defenceButtonImage.sprite = selectedButtonImage;
            attackButtonImage.sprite = activeButtonImage;
        }
    }

    private void UnSelectButtons() {
        orderSelected = false;
        attackButtonImage.sprite = activeButtonImage;
        defenceButtonImage.sprite = activeButtonImage;
    }
}
