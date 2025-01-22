using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class SaveAndApplyAnimationData : EditorWindow
{
    // Класс для хранения данных о трансформациях
    private class TransformData
    {
        public Vector3 localPosition;
        public Quaternion localRotation;
        public Vector3 localScale;
    }

    // Словарь для хранения данных: объект → данные трансформации
    private static Dictionary<Transform, TransformData> savedData = new Dictionary<Transform, TransformData>();

    [MenuItem("Tools/Save and Apply Animation Data")]
    public static void ShowWindow()
    {
        GetWindow<SaveAndApplyAnimationData>("Save and Apply Animation Data");
    }

    private void OnGUI()
    {
        GUILayout.Label("Save and Apply Animation Data", EditorStyles.boldLabel);

        if (GUILayout.Button("Save Animation Data from Selected Objects"))
        {
            SaveAnimationData();
        }

        if (GUILayout.Button("Apply Saved Data to Selected Objects"))
        {
            ApplySavedData();
        }

        
    }

    private static void SaveAnimationData()
    {
        savedData.Clear();

        if (Selection.gameObjects.Length == 0)
        {
            Debug.LogWarning("No objects selected. Please select objects to save animation data.");
            return;
        }

        foreach (GameObject obj in Selection.gameObjects)
        {
            SaveDataForObject(obj);
        }

        Debug.Log($"Saved animation data for {savedData.Count} objects.");
    }

    private static void SaveDataForObject(GameObject obj)
    {
        var transforms = obj.GetComponentsInChildren<Transform>();
        foreach (var trans in transforms)
        {
            // Сохраняем локальные данные
            savedData[trans] = new TransformData
            {
                localPosition = trans.localPosition,
                localRotation = trans.localRotation,
                localScale = trans.localScale
            };
        }
    }

    private static void ApplySavedData()
    {
        if (savedData.Count == 0)
        {
            Debug.LogWarning("No data saved. Please save animation data first.");
            return;
        }

        foreach (var entry in savedData)
        {
            if (entry.Key == null) continue; // Если объект был удален, пропускаем

            Undo.RecordObject(entry.Key, "Apply Saved Animation Data"); // Поддержка Undo

            // Применяем сохраненные данные
            entry.Key.localPosition = entry.Value.localPosition;
            entry.Key.localRotation = entry.Value.localRotation;
            entry.Key.localScale = entry.Value.localScale;
        }

        Debug.Log("Applied saved animation data to objects.");
    }

    
}
