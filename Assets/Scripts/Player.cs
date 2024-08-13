using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class Player : NetworkBehaviour
{
	public static Player LocalInstance { get; private set; }

	[SerializeField] private PlayerVisual playerVisual;
	[SerializeField] private TextMeshPro playerName;

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
		if (IsLocalPlayer)
		{
			playerName.gameObject.SetActive(false);
		}
		PlayerData playerData = CuisineClashMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
		playerVisual.SetPlayerColor(CuisineClashMultiplayer.Instance.getPlayerColor(playerData.colorId));
		playerName.text = playerData.playerName.ToString();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
