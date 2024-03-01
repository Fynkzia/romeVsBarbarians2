using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotRangeManager : MonoBehaviour
{
    public List<Collider> enemyColliders = new List<Collider>();
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Enemy" && !enemyColliders.Contains(other) && other.isTrigger) {
            enemyColliders.Add(other);
            Debug.Log(other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (enemyColliders.Contains(other)) {
            enemyColliders.Remove(other);
        }
    }
}
