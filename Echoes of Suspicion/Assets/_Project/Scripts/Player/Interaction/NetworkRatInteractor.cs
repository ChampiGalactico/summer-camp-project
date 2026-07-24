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

    [Header("Server Validation")]
    [SerializeField, Min(0.1f)]
    private float maximumServerDistance = 2.75f;

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
            !interactable.CanPreviewInteraction(gameObject))
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

        NetworkIdentity targetIdentity =
            currentTarget.netIdentity;

        if (targetIdentity == null ||
            targetIdentity.netId == 0)
        {
            Debug.LogWarning(
                $"{currentTarget.name} no está registrado " +
                "como objeto de red.",
                currentTarget);

            return;
        }

        CmdTryInteract(targetIdentity);
    }

    [Command]
    private void CmdTryInteract(
        NetworkIdentity targetIdentity)
    {
        if (targetIdentity == null)
        {
            return;
        }

        RatInteractable target =
            targetIdentity.GetComponent<RatInteractable>();

        if (target == null)
        {
            return;
        }

        if (!IsWithinServerRange(target))
        {
            Debug.LogWarning(
                $"{name} intentó interactuar con " +
                $"{target.name} desde demasiado lejos.",
                this);

            return;
        }

        if (!target.CanServerInteract(netIdentity))
        {
            return;
        }

        target.ServerInteract(netIdentity);
    }

    [Server]
    private bool IsWithinServerRange(
        RatInteractable target)
    {
        Vector3 serverOrigin =
            interactionOrigin != null
                ? interactionOrigin.position
                : transform.position;

        Collider targetCollider =
            target.GetComponentInChildren<Collider>();

        Vector3 closestPoint =
            targetCollider != null
                ? targetCollider.ClosestPoint(serverOrigin)
                : target.transform.position;

        float squaredDistance =
            (closestPoint - serverOrigin).sqrMagnitude;

        return squaredDistance <=
               maximumServerDistance *
               maximumServerDistance;
    }

    private void DrawInteractionRay()
    {
        if (!drawDebugRay)
        {
            return;
        }

        Debug.DrawRay(
            interactionOrigin.position,
            interactionOrigin.forward *
            interactionDistance,
            currentTarget != null
                ? Color.green
                : Color.red);
    }

    private void OnDisable()
    {
        currentTarget = null;
    }
}