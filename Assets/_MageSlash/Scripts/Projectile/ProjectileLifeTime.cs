using Unity.Netcode;
using UnityEngine;

public class ProjectileLifeTime : MonoBehaviour
{
    [SerializeField] float lifeTime = 2f;
    float currentTime = 0;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
