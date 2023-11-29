using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    
    [SerializeField] private int costOfOrder; //For basic implementation


    private OrderType currentOrder;
    private bool orderSelected = false;

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (coinsController.AmountOfCoins() < costOfOrder) {
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
                        orderSelected = false;
                        Debug.Log("Hit");
                    }
                }
                attackButton.interactable = true;
                defenceButton.interactable = true;
            }
        }
    }

    public void AttackButtonClick() { 
        currentOrder = OrderType.Attack;
        orderSelected = true;
        attackButton.interactable = false;
    }
    public void DefenceButtonClick() {
        currentOrder = OrderType.Defence;
        orderSelected = true;
        defenceButton.interactable = false;
    }
}
