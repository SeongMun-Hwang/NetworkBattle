using UnityEngine;

public class SpawnOnDestroy : MonoBehaviour
{
    [SerializeField] GameObject effectPrefab;

    private void OnDestroy()
    {
        Instantiate(effectPrefab).transform.position=transform.position;
    }
}
