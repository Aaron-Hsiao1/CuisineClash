using UnityEngine;
using TMPro;
using Unity.Netcode;
using System;

public class DeathZone : NetworkBehaviour
{
	public static DeathZone Instance { get; private set; }

	private SpectateManager spectateManager;
	
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
		//gameOverText = GameObject.Find("GameOverText").GetComponent<TMP_Text>();
		meatballManager = GameObject.Find("Raining Meatball Manager").GetComponent<RainingMeatballManager>();
		spectateManager = GameObject.Find("Spectate Manager").GetComponent<SpectateManager>();
		Debug.Log("meatballManager != null" + meatballManager != null);


		OnAlivePlayersChanged += Test;
	}

    private void Test(object sender, EventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter(Collider touch)
	{
		//Debug.Log("trigger is host: " + IsHost);
		if (touch.CompareTag("Player")) //enter death zone
		{
			var clientId = touch.GetComponent<NetworkBehaviour>().OwnerClientId;
			
			Debug.Log("Player hit death zone");
            RemoveFromPlayerListServerRpc();
            //ShowGameOverText();

            if (!IsHost)
			{
                InvokeAlivePlayersChangedServerRpc();
            }
			else
			{
                OnAlivePlayersChanged?.Invoke(this, null);
            }

			spectateManager.BecomeSpectator(clientId);

		}
	}

	[ServerRpc(RequireOwnership = false)]
	private void RemoveFromPlayerListServerRpc(ServerRpcParams serverRpcParams = default)
	{
		
        var clientId = serverRpcParams.Receive.SenderClientId;

		PlayerData playerData = CuisineClashMultiplayer.Instance.GetPlayerDataFromClientId(clientId);
        var playerDataNetworkList = CuisineClashMultiplayer.Instance.GetPlayerDataNetworkList();

        playerData.isAlive = false;

		playerDataNetworkList[CuisineClashMultiplayer.Instance.GetPlayerDataIndexFromClientId(clientId)] = playerData;


        meatballManager.EliminatePlayer(clientId);
		Debug.Log(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject == null);		
        Destroy(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject);

        Debug.Log(playerData.isAlive);

		Debug.Log("Player removed: " + clientId);
	}

	[ServerRpc(RequireOwnership = false)]
	private void InvokeAlivePlayersChangedServerRpc()
	{
        OnAlivePlayersChanged?.Invoke(this, null);
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