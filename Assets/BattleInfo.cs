using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfo : MonoBehaviour
{
    

    

   

    public GameObject toBattleButtonObject;
    public GameObject autoBattleObject;


    public Image startbattleTimer;


    public Canvas canvas;

    public MapBattleController mapBattleController;

   

    

    [SerializeField] private Animator animator;

   

    private void Awake()
    {
        
        

        mapBattleController.BattleTimerUpdate += StartBattleTimerUpdate;

    }


    public void Select(bool select)
    {
       

        if (select)
        {
          
            animator.SetBool("Select",true);

            if (!mapBattleController.inAutoBattle)
            {
                toBattleButtonObject.SetActive(false);
            }
        }
        else
        {
            animator.SetBool("Select", false);

            if (!mapBattleController.inAutoBattle)
            {
                toBattleButtonObject.SetActive(true);
            }
        }

        
    }

    

    public void AutoBattleStart()
    {
        
       
            animator.SetTrigger("AutoBattle");

        toBattleButtonObject.SetActive(false);
        autoBattleObject.SetActive(true);

    }

    public void BattleStart()
    {


        animator.SetTrigger("ToBattle");


        startbattleTimer.gameObject.SetActive(false);

    }

    public void StartBattleTimerUpdate(float fillAmount)
    {
        startbattleTimer.fillAmount = fillAmount;
        
    }

    

    

    

}
