using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadColliderController : MonoBehaviour
{
    private SquadController squadController;
    private void Awake()
    {
        squadController = gameObject.GetComponentInParent<SquadController>();
    }

     void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer != 15)// в конце проверка чтоб не срабатывало от шутинг триггера
            //squadController.OnMainCollisionEnter(other);

        Debug.Log("main collider");
    }

    //private void OnColliderExit(Collider other)
    //{
    //    if (other.gameObject.layer != 15)// в конце проверка чтоб не срабатывало от шутинг триггера
    //        squadController.OnMainTriggerExit(other);
    //}
}
