using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField] private float cameraSensitivity = 100f;
    private Transform _transform;

    private Camera _mainCamera;
    private Transform _mainCameraTransform;
    private float _xRotation = 0f;

    private void Awake()
    {
        _transform = transform;
        _mainCamera = Camera.main;
        _mainCameraTransform = _mainCamera.transform;
    }

    public void ReceiveLookInput(Vector2 mouseInput)
    {
        float mouseX = mouseInput.x * cameraSensitivity;
        float mouseY = mouseInput.y * cameraSensitivity;

        _transform.Rotate(Vector3.up, mouseX);

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -85f, 85f);
        _mainCameraTransform.localRotation = Quaternion.Euler(_xRotation,0,0);
    }
}
