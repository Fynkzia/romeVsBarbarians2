using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SelectChapterUI : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public RectTransform[] items;

    public float snapSpeed = 10f;
    public float thresholdToStop = 0.05f;

    public int SelectedIndex { get; private set; }
    public Action<int> OnLevelSelected;

    public string[] chapters;

    private bool isDragging = false;

    void Start()
    {
        if (items == null || items.Length == 0)
        {
            // Automatically collect children
            int count = content.childCount;
            items = new RectTransform[count];
            for (int i = 0; i < count; i++)
                items[i] = content.GetChild(i).GetComponent<RectTransform>();
        }
    }

    void Update()
    {
        if (isDragging) return;

        // When not dragging — smooth snap
        float nearestPos = FindClosestItemNormalizedPosition();
        float current = scrollRect.horizontalNormalizedPosition;

        scrollRect.horizontalNormalizedPosition =
            Mathf.Lerp(current, nearestPos, Time.deltaTime * snapSpeed);

        if (Mathf.Abs(current - nearestPos) < thresholdToStop)
        {
            scrollRect.horizontalNormalizedPosition = nearestPos;
        }
    }

    float FindClosestItemNormalizedPosition()
    {
        float minDist = Mathf.Infinity;
        float targetPos = 0f;

        for (int i = 0; i < items.Length; i++)
        {
            float pos = CalculateItemPosition(i);
            float dist = Mathf.Abs(scrollRect.horizontalNormalizedPosition - pos);

            if (dist < minDist)
            {
                minDist = dist;
                targetPos = pos;
                SelectedIndex = i;
            }
        }

        return targetPos;
    }

    float CalculateItemPosition(int index)
    {
        if (items.Length <= 1) return 0;

        return (float)index / (items.Length - 1);
    }

    // UI event → ScrollRect Drag
    public void OnBeginDrag()
    {
        isDragging = true;
    }

    // UI event → ScrollRect End Drag
    public void OnEndDrag()
    {
        isDragging = false;

        OnLevelSelected?.Invoke(SelectedIndex);
    }

    public void OnPlayPressed()
    {
        int level = SelectedIndex;
        Debug.Log("Start level: " + level);

        // Загрузка сцены
        UnityEngine.SceneManagement.SceneManager.LoadScene(chapters[level]);
    }
}
