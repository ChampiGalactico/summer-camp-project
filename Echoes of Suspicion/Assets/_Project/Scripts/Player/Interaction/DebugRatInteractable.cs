using UnityEngine;

public sealed class DebugRatInteractable : RatInteractable
{
    [SerializeField]
    private float rotationDegrees = 45f;

    public override void Interact(GameObject interactor)
    {
        transform.Rotate(
            Vector3.up,
            rotationDegrees,
            Space.Self);

        Debug.Log(
            $"{interactor.name} interactuó con {name}.",
            this);
    }
}