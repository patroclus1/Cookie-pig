using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    private Camera _camera;
    private Transform _transform;
    private Transform _cameraTransform;
    public bool IsDisplayed = false;
    [SerializeField] private TextMeshProUGUI _promptText;
    [SerializeField] private GameObject _uiPanel;

    private void Awake()
    {
        _camera = Camera.main;
        _cameraTransform = _camera.transform;
        _transform = transform;
        _uiPanel.SetActive(false);
    }

    private void LateUpdate()
    {
        var rotation = _cameraTransform.rotation;
        _transform.LookAt(_transform.position + rotation * Vector3.forward, rotation * Vector3.up);
    }

    public void ShowPrompt(string promptText, Transform promptTransform)
    {
        _promptText.text = promptText;
        _transform.position = promptTransform.position + promptTransform.forward/2;
        _uiPanel.SetActive(true);
        IsDisplayed = true;
    }

    public void HidePrompt()
    {
        _uiPanel.SetActive(false);
        IsDisplayed = false;
    }
}
