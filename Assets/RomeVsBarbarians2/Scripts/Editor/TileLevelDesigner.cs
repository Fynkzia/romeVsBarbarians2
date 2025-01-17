using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileLevelDesigner : EditorWindow
{

    private bool edit;
    private bool justMoving;

    private GameObject tilePrefab;
    private GameObject rampPrefab;

    private LayerMask tileLayer;
    private float tileHeight = 1f;
    private float rampOffset = 0.5f;

    private float rampDownOffset = 0.5f;

    private float cubeCheckOffset = 0.5f;
    private Vector3 cubeCheckScale;

    private Stack<System.Action> undoStack = new Stack<System.Action>();

    [MenuItem("Tools/Tile Level Designer")]
    public static void ShowWindow()
    {
        GetWindow<TileLevelDesigner>("Tile Level Designer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Tile Level Designer", EditorStyles.boldLabel);

        edit = EditorGUILayout.Toggle("Edit", edit);
        justMoving = EditorGUILayout.Toggle("Just Moving", justMoving);

        tilePrefab = (GameObject)EditorGUILayout.ObjectField("Tile Prefab", tilePrefab, typeof(GameObject), false);
        rampPrefab = (GameObject)EditorGUILayout.ObjectField("Ramp Prefab", rampPrefab, typeof(GameObject), false);

        tileLayer = EditorGUILayout.LayerField("Tile Layer", tileLayer.value);
        tileHeight = EditorGUILayout.FloatField("Tile Height", tileHeight);
        rampOffset = EditorGUILayout.FloatField("Ramp Offset", rampOffset);

        rampDownOffset = EditorGUILayout.FloatField("Ramp Down Offset", rampDownOffset);

        cubeCheckOffset = EditorGUILayout.FloatField("Cube Check Offset", cubeCheckOffset);
        cubeCheckScale = EditorGUILayout.Vector3Field("cubeCheckScale", cubeCheckScale);

        if (GUILayout.Button("Undo Last Action"))
        {
            UndoLastAction();
        }

        GUILayout.Space(10);
        GUILayout.Label("Keyboard Controls:");
        GUILayout.Label(" - W: Raise tile under cursor.");
        GUILayout.Label(" - S: Lower tile under cursor.");
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (edit)
        {
            Event e = Event.current;

            if (e.type == EventType.KeyDown)
            {
                switch (e.keyCode)
                {
                    case KeyCode.W: // Поднять тайл
                        HandleTileModification(tileHeight);
                        e.Use();
                        break;
                    case KeyCode.S: // Опустить тайл
                        HandleTileModification(-tileHeight);
                        e.Use();
                        break;
                    case KeyCode.X: // Опустить тайл
                        DeleteTile();
                        e.Use();
                        break;
                }
            }
        }
    }

    private void DeleteTile()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f, false);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if(hit.collider.name == "Collider")
            {
                DestroyImmediate(hit.collider.transform.parent.gameObject);
            }
            else
            {
                DestroyImmediate(hit.collider.gameObject);
            }
        }
    }

        private void HandleTileModification(float heightChange)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f, false);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name}", hit.collider.gameObject);

            GameObject hitTile = hit.collider.gameObject;

            Vector3 newPosition = hitTile.transform.position + Vector3.up * heightChange;

            // Сохраняем текущее состояние для отмены
            Vector3 oldPosition = hitTile.transform.position;
            undoStack.Push(() =>
            {
                hitTile.transform.position = oldPosition;
                RemoveRampsAroundTile(hitTile.transform.position);
            });

            hitTile.transform.position = newPosition;

            if (!justMoving)
            {
                // Добавляем подъёмные или спускные тайлы в зависимости от того, подняли ли мы тайл или опустили
                if (heightChange > 0)
                {
                    // Поднимали тайл — создаем подъемы, если по сторонам нет других тайлов
                    AddRampsAroundTile(hitTile.transform.position, true);
                }
                else
                {
                    // Опустили тайл — создаем спуски, если по сторонам есть другие тайлы
                    AddRampsAroundTile(hitTile.transform.position, false);
                }
            }
        }
        else
        {
            Debug.Log("No object hit.");
        }
    }

    private void AddRampsAroundTile(Vector3 tilePosition, bool isRaising)
    {
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        List<GameObject> createdRamps = new List<GameObject>();

        foreach (Vector3 direction in directions)
        {
            Vector3 neighborPosition = tilePosition + direction * cubeCheckOffset;
            neighborPosition.y = tilePosition.y;

            // Проверяем, есть ли соседний тайл
            bool hasNeighbor = Physics.CheckBox(neighborPosition, cubeCheckScale, Quaternion.identity);

            if (isRaising)
            {
                // Подъем: создаем подъем, только если нет соседнего тайла
                if (!hasNeighbor)
                {
                    Vector3 rampPosition = tilePosition + direction * rampOffset;
                    //rampPosition.y += tileHeight;

                    if (rampPrefab != null)
                    {
                        GameObject newRamp = Instantiate(rampPrefab, rampPosition, Quaternion.identity);
                        Vector3 rampRotation = rampPrefab.transform.rotation.eulerAngles;
                        rampRotation.y = GetRampRotation(direction);
                        newRamp.transform.rotation = Quaternion.Euler(rampRotation);

                        Undo.RegisterCreatedObjectUndo(newRamp, "Add Ramp Tile");
                        createdRamps.Add(newRamp);
                    }
                }
            }
            else
            {
                // Спуск: создаем спуск, только если есть соседний тайл
                if (hasNeighbor)
                {
                    Vector3 rampPosition = tilePosition + direction * rampDownOffset;
                    rampPosition.y += tileHeight;

                    if (rampPrefab != null)
                    {
                        GameObject newRamp = Instantiate(rampPrefab, rampPosition, Quaternion.identity);
                        Vector3 rampRotation = rampPrefab.transform.rotation.eulerAngles;
                        rampRotation.y = GetRampRotation(direction);
                        newRamp.transform.rotation = Quaternion.Euler(rampRotation);

                        newRamp.transform.localScale =  new Vector3(newRamp.transform.localScale.x, -newRamp.transform.localScale.y, newRamp.transform.localScale.z);

                        Undo.RegisterCreatedObjectUndo(newRamp, "Add Ramp Tile");
                        createdRamps.Add(newRamp);
                    }
                }
            }
        }

        // Добавляем рампы в стек для отмены
        if (createdRamps.Count > 0)
        {
            undoStack.Push(() =>
            {
                foreach (var ramp in createdRamps)
                {
                    if (ramp != null)
                    {
                        DestroyImmediate(ramp);
                    }
                }
            });
        }
    }



    private float GetRampRotation(Vector3 direction)
    {
        if (direction == Vector3.forward) return 0f;
        if (direction == Vector3.back) return 180f;
        if (direction == Vector3.left) return -90f;
        if (direction == Vector3.right) return 90f;
        return 0f;
    }

    private void RemoveRampsAroundTile(Vector3 tilePosition)
    {
        Collider[] colliders = Physics.OverlapBox(tilePosition, Vector3.one * rampOffset, Quaternion.identity, tileLayer);

        foreach (var collider in colliders)
        {
            if (collider != null && collider.gameObject != null && collider.gameObject != tilePrefab)
            {
                DestroyImmediate(collider.gameObject);
            }
        }
    }

    private void UndoLastAction()
    {
        if (undoStack.Count > 0)
        {
            System.Action undoAction = undoStack.Pop();
            undoAction?.Invoke();
        }
    }
}
