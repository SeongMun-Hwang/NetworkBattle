using Unity.Netcode;
using UnityEngine;

public class MagicPoint : NetworkBehaviour
{
    public float magicPoint = 100;
    public NetworkVariable<float> currentMagic=new NetworkVariable<float>(100,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    float regenerateRate = 2f;
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        currentMagic.Value = magicPoint;
    }

    void Update()
    {
        if(!IsOwner) return;
        RestoreMagic(regenerateRate * Time.deltaTime);
        currentMagic.Value=Mathf.Clamp(currentMagic.Value, 0, magicPoint);
    }
    public bool UseMagic(float magic)
    {
        if(currentMagic.Value < magic) return false;
        ModifyMagicPoint(-magic);
        return true;
    }
    [ClientRpc]
    public void RestoreClientRpc(float magic,ClientRpcParams clientRpcParams)
    {
        RestoreMagic(magic);
    }
    public void RestoreMagic(float magic)
    {
        ModifyMagicPoint(magic);
    }
    void ModifyMagicPoint(float value)
    {
        currentMagic.Value=Mathf.Clamp(currentMagic.Value+value,0,magicPoint);
    }
}
