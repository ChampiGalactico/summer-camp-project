using Mirror;
using UnityEngine;

public sealed class NetworkPlayerVisualMode : NetworkBehaviour
{
    [Header("Visual Roots")]
    [SerializeField]
    private GameObject firstPersonRig;

    [SerializeField]
    private GameObject thirdPersonModel;

    /// <summary>
    /// Se ejecuta en todas las copias del jugador presentes en cada cliente.
    /// El estado predeterminado representa a un jugador remoto.
    /// </summary>
    public override void OnStartClient()
    {
        base.OnStartClient();
        ApplyRemoteVisuals();
    }

    /// <summary>
    /// Mirror ejecuta este callback solamente en el jugador
    /// perteneciente al cliente local.
    /// </summary>
    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        ApplyLocalVisuals();
    }

    private void ApplyLocalVisuals()
    {
        SetActiveSafely(firstPersonRig, true);
        SetActiveSafely(thirdPersonModel, false);
    }

    private void ApplyRemoteVisuals()
    {
        SetActiveSafely(firstPersonRig, false);
        SetActiveSafely(thirdPersonModel, true);
    }

    private static void SetActiveSafely(GameObject target, bool isActive)
    {
        if (target != null && target.activeSelf != isActive)
        {
            target.SetActive(isActive);
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (firstPersonRig != null &&
            firstPersonRig == thirdPersonModel)
        {
            Debug.LogError(
                "First Person Rig y Third Person Model " +
                "no pueden apuntar al mismo GameObject.",
                this);
        }
    }
#endif
}