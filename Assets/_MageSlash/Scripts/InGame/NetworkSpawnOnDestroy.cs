using Unity.Netcode;
using UnityEngine;

public class NetworkSpawnOnDestroy : NetworkBehaviour
{
    [SerializeField] GameObject prefab;
    private void OnDestroy()
    {
        if(!IsServer) return;
        GameObject go = Instantiate(prefab);
        go.transform.position=transform.position;
        go.GetComponent<NetworkObject>().Spawn();
    }
}
