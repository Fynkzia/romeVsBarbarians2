using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SquadInfo : MonoBehaviour
{
    [SerializeField] public Animator squadInfoAnimator;

    [SerializeField] public GameObject battleIndicator;
    [SerializeField] public GameObject shootingIndicator;
    [SerializeField] public GameObject retreatIndicator;
    [SerializeField] public GameObject defenceMaxIndcator;
    [SerializeField] public GameObject defenceFilledIndcator;
    [SerializeField] public Image defenceFillImage;

    [SerializeField] public BarUI moraleBar;
    [SerializeField] public Image xpBar;

    [SerializeField] public TextMeshProUGUI levelText;

    [SerializeField] public TextMeshProUGUI countText;

    [SerializeField] public LookAtCamera lookAtCamera;

    // Start is called before the first frame update
    public void MoraleUpdate(float min, float max )
    {
        moraleBar.ChangeProgress(min,max) ;
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

        xpBar.fillAmount = xp/ xNneed;
        xpBar.transform.localScale = new Vector3( 0.7f + (0.05f*level), 0.7f + (0.05f * level), 0.7f + (0.05f * level)) ;

        levelText.transform.localPosition = new Vector3(xpBar.transform.localPosition.x, 0.88f + (0.1f * level), xpBar.transform.localPosition.z);
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
