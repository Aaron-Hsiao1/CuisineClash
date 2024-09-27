using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class DeathZone : NetworkBehaviour
{
	public static DeathZone Instance { get; private set; }
	
	[SerializeField] private RainingMeatballManager meatballManager;
	//[SerializeField] private CountdownTimer timer;

    public event EventHandler OnAlivePlayersChanged; 

    private TMP_Text gameOverText;

    private void Awake()
    {
		Instance = this;
    }

    private void Start()
	{
		if (gameOverText != null)
		{
			gameOverText.gameObject.SetActive(false);
		}
    }

	private void Update()
	{
		
    }

    public override void OnNetworkSpawn()
	{
		gameOverText = GameObject.Find("GameOverText").GetComponent<TMP_Text>();
		meatballManager = GameObject.Find("Raining Meatball Manager").GetComponent<RainingMeatballManager>();
		Debug.Log("meatballManager != null" + meatballManager != null);
	}

	private void OnTriggerEnter(Collider touch)
	{
		Debug.Log("trigger is host: " + IsHost);
		if (touch.CompareTag("Player")) //enter death zone
		{
			Debug.Log("Player hit death zone");
			if (!IsHost)
			{
                InvokeAlivePlayersChangedServerRpc();
            }
			else
			{
                OnAlivePlayersChanged?.Invoke(this, null);
            }	
			
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

	[ServerRpc(RequireOwnership = false)]
	private void InvokeAlivePlayersChangedServerRpc()
	{
		Debug.Log("server rpc called");
        if (OnAlivePlayersChanged != null) //this is jnull for some reason
        {
            Debug.Log("Invoking OnAlivePlayersChanged on server.");
            OnAlivePlayersChanged?.Invoke(this, null);
        }
        else
        {
            Debug.LogWarning("No subscribers for OnAlivePlayersChanged on server.");
        }
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
