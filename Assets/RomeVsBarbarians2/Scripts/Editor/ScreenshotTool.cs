using System;
using System.IO;
using UnityEngine;
using UnityEditor;

public class ScreenshotTool : EditorWindow
{
    private Camera targetCamera;
    private GameObject parentObject;
    private string screenshotsFolder;

    [MenuItem("Tools/Screenshot Tool")]
    public static void ShowWindow()
    {
        GetWindow<ScreenshotTool>("Screenshot Tool");
    }

    private void OnGUI()
    {
        GUILayout.Label("Screenshot Tool", EditorStyles.boldLabel);

        targetCamera = (Camera)EditorGUILayout.ObjectField("Target Camera", targetCamera, typeof(Camera), true);
        parentObject = (GameObject)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(GameObject), true);

        if (GUILayout.Button("Capture Screenshots"))
        {
            if (targetCamera == null || parentObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Please assign both Target Camera and Parent Object.", "OK");
                return;
            }

            if (targetCamera.targetTexture == null)
            {
                EditorUtility.DisplayDialog("Error", "Target Camera must have a RenderTexture assigned.", "OK");
                return;
            }

            CaptureScreenshots();
        }
    }

    private void CaptureScreenshots()
    {
        // Создаем папку для сохранения скриншотов
        string dateFolderName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string screenshotsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Screenshots", dateFolderName);

        Directory.CreateDirectory(screenshotsFolder);

        Debug.Log($"Скриншоты будут сохранены в: {screenshotsFolder}");

        // Скрываем все дочерние объекты перед началом
        foreach (Transform child in parentObject.transform)
        {
            child.gameObject.SetActive(false);
        }

        // Проходим по всем дочерним объектам
        foreach (Transform child in parentObject.transform)
        {
            // Активируем текущий объект
            child.gameObject.SetActive(true);

            // Принудительно обновляем рендеринг камеры
            targetCamera.Render();

            Debug.Log($"Saving screenshot for {child.name}");

            // Делаем скриншот
            string screenshotName = $"{child.name}.png";
            string screenshotPath = Path.Combine(screenshotsFolder, screenshotName);
            SaveRenderTextureToFile(targetCamera.targetTexture, screenshotPath);

            Debug.Log($"Скриншот объекта {child.name} сохранен в {screenshotPath}");

            // Деактивируем текущий объект
            child.gameObject.SetActive(false);
        }

        
       

        EditorUtility.DisplayDialog("Done", "All screenshots captured successfully!", "OK");
    }

    private void SaveRenderTextureToFile(RenderTexture renderTexture, string filePath)
    {
        // Сохраняем содержимое RenderTexture в файл с прозрачным фоном
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = renderTexture;

        Texture2D image = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
        image.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        image.Apply();

        byte[] bytes = image.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);

        RenderTexture.active = currentRT;
        DestroyImmediate(image);
    }
}
