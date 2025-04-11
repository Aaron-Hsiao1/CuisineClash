using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using Unity.Services.Lobbies.Models;

public class ConnectionLobbyUI : NetworkBehaviour
{
	[SerializeField] private Button startGameButton;
	[SerializeField] private Button mainMenuButton;
	[SerializeField] private TextMeshProUGUI lobbyNameText;
	[SerializeField] private TMP_Text lobbyCodeText;

	[SerializeField] private Button hideLobbyCodeButton;
	[SerializeField] private Button showLobbyCodeButton;

	[SerializeField] private GamemodeManager gamemodeManager;

	// Start is called before the first frame update
	void Start()
	{
		if (IsHost)
		{
			startGameButton.gameObject.SetActive(true);
		}
		Lobby lobby = CuisineClashLobby.Instance.GetLobby();

		lobbyNameText.text = $"Lobby Name: {lobby.Name}";
		lobbyCodeText.text = $"Lobby Code: {lobby.LobbyCode}";
	}

	private void Awake()
	{
		gamemodeManager = gamemodeManager = GameObject.FindGameObjectWithTag("Gamemode Manager").GetComponent<GamemodeManager>();
        string gamemode = gamemodeManager.GamemodeSelector();

        startGameButton.onClick.AddListener(() =>
		{
			CuisineClashLobby.Instance.DeleteLobby();
			Loader.LoadNetwork(gamemode);
		});
		mainMenuButton.onClick.AddListener(() =>
		{
			CuisineClashLobby.Instance.LeaveLobby();
			NetworkManager.Singleton.Shutdown();
			Loader.Load(Loader.Scene.MainMenu);
		});
		hideLobbyCodeButton.onClick.AddListener(() =>
		{
			HideLobbyCode();
		});
		showLobbyCodeButton.onClick.AddListener(() =>
		{
			ShowLobbyCode();
		});
	}

	private void HideLobbyCode()
	{
		lobbyCodeText.gameObject.SetActive(false);
		showLobbyCodeButton.gameObject.SetActive(true);
		hideLobbyCodeButton.gameObject.SetActive(false);
	}
	private void ShowLobbyCode()
	{
		lobbyCodeText.gameObject.SetActive(true);
		hideLobbyCodeButton.gameObject.SetActive(true);
		showLobbyCodeButton.gameObject.SetActive(false);
	}
}