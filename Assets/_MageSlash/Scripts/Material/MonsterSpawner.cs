using Unity.Netcode;
using UnityEngine;

public class MonsterSpawner : NetworkBehaviour
{
    [SerializeField] GameObject[] monsterPrefabs;
    [SerializeField] int maxMonsters = 20;
    [SerializeField] Vector2 minSpawnPosition;
    [SerializeField] Vector2 maxSpawnPosition;
    [SerializeField] LayerMask layerMask;

    float monsterRadius;
    Collider[] monsterBuffer = new Collider[1];
    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        monsterRadius = monsterPrefabs[0].GetComponent<SphereCollider>().radius;
        for(int i = 0; i < maxMonsters; i++)
        {
            SpawnMonster();
        }
    }
    void SpawnMonster()
    {
        GameObject go = Instantiate(monsterPrefabs[Random.Range(0, monsterPrefabs.Length)],GetSpawnPosition(),Quaternion.identity);
        go.GetComponent<NetworkObject>().Spawn();
    }
    Vector3 GetSpawnPosition()
    {
        float x = 0;
        float z = 0;
        while (true)
        {
            x=Random.Range(minSpawnPosition.x, maxSpawnPosition.x);
            z=Random.Range(minSpawnPosition.y, maxSpawnPosition.y);

            Vector3 spawnPos = new Vector3(x, 0, z);

            int numColliders = Physics.OverlapSphereNonAlloc(spawnPos, monsterRadius, monsterBuffer,layerMask);
            if(numColliders == 0)
            {
                return spawnPos;
            }
        }
    }
}
