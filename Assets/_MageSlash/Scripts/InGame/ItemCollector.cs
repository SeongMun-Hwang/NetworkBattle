using Unity.Netcode;
using UnityEngine;

public class ItemCollector : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<MagicPointItem>(out var item)) return;
        float magic = item.Collect();
        if(!IsServer) return;

        GetComponent<MagicPoint>().RestoreClientRpc(magic, new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new[] { OwnerClientId }
            }
        });
    }
}
