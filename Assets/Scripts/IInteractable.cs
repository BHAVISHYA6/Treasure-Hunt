using UnityEngine;

public interface IInteractable
{
    string GetInteractionPrompt();
    void Interact(PlayerController player);
    bool CanInteract(PlayerController player);
}
