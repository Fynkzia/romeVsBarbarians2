using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
    [Space(10)]
    [SerializeField] private GameObject orderIndicator;
    [SerializeField] private Image bar;
    [SerializeField] private TextMeshProUGUI barText;
    [SerializeField] private Image orderImage;
    [SerializeField] private ParticleSystem orderFx;

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
    public void DoOrder(OrderType currentOrder, Sprite orderSprite) {
        Instantiate(orderFx, gameObject.transform);
        if (prevOrder == OrderType.None) {
            orderIndicator.SetActive(true);
            if (currentOrder == OrderType.Attack) {
                AddAttackOrder();
                orderImage.sprite = orderSprite;
            }
            if (currentOrder == OrderType.Defence) {
                AddDefenceOrder();
                orderImage.sprite = orderSprite;
            }
            amountOfOrders = 1;
            ChangeText();
        }

        if (prevOrder == OrderType.Attack) {
            if (currentOrder == OrderType.Attack && amountOfOrders < ordersTime.Length) {
                AddAttackOrder();
                amountOfOrders++;
                ChangeText();
            }
            if (currentOrder == OrderType.Defence) {
                RemoveAttackOrder();
                squadController.attack = false;
                AddDefenceOrder();
                amountOfOrders = 1;
                ChangeText();
                orderImage.sprite = orderSprite;
            }
        }

        if (prevOrder == OrderType.Defence) {
            if (currentOrder == OrderType.Attack) {
                RemoveDefenceOrder();
                squadController.defence = false;
                AddAttackOrder();
                amountOfOrders = 1;
                ChangeText();
                orderImage.sprite = orderSprite;
            }
            if (currentOrder == OrderType.Defence && amountOfOrders < ordersTime.Length) {
                AddDefenceOrder();
                amountOfOrders++;
                ChangeText();
            }
        }
        prevOrder = currentOrder;
        orderTime = ordersTime[amountOfOrders - 1].timeOfExpire;
    }

    private void OrderTimer() {
        if (orderTime > 0) {
            orderTime -= Time.deltaTime;
            bar.fillAmount = orderTime / ordersTime[amountOfOrders - 1].timeOfExpire;
        }
        else {
            if (prevOrder == OrderType.Attack) { RemoveAttackOrder(); }
            if (prevOrder == OrderType.Defence) { RemoveDefenceOrder(); }

            amountOfOrders--;
            ChangeText();
            if (amountOfOrders == 0) {
                prevOrder = OrderType.None;
                orderIndicator.SetActive(false);
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

    private void ChangeText() {
        barText.text = amountOfOrders.ToString();
    }
}
