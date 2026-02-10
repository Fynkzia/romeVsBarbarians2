using System.Collections.Generic;
using UnityEngine;

public enum FormationType
{
    Line,
    LineWithFlanks,
    TwoLines,
    ThreeLines,
    DefensiveSemiCircle
}

public class FormationBuilder
{
    public float spacing = 3f;       // дистанция между отрядами
    public float lineDepth = 4f;     // дистанция между линиями

    public List<Vector3> BuildFormation(
        AIGlobalController.BattleGroup group,
        FormationType formation,
        Vector3 targetPosition)
    {
        Vector3 center = group.center;
        Vector3 forward = (targetPosition - center).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        int count = group.squadList.Count;

        switch (formation)
        {
            case FormationType.Line:
                return BuildLine(center, forward, right, count);

            case FormationType.LineWithFlanks:
                return BuildLineWithFlanks(center, forward, right, count);

            case FormationType.TwoLines:
                return BuildMultiLine(center, forward, right, count, 2);

            case FormationType.ThreeLines:
                return BuildMultiLine(center, forward, right, count, 3);

            case FormationType.DefensiveSemiCircle:
                return BuildSemiCircle(center, forward, count);
        }

        return new List<Vector3>();
    }


    private List<Vector3> BuildLine(
    Vector3 center,
    Vector3 forward,
    Vector3 right,
    int count)
    {
        List<Vector3> points = new List<Vector3>();

        float half = (count - 1) / 2f;

        for (int i = 0; i < count; i++)
        {
            float offset = (i - half) * spacing;
            points.Add(center + right * offset);
        }

        return points;
    }
    private List<Vector3> BuildLineWithFlanks(
        Vector3 center,
        Vector3 forward,
        Vector3 right,
        int count)
    {
        List<Vector3> points = BuildLine(center, forward, right, count);

        if (count < 4)
            return points;

        // Левый фланг
        points[0] -= forward * lineDepth;
        // Правый фланг
        points[count - 1] -= forward * lineDepth;

        return points;
    }
    private List<Vector3> BuildMultiLine(
    Vector3 center,
    Vector3 forward,
    Vector3 right,
    int count,
    int lines)
    {
        List<Vector3> points = new List<Vector3>();

        int perLine = Mathf.CeilToInt((float)count / lines);
        int index = 0;

        for (int l = 0; l < lines; l++)
        {
            float depthOffset = -l * lineDepth;
            float half = (perLine - 1) / 2f;

            for (int i = 0; i < perLine && index < count; i++)
            {
                float sideOffset = (i - half) * spacing;
                Vector3 pos =
                    center +
                    forward * depthOffset +
                    right * sideOffset;

                points.Add(pos);
                index++;
            }
        }

        return points;
    }
    private List<Vector3> BuildSemiCircle(
        Vector3 center,
        Vector3 forward,
        int count)
    {
        List<Vector3> points = new List<Vector3>();

        float radius = spacing * count / Mathf.PI;
        float angleStep = Mathf.PI / (count - 1);

        Vector3 right = Vector3.Cross(Vector3.up, forward);

        for (int i = 0; i < count; i++)
        {
            float angle = -Mathf.PI / 2f + angleStep * i;

            Vector3 dir =
                Mathf.Cos(angle) * forward +
                Mathf.Sin(angle) * right;

            points.Add(center + dir * radius);
        }

        return points;
    }
}

}
