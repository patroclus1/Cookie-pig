using UnityEngine;
using TMPro;

public class DebugSettings : MonoBehaviour
{
    private TMP_Text _uiText;

    private int _lastFrameIndex;
    private float[] _frameDeltaTimeArray;

    private void Awake()
    {
        _uiText = gameObject.GetComponentInChildren<TMP_Text>();
        _frameDeltaTimeArray = new float[50];

        Cursor.lockState = CursorLockMode.Locked;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
    }

    private void Update()
    {
        _frameDeltaTimeArray[_lastFrameIndex] = Time.deltaTime;
        _lastFrameIndex = (_lastFrameIndex + 1) % _frameDeltaTimeArray.Length;

        _uiText.text = Mathf.RoundToInt(CalculateFps()).ToString();
    }

    private float CalculateFps()
    {
        float total = 0f;
        foreach (float deltaTime in _frameDeltaTimeArray)
        {
            total += deltaTime;
        }
        return _frameDeltaTimeArray.Length / total;
    }
}
