using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HostDisconnectUI : NetworkBehaviour
{
	[SerializeField] private Button returnToMainMenu;
	[SerializeField] private Camera secondaryCamera;
	[SerializeField] private GameObject disconnectUI;

	private void Awake()
	{
		returnToMainMenu.onClick.AddListener(() =>
		{
			Hide();
			Loader.Load(Loader.Scene.MainMenu);
		});
	}

	private void Start()
	{
		NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectedCallback;
	}


	private void NetworkManager_OnClientDisconnectedCallback(ulong clientId)
	{
		if (clientId == NetworkManager.ServerClientId || clientId == NetworkManager.Singleton.LocalClientId)
		{
			secondaryCamera.gameObject.SetActive(true);
			secondaryCamera.tag = "MainCamera";
			Show();
		}
	}

	private void Show()
	{
		Cursor.lockState = CursorLockMode.None;
		disconnectUI.SetActive(true);
	}
	private void Hide()
	{
		disconnectUI.SetActive(false);
	}
}
