using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainTriggerController : MonoBehaviour
{
    private SquadController squadController;
    private void Awake() {
        squadController = gameObject.GetComponentInParent<SquadController>();
    }

    private void OnTriggerEnter(Collider other) {
        squadController.OnMainTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other) {
       squadController.OnMainTriggerExit(other);
    }
}
