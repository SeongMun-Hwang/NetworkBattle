using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

//NetworkBehaviour: ��Ʈ��ũ ����� �̿��� �� �ִ� MonoBehaviour�� Ȯ����
public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] GameObject spawnedObjectPrefab;
    GameObject spawnedObject;
    //����ü ������ �����ϱ� ���� ����
    struct SomeData : INetworkSerializable
    {
        public bool _bool;
        public int _int;
        public FixedString32Bytes _string;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _bool);
            serializer.SerializeValue(ref _int);
            serializer.SerializeValue(ref _string);
        }
    }

    //NetworkVariable : ��� ��Ʈ��ũ���� ���������� ����ϴ� ����. �� Value�� ���� ����ؾ� ��
    //�� ��° ���ڸ� ���� ���� ��� ����. �б� ����Ʈ : Everyone, ���� ����Ʈ : Server
    NetworkVariable<SomeData> randomNumber = new NetworkVariable<SomeData>(
        new SomeData
        {
            _bool = true,
            _int = 1
        },
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );
    //������ ���� ȣ�� �ּ�ȭ
    public override void OnNetworkSpawn()
    {
        randomNumber.OnValueChanged += HandleRandomNumber;
    }
    public override void OnNetworkDespawn()
    {
        randomNumber.OnValueChanged -= HandleRandomNumber;
    }
    void HandleRandomNumber(SomeData oldValue, SomeData newValue)
    {
        Debug.Log("Owner id : " + OwnerClientId + " random number : " + randomNumber.Value._int + " string : " + randomNumber.Value._string);

    }
    void Update()
    {

        //���� ��ü�� �ƴϸ� ���� X
        if (!IsOwner) { return; }
        //��Ʈ��ũ�� ������Ʈ�� Instantiate�ϴ� ���
        if (Input.GetKeyDown(KeyCode.X))
        {
            spawnedObject = Instantiate(spawnedObjectPrefab);
            //client���� �ϸ� ������. ���� ����. Ŭ���̾�Ʈ���� �ϰ� �ʹٸ� serverRpc �ʿ�
            spawnedObject.GetComponent<NetworkObject>().Spawn(true); 
        }       
        //��Ʈ��ũ ������Ʈ despawn
        if (Input.GetKeyDown(KeyCode.C))
        {
            Destroy(spawnedObject);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            randomNumber.Value = new SomeData
            {
                _int = Random.Range(0, 100),
                _string = "���̻�����ĥ�ȱ�",
            };
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            //TestServerRpc();
            TestClientRpc(new ClientRpcParams
            {
                Send = new ClientRpcSendParams
                {
                    TargetClientIds = new List<ulong> { 1 }
                }
            });
        }
        Vector3 moveDir = Vector3.zero;
        moveDir.x = Input.GetAxis("Horizontal");
        moveDir.z = Input.GetAxis("Vertical");

        float moveSpeed = 10;
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }
    //Rpc : remote procedure call ������ �ִ� �Լ��� �����. �Լ��� �ݵ�� ServerRpc�� ������ �� ����
    [ServerRpc]
    //ServerRpcParams : serverrpc ȣ�� �� ��밡���� �Ķ����
    void TestServerRpc(ServerRpcParams rpcParams)
    {
        Debug.Log("Server RPC called " + rpcParams.Receive.SenderClientId);
    }
    [ClientRpc]
    void TestClientRpc(ClientRpcParams rpcParams)
    {
        Debug.Log("Client Rpc called");
    }
}