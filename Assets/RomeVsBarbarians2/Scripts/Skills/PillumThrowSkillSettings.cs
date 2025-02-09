using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "Skills/Skill")]
public class PillumThrowSkillSettings : ScriptableObject
{
    [SerializeField] public float shotRange;
    [SerializeField] public int shotAmount;
    [SerializeField] public float shotSpeed;
    [SerializeField] public float shotDamage;
    [SerializeField] public float shotAccuracy;
    [SerializeField] public float shotRapidity;
    [SerializeField] public int projectilesPerShotCount;

    [Space(10)]
    [Header("Shooting Setup")]
    [SerializeField] public float shotStartOffset;
    [SerializeField] public float shotSpawnDelay;
    [SerializeField] public float shotHalfDelay;
    [SerializeField] public GameObject pfArrow;


}
