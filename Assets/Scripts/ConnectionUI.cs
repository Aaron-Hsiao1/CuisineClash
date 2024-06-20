using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class ConnectionUI : MonoBehaviour
{
	[SerializeField] private Button hostGameButton;
	[SerializeField] private Button joinGameButton;

	private void Awake()
	{
		hostGameButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.StartHost();
			Loader.LoadNetwork(Loader.Scene.MultiplayerTesting);
		});
		joinGameButton.onClick.AddListener(() =>
		{
			NetworkManager.Singleton.StartClient();
		});
	}
}
