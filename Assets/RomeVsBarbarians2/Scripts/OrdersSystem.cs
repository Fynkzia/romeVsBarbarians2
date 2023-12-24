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

    [SerializeField] private Animator attackButtonAnimator;
    [SerializeField] private Animator defenceButtonAnimator;
    
    [SerializeField] private int costOfOrder; //For basic implementation


    [SerializeField]private OrderType currentOrder;
    [SerializeField]private bool orderSelected = false;
    [SerializeField]private bool prevCoinsState;
    [SerializeField]private bool currentCoinsState;
    [Header("Order sprites")]
    [Space(10)]
    [SerializeField] private Sprite attackSprite;
    [SerializeField] private Sprite defenceSprite;

    private Sprite currentSprite;
    private void Start() {
        prevCoinsState = coinsController.AmountOfCoins() >= costOfOrder;
        attackButtonAnimator = attackButton.GetComponent<Animator>();
        defenceButtonAnimator = defenceButton.GetComponent<Animator>();
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
                        currentUnit.DoOrder(currentOrder, currentSprite);
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
            attackButtonAnimator.SetTrigger("Normal");
        } else { 
            currentOrder = OrderType.Attack;
            orderSelected = true;
            currentSprite = attackSprite;
            defenceButtonAnimator.SetTrigger("Normal");
            attackButtonAnimator.SetTrigger("Selected");
        }
    }
    public void DefenceButtonClick() {
        if (orderSelected && currentOrder == OrderType.Defence) {
            orderSelected = false;
            defenceButtonAnimator.SetTrigger("Normal");
        } else { 
            currentOrder = OrderType.Defence;
            orderSelected = true;
            currentSprite = defenceSprite;
            attackButtonAnimator.SetTrigger("Normal");
            defenceButtonAnimator.SetTrigger("Selected");
        }
    }

    private void UnSelectButtons() {
        orderSelected = false;
        attackButtonAnimator.SetTrigger("Normal");
        defenceButtonAnimator.SetTrigger("Normal");
    }
}
