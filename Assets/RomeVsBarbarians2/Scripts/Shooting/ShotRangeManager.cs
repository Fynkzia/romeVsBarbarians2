using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotRangeManager : MonoBehaviour
{
    [SerializeField] private ShootingController Controller;

    [SerializeField] private bool isBuilding;

    [SerializeField] private BuildingManager BuildController;


    public List<Collider> enemyColliders = new List<Collider>();


    private void OnTriggerEnter(Collider other) {
        if (((gameObject.tag == "Squad" && other.gameObject.tag == "Enemy") || (tag == "Enemy" && other.gameObject.tag == "Squad"))
            && !enemyColliders.Contains(other) && other.isTrigger)
        {
            enemyColliders.Add(other);

            if (isBuilding)
            {
                BuildController.GetNewTargets();
            }
            else
            {
                Controller.GetNewTargets();
            }
            

            Debug.Log("Shooting trigger find : " + other.gameObject.name);
        }
    }

    

    private void OnTriggerExit(Collider other) {
        if (enemyColliders.Contains(other)) {
            enemyColliders.Remove(other);

            if (isBuilding)
            {
                BuildController.GetNewTargets();
            }
            else
            {
                Controller.GetNewTargets();
            }
        }
    }
}
