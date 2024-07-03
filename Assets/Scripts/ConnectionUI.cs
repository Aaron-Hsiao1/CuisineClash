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
			CuisineClashMultiplayer.Instance.StartHost();
			Loader.LoadNetwork(Loader.Scene.ConnectionLobby);
		});
		joinGameButton.onClick.AddListener(() =>
		{
			CuisineClashMultiplayer.Instance.StartClient();
		});
	}
}
