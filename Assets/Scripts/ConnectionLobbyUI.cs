using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;

public class ConnectionLobbyUI : NetworkBehaviour
{
	[SerializeField] private Button startGameButton;
	// Start is called before the first frame update
	void Start()
	{
		if (IsHost)
		{
			startGameButton.gameObject.SetActive(true);
		}
	}

	private void Awake()
	{
		startGameButton.onClick.AddListener(() =>
		{
			Loader.LoadNetwork(Loader.Scene.MultiplayerTesting);
		});
	}
}