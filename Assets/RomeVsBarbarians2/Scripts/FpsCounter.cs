using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FpsCounter : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI _fpsText;
    [SerializeField] private float _hudRefreshRate = 1f;

    private float _timer;

    private float _fps;

    private void Update()
    {
        float currentFps = 1f / Time.unscaledDeltaTime;
        _fps = Mathf.Lerp(_fps, currentFps, 0.1f); // сглаживание
        _fpsText.text = "FPS: " + Mathf.RoundToInt(_fps);
    }
}
