using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    private Transform _camTransform;
    private Transform _transform;
    private Vector3 _rotationOffset = new Vector3(0,-180,0);

    private void Awake()
    {
        _camTransform = Camera.main.transform;
        _transform = transform;
    }

    private void LateUpdate()
    {
        if (_camTransform != null)
        {
            _transform.LookAt(_transform.position + _camTransform.rotation * Vector3.forward, _camTransform.rotation * Vector3.up);
        }
    }
}
