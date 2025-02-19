using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private Button createLobbyButton;
	[SerializeField] private Button quickJoinButton;
	[SerializeField] private LobbyCreateUI lobbyCreateUI;
	[SerializeField] private Transform lobbyContainer;
	[SerializeField] private Transform lobbyTemplate;
	[SerializeField] private Button joinCodeButton;
	[SerializeField] private TMP_InputField joinCodeInputField;
	[SerializeField] private TMP_InputField playerNameInputField;

	private void Awake()
	{


	}

	private void Start()
	{
		mainMenuButton.onClick.AddListener(() =>
		{
			CuisineClashLobby.Instance.LeaveLobby();
			Loader.Load(Loader.Scene.MainMenu);
		});
		createLobbyButton.onClick.AddListener(() =>
		{
			lobbyCreateUI.Show();
		});
		quickJoinButton.onClick.AddListener(() =>
		{
			CuisineClashLobby.Instance.QuickJoin();
		});
		joinCodeButton.onClick.AddListener(() =>
		{
			CuisineClashLobby.Instance.JoinWithCode(joinCodeInputField.text);
		});

		lobbyTemplate.gameObject.SetActive(false);

		//playerNameInputField.text = CuisineClashMultiplayer.Instance.GetPlayerName();
		playerNameInputField.text = "Enter Player Name Here";

		playerNameInputField.onValueChanged.AddListener((string newText) =>
		{
			CuisineClashMultiplayer.Instance.SetPlayerName(newText);
		});

		CuisineClashLobby.Instance.OnLobbyListChanged += CuisineClashLobby_OnLobbyListChanged;
		UpdateLobbyList(new List<Lobby>());
	}

	private void CuisineClashLobby_OnLobbyListChanged(object sender, CuisineClashLobby.OnLobbyListChangedEventArgs e)
	{
		UpdateLobbyList(e.lobbyList);
	}

	private void UpdateLobbyList(List<Lobby> lobbyList)
	{
		foreach (Transform child in lobbyContainer)
		{
			if (child == lobbyTemplate) continue;
			Destroy(child.gameObject);
		}

		foreach (Lobby lobby in lobbyList)
		{
			Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
			lobbyTransform.gameObject.SetActive(true);
			lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
		}
	}

	private void OnDestroy()
	{
		CuisineClashLobby.Instance.OnLobbyListChanged -= CuisineClashLobby_OnLobbyListChanged;
	}

}
