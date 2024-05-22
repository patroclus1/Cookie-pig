using UnityEngine;

public interface IInteractable
{
    string InteractionPrompt { get; }
    float HoldDuration { get; }
    bool HoldInteract { get; }
    bool MultipleUse { get; }
    bool IsInteractable { get; }
    Transform InteractableObject { get; }

    void OnInteract(Interactor interactor);
}
