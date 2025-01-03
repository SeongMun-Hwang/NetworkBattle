using Unity.Netcode;
using UnityEngine;

public class Monster : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        GetComponent<Health>().onDie += OnDie;
    }
    public override void OnNetworkDespawn()
    {
        GetComponent<Health>().onDie -= OnDie;
    }
    void OnDie()
    {
        Destroy(gameObject);
    }
}