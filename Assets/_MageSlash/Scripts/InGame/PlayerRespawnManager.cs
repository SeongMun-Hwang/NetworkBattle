using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerRespawnManager : NetworkBehaviour
{
    [SerializeField] NetworkObject playerPrefab;
    //OnNetworkSpawn -> 이미 존재하는 플레이어는 실행하지 않음
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
        //복제할 때 누구의 오브젝트인지 지정. 리스폰을 해주는 건 서버지만, 사망한 유저의 아이디를 가져야 함
        no.SpawnAsPlayerObject(ownerClientId);
    }

}
