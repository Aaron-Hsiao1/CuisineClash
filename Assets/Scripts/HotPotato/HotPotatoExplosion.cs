using Unity.VisualScripting;
using UnityEngine;
using Unity.Netcode;

public class HotPotatoExplosion : NetworkBehaviour
{
	public GameObject hotPotato; // Reference to the hot potato child object
	public GameObject player; // Reference to the hot potato child object
	public ParticleSystem explosionSFX;

	[SerializeField] private HotPotatoManager hotPotatoManager;

	public override void OnNetworkSpawn()
	{
		hotPotatoManager = GameObject.Find("Hot Potato Manager").GetComponent<HotPotatoManager>();
	}

	public bool HasHotPotato()
	{
		return OwnerClientId == hotPotatoManager.GetPlayerWithPotato();
	}

	// Method to handle player elimination
	public void Eliminate()
	{
		Debug.Log($"{name} has been eliminated!"); // Log elimination to console
		Instantiate(explosionSFX, transform.position, Quaternion.identity);
		player.GetComponent<NetworkObject>().Despawn(true);
	}
}