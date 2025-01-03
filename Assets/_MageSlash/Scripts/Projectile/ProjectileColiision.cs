using Unity.Netcode;
using UnityEngine;

public class ProjectileColiision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
