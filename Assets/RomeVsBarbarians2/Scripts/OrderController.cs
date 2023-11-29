using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OrderTimeCoef {
    public int numberOfOrders;
    public float timeOfExpire;
}

[System.Serializable]
public class AttackOrder {
    public float defenceSquad;
    public float powerSquad;
}
[System.Serializable]
public class DefenceOrder {
    public float defenceSquad;
    public float actionTime;
}


public class OrderController : MonoBehaviour {

    [SerializeField] private int amountOfOrders = 0;
    [SerializeField] public OrderTimeCoef[] ordersTime;
    [Space(10)]
    [SerializeField] private float orderTime = 0;
    [SerializeField] public AttackOrder attackOrder;
    [SerializeField] public DefenceOrder defenceOrder;
    private SquadController squadController;
    private OrderType prevOrder;
    private List<int> shieldUnits = new List<int>();

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
                squadController.defence = false;
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
                return;
            }
            orderTime = ordersTime[amountOfOrders - 1].timeOfExpire;
        }
    }

    private void AddAttackOrder() {
        if (squadController.unitArray.Count >= squadController.maxFightingUnit + 3) { 
            squadController.actionUnits += 3;
            squadController.maxFightingUnit += 3;
        }
        squadController.defenceSquad += attackOrder.defenceSquad;
        squadController.powerSquad += attackOrder.powerSquad;
        squadController.attack = true;
    }
    private void RemoveAttackOrder() {
        if (squadController.actionUnits >= 6) { 
            squadController.actionUnits -= 3;
            squadController.maxFightingUnit -= 3;
        }
        squadController.defenceSquad -= attackOrder.defenceSquad;
        squadController.powerSquad -= attackOrder.powerSquad;
    }
    private void AddDefenceOrder() {
        if (squadController.unitArray.Count >= squadController.actionUnits + 3) { 
            squadController.actionUnits += 3;
            squadController.maxFightingUnit += 3;
            int i = 0;
            while(i < 3) {
                int randomUnit = Random.Range(0, squadController.unitArray.Count);
                if (!shieldUnits.Contains(randomUnit)) { 
                    shieldUnits.Add(randomUnit);
                    squadController.SetShield(randomUnit, true);
                    i++;
                }
            }
        } 
        squadController.defenceSquad += defenceOrder.defenceSquad;
        squadController.actionTime += defenceOrder.actionTime;
        squadController.defence = true;
    }
    private void RemoveDefenceOrder() {
        if (squadController.actionUnits >= 6) {
            int i = 0;
            while (i < 3) {
                squadController.SetShield(shieldUnits[shieldUnits.Count-1], false);
                shieldUnits.RemoveAt(shieldUnits.Count-1);
                i++;
            }
            squadController.actionUnits -= 3;
            squadController.maxFightingUnit -= 3;
        }
        squadController.defenceSquad -= defenceOrder.defenceSquad;
        squadController.actionTime -= defenceOrder.actionTime;
    }
}
