using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplayer : NetworkBehaviour
{
    [SerializeField] Health health;
    [SerializeField] MagicPoint magicPoint;
    [SerializeField] Image healthBarImg;
    [SerializeField] Image manaBarImg;

    [SerializeField] TMP_Text userNameTmp;
    NetworkVariable<FixedString128Bytes> userName = new NetworkVariable<FixedString128Bytes>();
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
        if (IsServer)
        {
            userName.Value = ServerSingleton.Instance.clientIdToUserData[OwnerClientId].userName;
        }
        if(userNameTmp != null)
        {
            userName.OnValueChanged += HandleUserNameChange;
            UpdateName();
        }

    }

    private void HandleUserNameChange(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        UpdateName();
    }
    void UpdateName()
    {
        if(userNameTmp != null)
        {
            string name = userName.Value.ToString();
            if (name.Contains("#"))
            {
                name = name.Substring(0, name.LastIndexOf("#"));
            }
            userNameTmp.text = name;
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
        if (userName != null)
        {
            userName.OnValueChanged -= HandleUserNameChange;
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
