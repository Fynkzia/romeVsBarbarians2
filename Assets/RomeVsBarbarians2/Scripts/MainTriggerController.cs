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
        if(other.gameObject.layer != 15)// в конце проверка чтоб не срабатывало от шутинг триггера
            squadController.OnMainTriggerEnter(other);
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer != 15)// в конце проверка чтоб не срабатывало от шутинг триггера
            squadController.OnMainTriggerExit(other);
    }
}
