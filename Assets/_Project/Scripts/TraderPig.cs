using System;
using UnityEngine;

public class TraderPig : MonoBehaviour, IInteractable
{
    [SerializeField] private string _interactionPrompt;
    [SerializeField] private bool _isInteractable;
    [SerializeField] private bool _isHoldToInteract;
    [SerializeField] private float _interactionTime;
    [SerializeField] private bool _isMultiUse;

    public static event Action OnShopOpen;

    public float HoldDuration
    {
        get 
        { 
            if (_isHoldToInteract) return _interactionTime; 
            else return 0;
        }
    }
    public bool HoldInteract => _isHoldToInteract;
    public bool MultipleUse => _isMultiUse;
    public bool IsInteractable => _isInteractable;
    public string InteractionPrompt => _interactionPrompt;
    public Transform InteractableObject => transform;

    public void OnInteract(Interactor interactor)
    {
        OnShopOpen?.Invoke();
    }
}
