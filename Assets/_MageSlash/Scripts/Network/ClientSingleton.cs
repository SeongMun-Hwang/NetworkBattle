using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ClientSingleton : MonoBehaviour
{
    static ClientSingleton instance;
    public static ClientSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singleton = new GameObject("ClientSingleton");
                instance = singleton.AddComponent<ClientSingleton>();

                DontDestroyOnLoad(singleton);
            }
            return instance;
        }
    }
    JoinAllocation allocation;
    public async Task<bool> InitAsync()
    {
        await UnityServices.InitializeAsync();
        AuthState state = await Authenticator.DoAuth();

        if (state == AuthState.Authenticated)
        {
            return true;
        }
        return false;
    }
    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation=await RelayService.Instance.JoinAllocationAsync(joinCode);
        }
        catch(RelayServiceException e) 
        {
            Debug.LogException(e);
            return;
        }
        UnityTransport transport=NetworkManager.Singleton.GetComponent<UnityTransport>();
        RelayServerData relayServerData = allocation.ToRelayServerData("dtls");
        transport.SetRelayServerData(relayServerData);
        NetworkManager.Singleton.StartClient();
    }
}
