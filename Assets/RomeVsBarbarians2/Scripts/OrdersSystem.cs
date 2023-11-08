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
    [SerializeField] CoinsController coinsController;
    [SerializeField] private SquadController currentUnit;
    //[SerializeField] private Button attackButton;
    //[SerializeField] private Button defenceButton;
    [SerializeField]private OrderType currentOrder;
    [SerializeField] private int costOfOrder; //For basic implementation
   
    private int amountOfOrders;
    private OrderType prevOrder;

    private void Start() {
        prevOrder = currentOrder;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (coinsController.coins < costOfOrder) {
                Debug.Log("Not enough coins");
                return;
            }
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit)) {
                
                if (hit.collider.gameObject.tag == "Squad") {
                    currentUnit = hit.collider.transform.GetComponent<SquadController>();
                    TryDoOrder();
                    Debug.Log("Hit");
                }
            }
        }
    }

    private void TryDoOrder() {

        if (prevOrder == OrderType.None) {
            if (currentOrder == OrderType.Attack) { 
            
            }
            if (currentOrder == OrderType.Defence) { 
            
            }
        }
        if (prevOrder == OrderType.Attack) {
            if (currentOrder == OrderType.Attack) {

            }
            if (currentOrder == OrderType.Defence) {

            }
        }
        if (prevOrder == OrderType.Defence) {
            if (currentOrder == OrderType.Attack) {

            }
            if (currentOrder == OrderType.Defence) {

            }
        }
        prevOrder = currentOrder;
        coinsController.coins -= costOfOrder;
    }

    public void AttackButtonClick() { 
        currentOrder = OrderType.Attack;
    }
    public void DefenceButtonClick() {
        currentOrder = OrderType.Defence;
    }
}
