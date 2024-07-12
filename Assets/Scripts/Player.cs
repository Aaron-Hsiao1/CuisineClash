using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Player : NetworkBehaviour
{
	public static Player LocalInstance { get; private set; }

	public override void OnNetworkSpawn()
	{
		if (IsOwner)
		{
			LocalInstance = this;

		}
		if (IsServer)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
		}
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
	{
		if (clientId == OwnerClientId)
		{
			Debug.Log("player " + clientId + " disconnected");
		}
	}

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
