using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OfficerController : MonoBehaviour
{
    [CreateAssetMenu(fileName = "NewOfficer", menuName = "Officer/Officer")]
    public class OfficerSettings : ScriptableObject
    {
        [SerializeField] public string officerName;
        [SerializeField] public Sprite officerIcon;

        [SerializeField] public float powerSquad;
        [SerializeField] public float defenceSquad;
        [SerializeField] public float pushSquad;
        [SerializeField] public float formationSquad;
        [SerializeField] public float moraleSquad;


      
        [SerializeField] public float skillTime;
       

        [SerializeField] public string skillName;
        [SerializeField] public Sprite skillIcon;
        [SerializeField] public string skillDescription;

        [SerializeField] public string skillLogic;
        [SerializeField] public string skillSpecs;

        [SerializeField] public GameObject skillFx;

    }
    [SerializeField] public int officerIndex;
    public OfficerSettings[] officers;

    [SerializeField] public bool skillRedy;
    [SerializeField] public float currentSkillTime;

    public OfficerSettings officerSpecs;
    OfficerSystem officerSystemManager;


    [SerializeField] public SquadController squadController;
    [SerializeField] public OfficerSkill skillLogic;

    [Space(10)]
    [Header("UI")]
    [SerializeField] public OfficerUIPanel officerPanel;
    [SerializeField] public bool isSelected;

    private CameraMovement cameraMovement;

    void Start()
    {
        squadController = GetComponent<SquadController>();
        officerSpecs = officers[officerIndex];

        //skillLogic = officerSpecs.skillLogic;

        System.Type type = System.Type.GetType(officerSpecs.skillLogic);

        OfficerSkill skill = (OfficerSkill)gameObject.AddComponent(type);

        skillLogic = skill;

        skill.skillSpecs = officerSpecs.skillSpecs;

        skillLogic.SkillInit();

        officerSystemManager = GameObject.Find("OfficerSystemManager").GetComponent<OfficerSystem>();

        officerSystemManager.OfficerInit(this);

        cameraMovement = GameObject.Find("CameraControlManager").GetComponent<CameraMovement>();

        

    }

    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update()
    {
        if (officerSpecs != null)
        {
            if (!skillRedy)
            {
                currentSkillTime += Time.deltaTime;

                if (officerPanel != null)
                {
                    officerPanel.UpdateCDImage(1f - (currentSkillTime / officerSpecs.skillTime));
                }

                if (currentSkillTime > officerSpecs.skillTime)
                {
                    currentSkillTime = 0;
                    skillRedy = true;

                    officerPanel.SkillRedy(true);
                }
            }
        }
    }

    public void SkillActivation()
    {
        if (skillRedy)
        {
            skillLogic.ActivateSkill();
            skillRedy = false;

            officerPanel.SkillRedy(false);
            officerPanel.OfficerSelect(false);
            isSelected = false;

            Instantiate(officerSpecs.skillFx, transform);
        }
    }

    public void OfficerPanelSelect()
    {
        if (isSelected)
        {
            officerPanel.OfficerSelect(false);
            isSelected = false;
        }
        else
        {
            officerPanel.OfficerSelect(true);
            isSelected = true;

            cameraMovement.CamToPoint(transform.position);
        }
    }
}
