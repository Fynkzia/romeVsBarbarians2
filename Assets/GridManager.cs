using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{


    [SerializeField] public List<GridCell> cells = new List<GridCell>();

    void Start()
    {
       
    }

    

    // Находит клетку, в которой стоит точка
    public GridCell GetCellAt(Vector3 worldPos)
    {
        foreach (var c in cells)
        {
            if (c.Contains(worldPos))
                return c;
        }
        return null;
    }
}
