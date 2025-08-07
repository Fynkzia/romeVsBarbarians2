using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GrassPainter : EditorWindow
{
    public LayerMask terrainMask;
    public GameObject parentObject; // Родительский объект для травы
    public List<GameObject> grassPrefabs = new List<GameObject>(); // Список префабов травы
    public bool useSinglePrefab = false; // Использовать только первый префаб?
    private GameObject selectedPrefab; // Выбранный префаб

    private bool isPainting = false; // Флаг рисования

    [MenuItem("Tools/Grass Painter")]
    public static void ShowWindow()
    {
        GetWindow<GrassPainter>("Grass Painter");
    }

    private void OnGUI()
    {


        

        GUILayout.Label("Grass Painter Tool", EditorStyles.boldLabel);

        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        EditorGUILayout.LabelField("Grass Prefabs:");
        for (int i = 0; i < grassPrefabs.Count; i++)
        {
            grassPrefabs[i] = (GameObject)EditorGUILayout.ObjectField($"Prefab {i + 1}", grassPrefabs[i], typeof(GameObject), false);
        }

        if (GUILayout.Button("Add Prefab Slot"))
        {
            grassPrefabs.Add(null);
        }

        if (GUILayout.Button("Remove Last Prefab Slot") && grassPrefabs.Count > 0)
        {
            grassPrefabs.RemoveAt(grassPrefabs.Count - 1);
        }

        useSinglePrefab = EditorGUILayout.Toggle("Use Only First Prefab", useSinglePrefab);

        if (GUILayout.Button(isPainting ? "Stop Painting" : "Start Painting"))
        {
            isPainting = !isPainting;
            SceneView.duringSceneGui -= OnSceneGUI;
            if (isPainting) SceneView.duringSceneGui += OnSceneGUI;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt)
        {
            HandleMouseClick(e);
        }
    }

    private void HandleMouseClick(Event e)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (grassPrefabs.Count == 0 || (useSinglePrefab && grassPrefabs[0] == null))
            {
                Debug.LogWarning("No grass prefabs assigned!");
                return;
            }

            selectedPrefab = useSinglePrefab ? grassPrefabs[0] : grassPrefabs[Random.Range(0, grassPrefabs.Count)];

            if (selectedPrefab == null)
            {
                Debug.LogWarning("Selected prefab is null!");
                return;
            }

            GameObject grassInstance = (GameObject)PrefabUtility.InstantiatePrefab(selectedPrefab);
            Undo.RegisterCreatedObjectUndo(grassInstance, "Paint Grass");

            grassInstance.transform.position = hit.point;
            grassInstance.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            if (parentObject) grassInstance.transform.SetParent(parentObject.transform);

            e.Use();
        }
    }

    private void OnDestroy()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }
}
