using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum OrderType {
    None,
    Attack,
    Defence
}
[System.Serializable]
public class OrderTimeCoef {
    public int numberOfOrders;
    public float timeOfExpire;
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
    [SerializeField] public OrderTimeCoef[] ordersTime;
    [Space(10)]
    [SerializeField]private float orderTime = 0;

    [SerializeField]private int amountOfOrders = 0;
    private OrderType prevOrder;
    private bool orderSelected = false;

    private void Start() {
        prevOrder = currentOrder;
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (coinsController.coins < costOfOrder) {
                Debug.Log("Not enough coins");
                return;
            }
            if (orderSelected) { 
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit)) {
                    
                    if (hit.collider.gameObject.tag == "Squad") {
                        currentUnit = hit.collider.transform.GetComponent<SquadController>();
                        DoOrder();
                        Debug.Log("Hit");
                    }
                }
            }
        }

        if (amountOfOrders > 0) {
            OrderTimer();
        }
    }

    private void DoOrder() {

        if (prevOrder == OrderType.None) {
            if (currentOrder == OrderType.Attack) {
                
            }
            if (currentOrder == OrderType.Defence) { 
            
            }
            amountOfOrders = 1;
        }
        if (prevOrder == OrderType.Attack) {
            if (currentOrder == OrderType.Attack) {
                amountOfOrders++;
            }
            if (currentOrder == OrderType.Defence) {
                amountOfOrders = 1;
            }
        }
        if (prevOrder == OrderType.Defence) {
            if (currentOrder == OrderType.Attack) {
                amountOfOrders = 1;
            }
            if (currentOrder == OrderType.Defence) {
                amountOfOrders++;
            }
        }
        prevOrder = currentOrder;
        coinsController.coins -= costOfOrder;
        orderSelected = false;
        orderTime = ordersTime[amountOfOrders - 1].timeOfExpire;
    }

    private void OrderTimer() {
        if (orderTime > 0) {
            orderTime -= Time.deltaTime;
        }
        else {
            amountOfOrders--;
            if (amountOfOrders == 0) { 
                orderTime = ordersTime[amountOfOrders - 1].timeOfExpire;
            }
        }
    }

    public void AttackButtonClick() { 
        currentOrder = OrderType.Attack;
        orderSelected = true;
    }
    public void DefenceButtonClick() {
        currentOrder = OrderType.Defence;
        orderSelected = true;
    }
}
