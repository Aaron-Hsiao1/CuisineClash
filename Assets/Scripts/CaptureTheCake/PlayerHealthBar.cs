﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
public class HealthBar : NetworkBehaviour
{
	public Slider slider; 
	public Gradient gradient;
	public Image fill;

	public void SetMaxHealth(int health)
	{
		slider.maxValue = health;
		slider.value = health;

		fill.color = gradient.Evaluate(1f);
	}

    public void SetHealth(int health) //CLIENT CANNOT CALL SERVER RPCS 
	{
        SetHealthServerRpc(health);
		Debug.Log("SEt health called on client: " + NetworkManager.Singleton.LocalClientId);
    }

	[ServerRpc]
	private void SetHealthServerRpc(int health)
	{
		SetHealthClientRpc(health);

    }

	[ClientRpc]
	private void SetHealthClientRpc(int health)
	{
		Debug.Log("Setting health on all clients");
		Debug.Log("Slider.value: " + slider.value);
		slider.value = health;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

}