using System;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform _interactionPoint;
    [SerializeField] private float _interactionRadius;
    [SerializeField] private LayerMask _interactableLayer;
    [SerializeField] private InteractionPromptUI _interactionPromptUI;
    [SerializeField] private int _numOfInteractables;

    private IInteractable _interactable;
    private readonly Collider[] _colliders = new Collider[3];

    public static event Action OnOutOfInteraction;

    private void Update()
    {
        _numOfInteractables = Physics.OverlapSphereNonAlloc(_interactionPoint.position, _interactionRadius, _colliders, _interactableLayer);

        if (_numOfInteractables > 0)
        {
            _interactable = _colliders[0].GetComponent<IInteractable>();

            if (_interactable != null)
            {
                if (!_interactionPromptUI.IsDisplayed)
                {
                    _interactionPromptUI.ShowPrompt(_interactable.InteractionPrompt, _interactable.InteractableObject.transform);
                }

                if (Input.GetKeyDown(KeyCode.E)) _interactable.OnInteract(this);
            }
        }
        else
        {
            OnOutOfInteraction?.Invoke();
            if (_interactable != null) _interactable = null;
            if (_interactionPromptUI.IsDisplayed) _interactionPromptUI.HidePrompt();
        }
    }
}
