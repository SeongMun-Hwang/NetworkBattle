using Cinemachine.Utility;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ProjectileLauncher : NetworkBehaviour
{
    [SerializeField] GameObject serverProjectilePrefab;
    [SerializeField] GameObject clientProjectilePrefab;
    [SerializeField] float projectileSpeed = 3f;
    [SerializeField] float coolTime = 1;
    [SerializeField] float initialDistance = 1;

    float coolTimer = 0f;

    private void Update()
    {
        if (!IsOwner) return;

        coolTimer += Time.deltaTime;
    }

    public void Attack(Vector3 targetPos)
    {
        if (coolTimer < coolTime) return;
        coolTimer = 0f;

        Vector3 direction = (targetPos - transform.position).normalized;
        Vector3 spawnPos = direction * initialDistance + transform.position;
        spawnPos.y = spawnPos.y + 1;

        StartCoroutine(FireAfterDelay(spawnPos, direction));

    }
    IEnumerator FireAfterDelay(Vector3 spawnPos,Vector3 direction)
    {
        yield return new WaitForSeconds(0.5f);
        SpawnDummyProjectile(spawnPos, direction);
        FireServerRpc(spawnPos, direction);
    }
    [ServerRpc]
    void FireServerRpc(Vector3 spawnPos, Vector3 direction)
    {
        //server projectile�� �ð������� ���̴°� ����. ���� �浹�� ���
        GameObject go = Instantiate(serverProjectilePrefab, spawnPos, Quaternion.identity);
        Physics.IgnoreCollision(GetComponent<Collider>(), go.GetComponent<Collider>());
        if (go.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.linearVelocity = direction * projectileSpeed;
        }
        FireClientRpc(spawnPos, direction);
    }
    [ClientRpc]
    void FireClientRpc(Vector3 spawnPos, Vector3 direction)
    {
        //���ʴ� �̹� Spawn�� �����Ƿ� ����
        if (IsOwner) return;
        SpawnDummyProjectile(spawnPos,direction);
    }
    void SpawnDummyProjectile(Vector3 spawnPos, Vector3 direction)
    {
        GameObject go =Instantiate(clientProjectilePrefab,spawnPos,Quaternion.identity);
        Physics.IgnoreCollision(GetComponent<Collider>(), go.GetComponent<Collider>());
        if(go.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            rigidbody.linearVelocity = direction * projectileSpeed;
        }
    }
}
