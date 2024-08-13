using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerKickedUI : MonoBehaviour
{
	[SerializeField] private Button mainMenuButton;


	private void Awake()
	{
		mainMenuButton.onClick.AddListener(() =>
		{
			Loader.Load(Loader.Scene.MainMenu);
		});
	}

	private void Start()
	{
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
		Hide();
	}

	private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
	{
		Debug.Log("client disconnected (player kicked)");
		if (clientId == NetworkManager.Singleton.LocalClientId)
		{
			Show();
		}
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	private void Hide()
	{
		gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		NetworkManager.Singleton.OnClientDisconnectCallback -= NetworkManager_OnClientDisconnectCallback;
	}
}
