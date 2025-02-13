using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Core;
using UnityEngine;

public class ServerSingleton : MonoBehaviour
{
    static ServerSingleton instance;
    public ServerGameManager serverManager;
    public static ServerSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject singleton = new GameObject("ServerSingleton");
                instance = singleton.AddComponent<ServerSingleton>();

                DontDestroyOnLoad(singleton);
            }
            return instance;
        }
    }
    public Dictionary<ulong,UserData> clientIdToUserData=new Dictionary<ulong,UserData>();
    public Dictionary<string,UserData> authIdToUserData=new Dictionary<string,UserData>();
    public Action<string> OnClientLeft;

    public Action<UserData> OnUserJoined;
    public Action<UserData> OnUserLeft;
    public void Init()
    {

    }
    private void OnEnable()
    {
        //ConnectionApprovalCallback : Ŀ�ؼ��� ���ö� ������ �� ����
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;
    }

    private void OnClientDisconnect(ulong obj)
    {
        if(clientIdToUserData.ContainsKey(obj))
        {
            string authId = clientIdToUserData[obj].userAuthId;

            OnUserLeft?.Invoke(authIdToUserData[authId]);

            clientIdToUserData.Remove(obj);
            authIdToUserData.Remove(authId);

                OnClientLeft?.Invoke(authId);           
        }
    }
    public async Task CreateServer()
    {
        await UnityServices.InitializeAsync();
        serverManager = new ServerGameManager(
            ApplicationData.IP(),
            (ushort)ApplicationData.Port(),
            (ushort)ApplicationData.QPort(),
            NetworkManager.Singleton
            );
    }
    public bool OpenConnection(string ip,ushort port)
    {
        UnityTransport transport=NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetConnectionData(ip, port);

        return NetworkManager.Singleton.StartServer();
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnect;
        }
    }
    //request : ���� ��û, reponse : �� ���� ����
    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        string payload=Encoding.UTF8.GetString(request.Payload);
        UserData userData=JsonConvert.DeserializeObject<UserData>(payload);
        Debug.Log("User data : " + userData.userName);

        clientIdToUserData[request.ClientNetworkId]=userData;
        authIdToUserData[userData.userAuthId]=userData;

        OnUserJoined?.Invoke(userData);

        response.Approved = true;
        response.CreatePlayerObject = true;
        response.Position = SpawnPoint.GetRandomSpawnPoint();
        response.Position=SpawnPoint.GetRandomSpawnPoint();
        response.Rotation=Quaternion.identity;
    }
}
