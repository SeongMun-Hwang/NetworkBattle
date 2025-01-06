using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using System.Collections;
public class HostSingleton : MonoBehaviour
{
    static HostSingleton instance;
    public static HostSingleton Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject singleton = new GameObject("HostSingleton");
                instance = singleton.AddComponent<HostSingleton>();

                DontDestroyOnLoad(singleton);
            }
            return instance;
        }
    }
    const int MaxConnections = 10;
    Allocation allocation;
    string joinCode;
    string lobbyId;
    public async Task StartHostAsync()
    {
        try
        {
            allocation = await RelayService.Instance.CreateAllocationAsync(MaxConnections);
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch(RelayServiceException e)
        {
            Debug.LogException(e);
            return;
        }
        UnityTransport transport=NetworkManager.Singleton.GetComponent<UnityTransport>();
        //ToRelayServerData 옵션 : udp - ,dtls - ,ws - ,wss - 
        RelayServerData relayServerData = allocation.ToRelayServerData("dtls");
        transport.SetRelayServerData(relayServerData);

        //로비 생성
        try
        {
            CreateLobbyOptions options=new CreateLobbyOptions();
            options.IsPrivate = false;
            options.Data = new Dictionary<string, Unity.Services.Lobbies.Models.DataObject>
            {
                {
                    //Unity.Services.Lobbies.Models.DataObject.VisibilityOptions.Member : 누구까지 볼 수 있는가
                    "JoinCode",new Unity.Services.Lobbies.Models.DataObject(Unity.Services.Lobbies.Models.DataObject.VisibilityOptions.Member, joinCode)
                }
            };
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(joinCode, MaxConnections, options);
            lobbyId = lobby.Id;
            StartCoroutine(HeartBeatLobby(15));
        }
        catch(LobbyServiceException e)
        {
            Debug.LogException(e);
            return;
        }

        NetworkManager.Singleton.StartHost();
        NetworkManager.Singleton.SceneManager.LoadScene("BattleScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
    IEnumerator HeartBeatLobby(float waitTimeSeconds)
    {
        var delay = new WaitForSecondsRealtime(waitTimeSeconds);
        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            yield return delay;
        }
    }
}
