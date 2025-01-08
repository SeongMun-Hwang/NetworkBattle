using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class ScoreBoard : NetworkBehaviour
{
    [SerializeField] Transform scoreboardParent;
    [SerializeField] ScoreBoardItem scoreboardItemPrefab;

    NetworkList<ScoreboardData> scoreboardDatas = new NetworkList<ScoreboardData>();
    List<ScoreBoardItem> scoreBoardItems = new List<ScoreBoardItem>();
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            scoreboardDatas.OnListChanged += HandleScoreboardChanged;
            foreach (ScoreboardData data in scoreboardDatas)
            {
                HandleScoreboardChanged(new NetworkListEvent<ScoreboardData>
                {
                    Type = NetworkListEvent<ScoreboardData>.EventType.Add,
                    Value = data
                });
            }
        }
        if (IsServer)
        {
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (PlayerController player in players)
            {
                HandlePlayerSpawned(player);
            }

            PlayerController.OnPlayerSpawn += HandlePlayerSpawned;
            PlayerController.OnPlayerDespawn += HandlePlayerDespawned;

            Health.OnScored += HandleGetScore;
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            scoreboardDatas.OnListChanged -= HandleScoreboardChanged;
        }
        if (IsServer)
        {
            PlayerController.OnPlayerSpawn -= HandlePlayerSpawned;
            PlayerController.OnPlayerDespawn -= HandlePlayerDespawned;
        }
        Health.OnScored -= HandleGetScore;
    }

    private void HandlePlayerSpawned(PlayerController player)
    {
        scoreboardDatas.Add(new ScoreboardData
        {
            clientId = player.OwnerClientId,
            userName = ServerSingleton.Instance.clientIdToUserData[player.OwnerClientId].userName,
            score = 0
        });
    }
    private void HandlePlayerDespawned(PlayerController player)
    {
        foreach (ScoreboardData data in scoreboardDatas)
        {
            if (data.clientId == player.OwnerClientId)
            {
                scoreboardDatas.Remove(data);
                break;
            }
        }
    }
    //NetworkListEvent<ScoreboardData> : enum을 통해 값이 어떻게 변화한 건지 전달
    private void HandleScoreboardChanged(NetworkListEvent<ScoreboardData> changeEvent)
    {
        switch (changeEvent.Type)
        {
            case NetworkListEvent<ScoreboardData>.EventType.Add:
                {
                    if (!scoreBoardItems.Any(x => x.clientId == changeEvent.Value.clientId))
                    {
                        ScoreBoardItem item = Instantiate(scoreboardItemPrefab, scoreboardParent);
                        item.SetScore(changeEvent.Value.clientId, changeEvent.Value.userName, changeEvent.Value.score);
                        scoreBoardItems.Add(item);
                    }
                }
                break;
            case NetworkListEvent<ScoreboardData>.EventType.Remove:
                {
                    ScoreBoardItem item = scoreBoardItems.FirstOrDefault(x => x.clientId == changeEvent.Value.clientId);
                    if (item != null)
                    {
                        scoreBoardItems.Remove(item);
                        Destroy(item.gameObject);
                    }
                }
                break;
            case NetworkListEvent<ScoreboardData>.EventType.Value:
                {
                    ScoreBoardItem item = scoreBoardItems.FirstOrDefault(x => x.clientId == changeEvent.Value.clientId);
                    if (item != null)
                    {
                        item.SetScore(changeEvent.Value.clientId, changeEvent.Value.userName, changeEvent.Value.score);
                    }
                }
                break;
        }
    }
    private void HandleGetScore(ulong clientId, int score)
    {
        for (int i = 0; i < scoreboardDatas.Count; i++)
        {
            if (scoreboardDatas[i].clientId == clientId)
            {
                scoreboardDatas[i] = new ScoreboardData
                {
                    clientId = clientId,
                    userName = scoreboardDatas[i].userName,
                    score = scoreboardDatas[i].score + score
                };
                break;
            }
        }
    }
}
