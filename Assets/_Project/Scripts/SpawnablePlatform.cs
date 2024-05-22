using UnityEngine;

public class SpawnablePlatform : MonoBehaviour
{
    private Transform _transform;
    private Rigidbody _rb;
    [SerializeField] private Transform parentAfterDeployed;
    private Transform _initialParent;
    [SerializeField] private float _moveSpeed = 5f;
    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }


    private bool _shouldMove;

    private void Awake()
    {
        _transform = transform;
        _initialParent = transform.parent;
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!_shouldMove) return;
        _rb.AddForce(MoveSpeed * Time.fixedDeltaTime * _transform.forward, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision != null)
        {
            _rb.velocity = Vector3.zero;
            _rb.isKinematic = true;
        }
    }

    public void Launch()
    {
        _transform.parent = parentAfterDeployed;
        _rb.isKinematic = false;
        _shouldMove = true;
    }

    public void Return()
    {
        _rb.isKinematic = true;
        _shouldMove = false;
        _transform.parent = _initialParent;
        _transform.position = _initialParent.position;
        _transform.rotation = _initialParent.rotation;
    }
}
