using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/FormationSkill")]
public class TestudoSkillSettings : ScriptableObject
{
    [SerializeField] public float speed;
    [SerializeField] public float defence;

    [SerializeField] public PointsController formation;
    [SerializeField] public GameObject shieldObject;



}
