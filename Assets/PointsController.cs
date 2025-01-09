using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsController : MonoBehaviour
{
    [Range(0f, 1f)]
    public float scatterAmount = 0f;
    public float radius = 1f;// Значение разброса от 0 до 1

    public float maxScatterDistance = 5f; // Максимальная дистанция разброса

    private Vector3[] originalPositions; // Исходные позиции точек
    public Transform[] points; // Список точек

    private void Awake()
    {
        // Сохраняем исходные позиции точек
        int childCount = transform.childCount;
        points = new Transform[childCount];
        originalPositions = new Vector3[childCount];

        for (int i = 0; i < childCount; i++)
        {
            points[i] = transform.GetChild(i);
            originalPositions[i] = points[i].localPosition;
        }
    }

    public void RadiusUpdate()
    {
        // Обновляем позиции точек в зависимости от scatterAmount
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i] == null) continue;

            // Исходная позиция
            Vector3 originalPosition = originalPositions[i];

            

            // Случайное смещение
            Vector3 randomOffset = Random.insideUnitSphere * maxScatterDistance * scatterAmount;

            Vector3 direction = (originalPosition-transform.localPosition).normalized  * radius * scatterAmount;

            Vector3 yVector = new Vector3(randomOffset.x, 0, randomOffset.z);

            Vector3 result = originalPosition + yVector + direction;

            result = new Vector3(result.x, 0f, result.z);
            // Новая позиция

            points[i].localPosition = result;
        }
    }
}
