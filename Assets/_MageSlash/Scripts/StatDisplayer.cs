using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplayer : NetworkBehaviour
{
    [SerializeField] Health health;
    [SerializeField] MagicPoint magicPoint;
    [SerializeField] Image healthBarImg;
    [SerializeField] Image manaBarImg;
    public override void OnNetworkSpawn()
    {
        if (!IsClient) { return; }
        if (health != null)
        {
            health.currentHealth.OnValueChanged += HandleHealthChange;
        }        
        if (magicPoint != null)
        {
            magicPoint.currentMagic.OnValueChanged += HandleMagicPointChange;
        }
    }
    public override void OnNetworkDespawn()
    {
        if (!IsClient) { return; }
        if (health != null)
        {
            health.currentHealth.OnValueChanged -= HandleHealthChange;
        }
        if (magicPoint != null)
        {
            health.currentHealth.OnValueChanged -= HandleHealthChange;
        }
    }

    void HandleHealthChange(int oldHealth, int newHealth)
    {
        healthBarImg.fillAmount = newHealth / (float)health.maxHealth;
    }    
    void HandleMagicPointChange(float oldpoint, float newpoint)
    {
        manaBarImg.fillAmount = newpoint / magicPoint.magicPoint;
    }
}
