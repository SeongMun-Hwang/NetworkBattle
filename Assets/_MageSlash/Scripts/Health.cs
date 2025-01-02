using Unity.Netcode;
using UnityEngine;

public class Health : NetworkBehaviour
{
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth=new NetworkVariable<int>();
    bool isDead;
    public override void OnNetworkSpawn()
    {
        if(!IsServer) return;
        currentHealth.Value = maxHealth;
    }
    public void TakeDamage(int damage)
    {
        ModifyHealthValue(-damage);
    }
    public void RestoreHealth(int heal)
    {
        ModifyHealthValue(heal);
    }
    void ModifyHealthValue(int value)
    {
        if(isDead) return;
        int newHealth = currentHealth.Value + value;
        currentHealth.Value=Mathf.Clamp(newHealth, 0,maxHealth);
        if (currentHealth.Value == 0)
        {
            isDead= true;
        }
    }
}
