using System.Collections.Generic;
using UnityEngine;

public class FormationPointsProvider : MonoBehaviour
{
    [Header("Formation settings")]
    [SerializeField] private float spacing = 1.5f;   // расстояние между точками
    [SerializeField] private int rowLength = 5;       // сколько точек в ряду

    private readonly List<Transform> points = new();

    /// <summary>
    /// Гарантирует, что будет ровно count точек построения
    /// </summary>
    public List<Transform> RequestPoints(int count)
    {
        if (count < 0)
            count = 0;

        AdjustPointCount(count);
        UpdatePointPositions();

        return points;
    }

    /// <summary>
    /// Удалить все точки
    /// </summary>
    public void Clear()
    {
        for (int i = points.Count - 1; i >= 0; i--)
        {
            if (points[i] != null)
                Destroy(points[i].gameObject);
        }

        points.Clear();
    }

    // ===================== INTERNAL =====================

    private void AdjustPointCount(int targetCount)
    {
        // удалить лишние
        while (points.Count > targetCount)
        {
            var p = points[^1];
            points.RemoveAt(points.Count - 1);
            Destroy(p.gameObject);
        }

        // создать недостающие
        while (points.Count < targetCount)
        {
            GameObject point = new GameObject($"FormationPoint_{points.Count}");
            point.transform.SetParent(transform, false);
            points.Add(point.transform);
        }
    }

    private void UpdatePointPositions()
    {
        for (int i = 0; i < points.Count; i++)
        {
            int row = i / rowLength;
            int col = i % rowLength;

            Vector3 localPos = new Vector3(
                col * spacing,
                0f,
                -row * spacing
            );

            points[i].localPosition = localPos;
            points[i].localRotation = Quaternion.identity;
        }
    }
}
