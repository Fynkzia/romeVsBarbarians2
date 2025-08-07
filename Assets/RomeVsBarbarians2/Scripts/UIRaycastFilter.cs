using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public static class UIRaycastFilter
{
    public static bool IsPointerOverUI()
    {
        EventSystem eventSystem = EventSystem.current;
        if (eventSystem == null) return false;

        PointerEventData eventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        GraphicRaycaster raycaster = Object.FindObjectOfType<GraphicRaycaster>();
        if (raycaster == null) return false;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.layer == 5) 
                return true; 


        }

        return false;
    }
}
