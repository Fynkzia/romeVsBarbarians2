using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillumThrowSkill : OfficerSkill
{

    [Header("Shooting Spec")]
    [SerializeField] public LayerMask detectionMask = 1 << 12;
    [SerializeField] public PillumThrowSkillSettings specs;

  

    SquadController squadController;


    

    // Start is called before the first frame update
    public override void SkillInit()
    {
        squadController = GetComponent<SquadController>();

        if (skillSpecs != null)
        {
            specs = Resources.Load<PillumThrowSkillSettings>("Data/Skills/" + skillSpecs);

        }

    }

    public override void ActivateSkill()
    {
        if (specs != null)
        {
            Debug.Log("Боевой клич активирован! Отряд получает бонус к атаке!");

            Collider[] nearColliders = Physics.OverlapSphere(transform.position, specs.shotRange, detectionMask);


            for (int i = 0; i < nearColliders.Length; i++)
            {
                if (nearColliders[i].tag == "Enemy")
                {
                    squadController.predictEnemy = nearColliders[i];
                    StartCoroutine(AttackShootingSiqunce());
                    return;
                }
            }

        }
        else
        {
            
        } 
    }

    IEnumerator AttackShootingSiqunce()
    {

        squadController.CancelMovement();


        Vector3 enemyPosition = squadController.predictEnemy.gameObject.transform.position;

        yield return new WaitForSeconds(0.5f);



        int shootingUnitsCouns = specs.projectilesPerShotCount;


        for (int i = 0; i < shootingUnitsCouns; i++)
        {

            squadController.animatorControllers[i].SpriteAnimationChange(10);


        }

        yield return new WaitForSeconds(specs.shotSpawnDelay);
        float distance = Vector3.Distance(transform.position, enemyPosition);
        ShotMovement.Create(specs.pfArrow, transform.position + new Vector3(0, specs.shotStartOffset, 0), enemyPosition, specs.shotSpeed, shootingUnitsCouns, specs.shotDamage, (distance / specs.shotRange), gameObject.tag, squadController.TriggerObject.radius);

        yield return new WaitForSeconds(specs.shotHalfDelay);
        ShotMovement.Create(specs.pfArrow, transform.position + new Vector3(0, specs.shotStartOffset, 0), enemyPosition, specs.shotSpeed, shootingUnitsCouns, specs.shotDamage, (distance / specs.shotRange), gameObject.tag, squadController.TriggerObject.radius);

        for (int i = 0; i < shootingUnitsCouns; i++)
        {

            squadController.animatorControllers[i].SpriteAnimationChange(11);


        }

        yield return new WaitForSeconds(1f);

        //SquadControlManager controlController = GameObject.Find("SquadControlManager").GetComponent<SquadControlManager>();

        //controlController.SquadWayToPoint(squadController, enemyPosition); // идем в рукопашную после выстрела атакаки

        squadController.shootingIndicator.SetActive(false);
    }
}
