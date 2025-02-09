using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestudoSkill : OfficerSkill
{

    [Header("Skill Settings")]

    [SerializeField] public TestudoSkillSettings specs;



    SquadController squadController;

    float oldSpeed;
    float oldDefence;
    PointsController oldFormation;

    GameObject[] sheilds;

    bool skillActive;




    // Start is called before the first frame update
    public override void SkillInit()
    {
        squadController = GetComponent<SquadController>();

        if (skillSpecs != null)
        {
            specs = Resources.Load<TestudoSkillSettings>("Data/Skills/" + skillSpecs);

        }

        oldSpeed = squadController.movementSpeed;
        oldDefence = squadController.defenceSquad;

        oldFormation = squadController.unitPositions;

        squadController.OnInBattleTrue += DeactiveteSkillWhenInBattle;
    }

    public override void ActivateSkill()
    {
        if (!skillActive)
        {
            if (specs != null)
            {
                Debug.Log("TestudoSkillSettings");

                PointsController newFormation = Instantiate(specs.formation.gameObject, transform).GetComponent<PointsController>();
                oldFormation.gameObject.SetActive(false);

                squadController.unitPositions = newFormation;
                newFormation.transform.rotation = oldFormation.transform.rotation;

                squadController.UnitsTransformInFormation();

                sheilds = new GameObject[squadController.unitArray.Count];

                for (int i = 0; i < sheilds.Length; i++)
                {
                    GameObject newShield = Instantiate(specs.shieldObject, squadController.unitArray[i].transform.GetChild(0));
                    sheilds[i] = newShield;

                }

                squadController.movementSpeed += specs.speed;
                squadController.defenceSquad += specs.defence;

                skillActive = true;
            }
            else
            {

            }
        }
    }


    public void DeactiveteSkillWhenInBattle()
    {
        if (skillActive)
        {
            oldFormation.transform.rotation = squadController.unitPositions.transform.rotation;
            squadController.unitPositions = oldFormation;

            squadController.UnitsTransformInFormation();



            for (int i = 0; i < sheilds.Length; i++)
            {
                if (sheilds[i] != null)
                {
                    Destroy(sheilds[i]);
                }

            }

            squadController.movementSpeed = oldSpeed;
            squadController.defenceSquad = oldDefence;

            skillActive = false;

        }
    }
}
