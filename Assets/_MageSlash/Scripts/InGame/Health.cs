using System;
using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth=new NetworkVariable<int>();
    bool isDead;
    public Action<Health> onDie;
    public static Action<ulong, int> OnScored;
    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        currentHealth.Value = maxHealth;
    }
    public void TakeDamage(int damage, ulong cliendId)
    {
        ModifyHealthValue(-damage, cliendId);
    }
    public void RestoreHealth(int heal, ulong clientId)
    {
        ModifyHealthValue(heal, clientId);
    }
    void ModifyHealthValue(int value, ulong cliendId)
    {
        if(isDead) return;
        int newHealth = currentHealth.Value + value;
        currentHealth.Value=Mathf.Clamp(newHealth, 0,maxHealth);
        if (currentHealth.Value == 0)
        {
            isDead= true;
            onDie?.Invoke(this);

            if(GetComponent<PlayerController>() != null)
            {
                OnScored?.Invoke(cliendId, 10);
            }
            else if (GetComponent<Monster>() != null)
            {
                OnScored?.Invoke(cliendId, 1);
            }
        }
    }
}
