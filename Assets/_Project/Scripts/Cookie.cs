using System;
using UnityEngine;

public class Cookie : MonoBehaviour
{
    public static Action<int> OnCookieCollect;
    public Action OnSpecialCookieCollect;
    private Transform _transform;

    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float rotationSpeed = 50f;
    [SerializeField] private int cookieAmount = 1;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private bool isSpecialCookie = false;

    private void Awake()
    {
        _transform = transform;

        if (particles != null)
        {
            var p = Instantiate(particles);
            p.transform.position = _transform.position;
            p.transform.parent = _transform;
        }
    }

    private void Update()
    {
        _transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(playerTag))
        {
            _transform.gameObject.SetActive(false);
            OnCookieCollect?.Invoke(cookieAmount);
           if (isSpecialCookie) OnSpecialCookieCollect?.Invoke();
        }
    }
}
