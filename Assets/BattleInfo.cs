using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleInfo : MonoBehaviour
{
    

    

   

    public GameObject toBattleButtonObject;
    public GameObject battleObject;
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

           autoBattleObject.SetActive(false);
            toBattleButtonObject.SetActive(false);
            battleObject.SetActive(false);


        }
        else
        {
            animator.SetBool("Select", false);

            if (mapBattleController.inAutoBattle)
            {
                autoBattleObject.SetActive(true);
            }
            else if(mapBattleController.inBattle)
            {
                battleObject.SetActive(true);
            }else
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
        toBattleButtonObject.SetActive(false);
       battleObject.SetActive(true);

    }

    public void StartBattleTimerUpdate(float fillAmount)
    {
        startbattleTimer.fillAmount = fillAmount;
        
    }

    

    

    

}
