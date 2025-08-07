using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampainManager : MonoBehaviour
{
    public GameObject targetPointPrefab;

    public List<CityController> enemyCities;
    public List<ArmyController> enemyArmies;

    public List<CityController> defenceCities;

    public bool defenceTarget;
    public bool destroyTarget;

    public UIManager uIManager;
    public CameraMapMovement cameraMapMovement;

   // Start is called before the first frame update
   void Start()
    {
        CampainStart();

        
    }

    // Update is called once per frame
    void CityCptured(CityController city)
    {
        if (city.isCampainTarget)
        {
            if (!city.isPlayer)
            {
                enemyCities.Remove(city);
            }
            else
            {
                defenceCities.Remove(city);
            }

        }

                CheckCompainStatus();
    }

    void EnemyArmyDestroy(ArmyController army)
    {
        if (army.isCampainTarget)
        {
            if (!army.isPlayer)
            {
                enemyArmies.Remove(army);

                CheckCompainStatus();
            }
            else
            {
                CheckCompainStatus();
            }
        }

    }

    public void CheckCompainStatus()
    {
        if (defenceCities.Count == 0)
        {
            CampainLose();
        }
        else
        {

            if (enemyCities.Count == 0 && enemyArmies.Count == 0)
            {
                CampainCompited();
            }
        }
    }


    public void CampainCompited()
    {
        uIManager.CampainWinLoose(true);
    }

    public void CampainLose()
    {
        uIManager.CampainWinLoose(false);
    }

    public void CampainStart()
    {
        for (int i = 0; i < enemyCities.Count; i++)
        {
            Instantiate(targetPointPrefab, enemyCities[i].transform.position, targetPointPrefab.transform.rotation, enemyCities[i].transform);
            enemyCities[i].isCampainTarget = true;

            enemyCities[i].OnCityCaptured += CityCptured;
        }

        for (int i = 0; i < enemyArmies.Count; i++)
        {
            Instantiate(targetPointPrefab, enemyArmies[i].transform.position, targetPointPrefab.transform.rotation, enemyArmies[i].transform);
            enemyArmies[i].isCampainTarget = true;

            enemyArmies[i].OnArmyDestroyed += EnemyArmyDestroy;
        }

    }

    public void CampainButton()
    {
        uIManager.UIReset();

        if (enemyArmies.Count > 0)
        {
            cameraMapMovement.CamToPoint(enemyArmies[0].transform.position, null);
        }else if (enemyCities.Count > 0)
        {
            cameraMapMovement.CamToPoint(enemyCities[0].transform.position, null);
        }
    }
}
