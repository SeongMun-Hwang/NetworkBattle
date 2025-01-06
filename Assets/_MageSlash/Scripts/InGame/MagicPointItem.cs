using Unity.Netcode;
using UnityEngine;

public class MagicPointItem : NetworkBehaviour
{
    [SerializeField] float magicPoint = 50;
    public float Collect()
    {
        if (!IsServer) return 0;
        Destroy(gameObject);
        return magicPoint;
    }
}
