using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderController : MonoBehaviour {

    [SerializeField] private int amountOfOrders = 0;
    [SerializeField] public OrderTimeCoef[] ordersTime;
    [Space(10)]
    [SerializeField] private float orderTime = 0;
    private SquadController squadController;
    private OrderType prevOrder;

    private void Start () {
        squadController = GetComponent<SquadController>();
        prevOrder = OrderType.None;
    }

    private void Update () {
        if (amountOfOrders > 0) {
            OrderTimer();
        }
    }
    public void DoOrder(OrderType currentOrder) {

        if (prevOrder == OrderType.None) {
            if (currentOrder == OrderType.Attack) {
                AddAttackOrder();
            }
            if (currentOrder == OrderType.Defence) {
                AddDefenceOrder();
            }
            amountOfOrders = 1;
        }
        if (prevOrder == OrderType.Attack) {
            if (currentOrder == OrderType.Attack && amountOfOrders < ordersTime.Length) {
                AddAttackOrder();
                amountOfOrders++;
            }
            if (currentOrder == OrderType.Defence) {
                RemoveAttackOrder();
                squadController.attack = false;
                AddDefenceOrder();
                amountOfOrders = 1;
            }
        }
        if (prevOrder == OrderType.Defence) {
            if (currentOrder == OrderType.Attack) {
                RemoveDefenceOrder();
                squadController.defence = true;
                AddAttackOrder();
                amountOfOrders = 1;
            }
            if (currentOrder == OrderType.Defence && amountOfOrders < ordersTime.Length) {
                AddDefenceOrder();
                amountOfOrders++;
            }
        }
        prevOrder = currentOrder;
        orderTime = ordersTime[amountOfOrders - 1].timeOfExpire;
    }

    private void OrderTimer() {
        if (orderTime > 0) {
            orderTime -= Time.deltaTime;
        }
        else {
            if (prevOrder == OrderType.Attack) { RemoveAttackOrder(); }
            if (prevOrder == OrderType.Defence) { RemoveDefenceOrder(); }

            amountOfOrders--;
            if (amountOfOrders == 0) {
                prevOrder = OrderType.None;
                squadController.attack = false;
                squadController.defence = false;
                orderTime = 0;
            }
        }
    }

    private void AddAttackOrder() {
        if (squadController.unitArray.Count <= squadController.maxFightingUnit + 3) { 
            squadController.actionUnits += 3;
            squadController.maxFightingUnit += 3;
        }
        squadController.defenceSquad -= 0.1f;
        squadController.powerSquad += 0.05f;
        squadController.attack = true;
    }
    private void RemoveAttackOrder() {
        squadController.actionUnits -= 3;
        squadController.maxFightingUnit -= 3;
        squadController.defenceSquad += 0.1f;
        squadController.powerSquad -= 0.05f;
    }
    private void AddDefenceOrder() {
        if (squadController.unitArray.Count <= squadController.actionUnits + 3) { 
            squadController.actionUnits += 3;
        } 
        squadController.defenceSquad += 0.1f;
        squadController.actionTime -= 0.1f;
        squadController.defence = true;
    }
    private void RemoveDefenceOrder() {
        squadController.actionUnits -= 3;
        squadController.defenceSquad -= 0.1f;
        squadController.actionTime += 0.1f;
    }
}
