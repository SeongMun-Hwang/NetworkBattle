using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerRespawnManager : NetworkBehaviour
{
    [SerializeField] NetworkObject playerPrefab;
    //OnNetworkSpawn -> �̹� �����ϴ� �÷��̾�� �������� ����
    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach(PlayerController player in players)
        {
            HandlePlayerSpawn(player);
        }

        PlayerController.OnPlayerSpawn += HandlePlayerSpawn;
        PlayerController.OnPlayerDespawn += HandlePlayerDespawn;
    }
    public override void OnNetworkDespawn()
    {
        PlayerController.OnPlayerSpawn -= HandlePlayerSpawn;
        PlayerController.OnPlayerDespawn -= HandlePlayerDespawn;
    }
    private void HandlePlayerSpawn(PlayerController player)
    {
        player.GetComponent<Health>().onDie += HandlePlayerDie;
    }
    private void HandlePlayerDespawn(PlayerController player)
    {
        player.GetComponent<Health>().onDie -= HandlePlayerDie;

    }
    private void HandlePlayerDie(Health sender)
    {
        PlayerController player = sender.GetComponent<PlayerController>();
        StartCoroutine(RespawnPlayerRoutine(player.OwnerClientId));
        Destroy(player.gameObject);
    }
    IEnumerator RespawnPlayerRoutine(ulong ownerClientId)
    {
        yield return null;

        NetworkObject no = Instantiate(playerPrefab, SpawnPoint.GetRandomSpawnPoint(), Quaternion.identity);
        //������ �� ������ ������Ʈ���� ����. �������� ���ִ� �� ��������, ����� ������ ���̵� ������ ��
        no.SpawnAsPlayerObject(ownerClientId);
    }

}
