using UnityEngine;

/// <summary>
/// Clase base para cualquier objeto con el que Carmen o Carlos
/// puedan interactuar.
/// </summary>
public abstract class RatInteractable : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField]
    private string interactionPrompt = "Interactuar";

    public string InteractionPrompt => interactionPrompt;

    /// <summary>
    /// Permite que cada objeto decida si puede usarse en este momento.
    /// </summary>
    public virtual bool CanInteract(GameObject interactor)
    {
        return interactor != null;
    }

    /// <summary>
    /// Ejecuta la interacción concreta del objeto.
    /// </summary>
    public abstract void Interact(GameObject interactor);
}