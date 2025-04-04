using UnityEngine;
using TMPro;
using Unity.Netcode;
using System.Collections;

public class DeathZone : NetworkBehaviour
{
	[SerializeField] private RainingMeatballManager meatballManager;

	private TMP_Text gameOverText;

	private void Start()
	{
		if (gameOverText != null)
		{
			gameOverText.gameObject.SetActive(false);
		}

        meatballManager = GameObject.Find("Raining Meatball Manager").GetComponent<RainingMeatballManager>();
    }

	public override void OnNetworkSpawn()
	{
		Debug.Log("");
		gameOverText = GameObject.Find("GameOverText").GetComponent<TMP_Text>();
		meatballManager = GameObject.Find("Raining Meatball Manager").GetComponent<RainingMeatballManager>();
		Debug.Log("meatballManager != null" + meatballManager != null);

    }
		
	private void OnTriggerEnter(Collider touch)
	{
		if (touch.CompareTag("Player")) //enter death zone
		{
			Debug.Log("Player hit death zone");
			RemoveFromPlayerListServerRpc();
			ShowGameOverText();
		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void RemoveFromPlayerListServerRpc(ServerRpcParams serverRpcParams = default)
	{
		var clientId = serverRpcParams.Receive.SenderClientId;

        Destroy(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject);
		meatballManager.EliminatePlayer(clientId);

		Debug.Log("Player removed: " + clientId);
	}

	private void ShowGameOverText()
	{
		if (gameOverText != null)
		{
			Debug.Log("Player Died");
			gameOverText.gameObject.SetActive(true);
		}

	}


}
