using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] Transform lobbyItemParent;
    [SerializeField] LobbyItem lobbyItemPrefab;

    bool isRefreshing;
    bool isJoining;

    private void OnEnable()
    {
        RefreshList();
    }
    public async void RefreshList()
    {
        if (isRefreshing) return;
        isRefreshing = true;

        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25; //불러올 개수 지정
            options.Filters = new List<QueryFilter>
            {
                new QueryFilter(
                    QueryFilter.FieldOptions.AvailableSlots, //남아있는 자리가
                    "0", //0보다
                    QueryFilter.OpOptions.GT //큰 걸 가지고 옴
                    ),
                new QueryFilter(
                    QueryFilter.FieldOptions.IsLocked, //잠긴게
                    "0", //0==false
                    QueryFilter.OpOptions.EQ //랑 동일한 거
                    )
            };
            QueryResponse lobbies=await LobbyService.Instance.QueryLobbiesAsync(options);
            foreach(Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }
            foreach(Lobby lobby in lobbies.Results)
            {
                LobbyItem lobbyItem = Instantiate(lobbyItemPrefab, lobbyItemParent);
                lobbyItem.SetItem(this, lobby);
            }

        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
            return;
        }
    }
    public async void JoinAsync(Lobby lobby)
    {
        if (isJoining) return;
        isJoining = true;
        try
        {
            Lobby joiningLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
            string joinCode = joiningLobby.Data["JoinCode"].Value;
            await ClientSingleton.Instance.StartClientAsync(joinCode);

        }
        catch (LobbyServiceException e)
        {
            Debug.LogException(e);
        }
        isJoining=false;
    }
}
