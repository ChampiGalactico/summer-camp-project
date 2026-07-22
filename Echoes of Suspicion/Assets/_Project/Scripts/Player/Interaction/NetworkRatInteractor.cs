using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

[DisallowMultipleComponent]
public sealed class NetworkRatInteractor : NetworkBehaviour
{
    [Header("Raycast")]
    [SerializeField]
    private Transform interactionOrigin;

    [SerializeField, Min(0.1f)]
    private float interactionDistance = 2.2f;

    [SerializeField]
    private LayerMask interactionMask;

    [Header("Debug")]
    [SerializeField]
    private bool drawDebugRay = true;

    private RatInteractable currentTarget;

    private void Update()
    {
        if (!isLocalPlayer || interactionOrigin == null)
        {
            return;
        }

        UpdateCurrentTarget();
        HandleInteractionInput();
        DrawInteractionRay();
    }

    private void UpdateCurrentTarget()
    {
        RatInteractable detectedTarget = DetectTarget();

        if (detectedTarget == currentTarget)
        {
            return;
        }

        currentTarget = detectedTarget;

        if (currentTarget != null)
        {
            Debug.Log(
                $"[E] {currentTarget.InteractionPrompt}: " +
                currentTarget.name,
                currentTarget);
        }
    }

    private RatInteractable DetectTarget()
    {
        bool didHit = Physics.Raycast(
            interactionOrigin.position,
            interactionOrigin.forward,
            out RaycastHit hit,
            interactionDistance,
            interactionMask,
            QueryTriggerInteraction.Ignore);

        if (!didHit)
        {
            return null;
        }

        RatInteractable interactable =
            hit.collider.GetComponentInParent<RatInteractable>();

        if (interactable == null ||
            !interactable.CanInteract(gameObject))
        {
            return null;
        }

        return interactable;
    }

    private void HandleInteractionInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null ||
            !keyboard.eKey.wasPressedThisFrame ||
            currentTarget == null)
        {
            return;
        }

        currentTarget.Interact(gameObject);
    }

    private void DrawInteractionRay()
    {
        if (!drawDebugRay)
        {
            return;
        }

        Debug.DrawRay(
            interactionOrigin.position,
            interactionOrigin.forward * interactionDistance,
            currentTarget != null ? Color.green : Color.red);
    }

    private void OnDisable()
    {
        currentTarget = null;
    }
}