using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

//NetworkBehaviour: 네트워크 기능을 이용할 수 있는 MonoBehaviour의 확장판
public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] GameObject spawnedObjectPrefab;
    GameObject spawnedObject;
    //구조체 정보를 저장하기 위한 예시
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

    //NetworkVariable : 모든 네트워크에서 공통적으로 사용하는 변수. 꼭 Value로 값을 사용해야 함
    //두 번째 인자를 통해 접근 대상 조절. 읽기 디폴트 : Everyone, 쓰기 디폴트 : Server
    NetworkVariable<SomeData> randomNumber = new NetworkVariable<SomeData>(
        new SomeData
        {
            _bool = true,
            _int = 1
        },
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
        );
    //구독을 통한 호출 최소화
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

        //내가 주체가 아니면 동작 X
        if (!IsOwner) { return; }
        //네트워크에 오브젝트를 Instantiate하는 방법
        if (Input.GetKeyDown(KeyCode.X))
        {
            spawnedObject = Instantiate(spawnedObjectPrefab);
            //client에서 하면 오류남. 서버 권한. 클라이언트에서 하고 싶다면 serverRpc 필요
            spawnedObject.GetComponent<NetworkObject>().Spawn(true); 
        }       
        //네트워크 오브젝트 despawn
        if (Input.GetKeyDown(KeyCode.C))
        {
            Destroy(spawnedObject);
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            randomNumber.Value = new SomeData
            {
                _int = Random.Range(0, 100),
                _string = "일이삼사오륙칠팔구",
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
    //Rpc : remote procedure call 서버에 있는 함수가 실행됨. 함수가 반드시 ServerRpc로 끝나야 함 ㅋㅋ
    [ServerRpc]
    //ServerRpcParams : serverrpc 호출 시 사용가능한 파라미터
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