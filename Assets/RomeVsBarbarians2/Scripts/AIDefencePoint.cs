using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDefencePoint : MonoBehaviour
{
    [SerializeField] public bool needToDefence = true;
  
    [SerializeField] public float defencePriority;

    [SerializeField] public float defenceBasePriority;

    public BuildingManager[] buildings;

    SphereCollider defencePointCollider;

    public float radius;

    [SerializeField] public LayerMask detectionMask;
    // Start is called before the first frame update
    void Start()
    {
        defencePointCollider = GetComponent<SphereCollider>();
        radius = defencePointCollider.radius;
    }


    public void CheckDefencePoint()
    {

        Collider[] nearColliders = Physics.OverlapSphere(transform.position, radius, detectionMask);

        defencePriority = defenceBasePriority;

        int enemies = 0;
        int players = 0;
        int buildings = 0;

        for (int i = 0; i < nearColliders.Length; i++)
        {
            if (nearColliders[i].tag == "Enemy")
            {
                if(nearColliders[i].gameObject.layer == 16)
                {
                     // есть здания, есть что защищать
                    buildings++;
                }
                else
                {
                    //defencePriority -= 0.5f; // уже есть кто-то из наших на дефенс точке
                    enemies++;
                }
                

            }
            else if(nearColliders[i].tag == "Squad")
            {
               // defencePriority -= 1; // уже есть кто-то из наших на дефенс точке
                players++;
            }


            
        }
        if(players == 0 && enemies == 0)
        {
            defencePriority += buildings ;
        }else if (enemies > players)
        {
            defencePriority -= enemies * 0.5f;
            defencePriority += players * 0.5f;
            defencePriority += buildings;
        }
        else if (enemies <= players)
        {
            
            defencePriority += (players - enemies) * 2f;
            defencePriority += buildings;
        }


        if (enemies == 0 && players > 1)
        {
            needToDefence = false;
            defencePriority = 0;
        }

    }

}
