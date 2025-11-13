using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanel : MonoBehaviour
{
   public SceneLoader sceneLoader;
    public ResourceManager resourceManager;
    public MapSceneManager mapSceneManager;

    public GameObject cheatPanel;

    public int tapCounter;
   

    // Start is called before the first frame update
    public void CheatPanelActivation()
    {
        if (cheatPanel.activeSelf == true)
        {
            cheatPanel.SetActive(false);
        }
        else
        {
            tapCounter++;

            if (tapCounter > 3)
            {
                cheatPanel.SetActive(true);
                tapCounter = 0;
            }

        }
    }

    // Update is called once per frame
    public void AddCoins(int count)
    {
        resourceManager.ChangeAmountOfCoins(count);
    }

    public void TimeChannge(int count)
    {
        Time.timeScale = count;
    }

    public void AddUnits(int count)
    {
        if(mapSceneManager.controlController.cityController != null)
        {
            mapSceneManager.controlController.cityController.cityUnits += count;
        }
    }

    public void WinBattle()
    {
        if (SceneLoader.Instance.activeScene >= 0)
        {
            SceneLoader.Instance.activeBattleScenes[SceneLoader.Instance.activeScene].Debug_BattleWin();




        }
    }
}
