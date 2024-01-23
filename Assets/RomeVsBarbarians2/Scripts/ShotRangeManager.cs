using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotRangeManager : MonoBehaviour
{
    public List<Collider> enemyColliders = new List<Collider>();
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "EnemyTrigger" && !enemyColliders.Contains(other)) {
            enemyColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (enemyColliders.Contains(other)) {
            enemyColliders.Remove(other);
        }
    }
}
