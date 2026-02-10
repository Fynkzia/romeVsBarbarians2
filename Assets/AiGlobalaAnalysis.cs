using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiGlobalaAnalysis : MonoBehaviour
{
    public AIGlobalController aiController;
    [Space(10)]


    [SerializeField] public float allEnemyPower;
    [SerializeField] public float avgEnemyGropPower;
    [Space(10)]

    [SerializeField] public float allPlayerPower;
    [SerializeField] public float avgPlayerGropPower;

    

    // Start is called before the first frame update
    public void UpdateAnalysis()
    {
        AllPowerCalculate();
    }

    // Update is called once per frame
    public void AllPowerCalculate()
    {
        allEnemyPower = 0;
        allPlayerPower = 0;

        for (int i = 0; i < aiController.allEnemiesList.Count; i++)
        {
            allEnemyPower += aiController.allEnemiesList[i].AiPowerCalculate();



        }

        for (int i = 0; i < aiController.allPlayerList.Count; i++)
        {
            allPlayerPower += aiController.allPlayerList[i].AiPowerCalculate();



        }

        avgEnemyGropPower = allEnemyPower / aiController.enemyGrops.Count;
        avgPlayerGropPower = allPlayerPower / aiController.playerGroups.Count;

    }
}
