using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadInfo : MonoBehaviour
{
    [SerializeField] public Animator squadInfoAnimator;
    [SerializeField] public Animator squadCountAnimator;

    [SerializeField] public GameObject battleIndicator;
    [SerializeField] public GameObject shootingIndicator;
    [SerializeField] public GameObject retreatIndicator;
    [SerializeField] public GameObject defenceMaxIndcator;
    [SerializeField] public GameObject defenceFilledIndcator;
    [SerializeField] public Image defenceFillImage;

    [SerializeField] public BarUI moraleBar;
    [SerializeField] public BarUI ammoBar;
    [SerializeField] public Transform xpBarObject;

    [SerializeField] public TextMeshPro levelText;

    [SerializeField] public TextMeshPro countText;

    [SerializeField] public LookAtCamera lookAtCamera;

    // Start is called before the first frame update
    public void MoraleUpdate(float min, float max )
    {
        moraleBar.ChangeProgress(min,max) ;
    }

    public void AmmoUpdate(float min, float max)
    {
        ammoBar.ChangeProgress(min, max);
    }

    public void Reset()
    {
        lookAtCamera.Reset();
    }

    // Update is called once per frame
    public void CountUpdate(int count)
    {
        countText.text = "" + count;
    }

    public void XpUpdate(float xp, float xNneed, int level)
    {
        levelText.text = "" + level;

        xpBarObject.localScale = new Vector3(xp / xNneed, 1, 1);
       // xpBarObject.transform.localScale = new Vector3( 0.7f + (0.05f*level), 0.7f + (0.05f * level), 0.7f + (0.05f * level)) ;

        //levelText.transform.localPosition = new Vector3(levelText.transform.localPosition.x, 12.51f + (0.1f * level), levelText.transform.localPosition.z);
    }

    public void DefenceFilleUpdate(float fill)
    {
        defenceFillImage.fillAmount = fill;
    }



    public void BattleIndicator(bool active)
    {
        battleIndicator.SetActive(active);
    }
    public void ShootingIndicator(bool active)
    {
        shootingIndicator.SetActive(active);
    }
    public void RetreatIndicator(bool active)
    {
        retreatIndicator.SetActive(active);
    }

    public void DefenceMaxIndcator(bool active)
    {
        defenceMaxIndcator.SetActive(active);
    }
    public void DefenceFilledIndcator(bool active)
    {
        defenceFilledIndcator.SetActive(active);
    }

}
