using UnityEngine;
using System.Collections;
using Unity.Netcode;
using TMPro;

public class MeatballManager : NetworkBehaviour
{
    [SerializeField] private RainingMeatballManager rainingMeatballManager;

    private Rigidbody rb;

    private float gravity = 25f;

    //private TMP_Text gameOverText;

    private void Start()
    {
        /*if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }*/
        rb = GetComponent<Rigidbody>();
        rainingMeatballManager = GameObject.Find("Raining Meatball Manager").GetComponent<RainingMeatballManager>();
    }

    public override void OnNetworkSpawn()
    {
        //Debug.Log("");
        //gameOverText = GameObject.Find("GameOverText").GetComponent<TMP_Text>();
        rainingMeatballManager = GameObject.Find("Raining Meatball Manager").GetComponent<RainingMeatballManager>();
        //Debug.Log("meatballManager != null" + meatballManager != null);
    }

    void FixedUpdate()
    {
        rb.AddForce(Vector3.down * gravity * rb.mass);
    }

    private void OnTriggerEnter(Collider collision)
	{
        if (collision.CompareTag("Player")) //enter death zone
        {
            Debug.Log("Player died");
            RemoveFromPlayerListServerRpc();
        }

        if (collision.gameObject.CompareTag("Ground") && IsServer)
		{
            DestroyMeatballServerRpc();
        }

        
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveFromPlayerListServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        Debug.Log("Sender Client ID:" + clientId);
        Destroy(NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.gameObject); //object destroyed but stillt rying to access after host dies and then the clinet dies.
        rainingMeatballManager.EliminatePlayer(clientId);

        Debug.Log("Player removed: " + clientId);

        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

    [ServerRpc]
	private void DestroyMeatballServerRpc()
	{
		Destroy(gameObject);
	}
}
