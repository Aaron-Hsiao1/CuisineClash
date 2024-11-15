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

	[SerializeField] private Rigidbody rb;

	public override void OnNetworkSpawn()
	{
		Debug.Log("player sapwned on server" + NetworkManager.Singleton.LocalClientId);
		PlayerSpawnFix();
		if (IsOwner)
		{
			LocalInstance = this;

		}
		if (IsServer)
		{
			NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
		}
	}

	private void PlayerSpawnFix()
	{
		PlayerSpawnFixServerRpc();
    }

	[ServerRpc(RequireOwnership = false)]
	private void PlayerSpawnFixServerRpc()
	{
        //Debug.Log("Player spawned on server, IsServer == true");
        //Debug.Log(NetworkManager.Singleton.LocalClientId);
        SpawnManager spawnManager = GameObject.Find("Spawn Points").GetComponent<SpawnManager>();
        Vector3 nextSpawnPoint = spawnManager.GetNextSpawnPoint();
        SetPlayerLocation(nextSpawnPoint.x, nextSpawnPoint.y, nextSpawnPoint.z);
        //Debug.Log($"Transfrom.position: {rb.position}");
    }
	
	[ClientRpc]
	private void SetPlayerLocationClientRpc(float x, float y, float z)
	{
		rb.position = new Vector3(x, y, z);
		//Debug.Log($"rb.psition client: {rb.position}");
	}

	private void SetPlayerLocation(float x, float y, float z)
	{
		SetPlayerLocationClientRpc(x, y, z);
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
		playerVisual.SetPlayerColor(CuisineClashMultiplayer.Instance.GetPlayerOuterColor(playerData.outerColorId), CuisineClashMultiplayer.Instance.GetPlayerInnerColor(playerData.innerColorId));
		playerName.text = playerData.playerName.ToString();
	}

}
