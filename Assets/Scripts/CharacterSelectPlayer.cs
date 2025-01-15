using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
	[SerializeField] private int playerIndex;
	[SerializeField] private PlayerVisual playerVisual;
	[SerializeField] private Button kickButton;
	[SerializeField] private TextMeshPro playerNameText;

	private void Start()
	{
		CuisineClashMultiplayer.Instance.OnPlayerDataNetworkListChanged += CuisineClashMultiplayer_OnPlayerDataNetworkListChanged;

		kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer);

		UpdatePlayer();
	}

	private void Awake()
	{
		kickButton.onClick.AddListener(() =>
		{
			PlayerData playerData = CuisineClashMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
			CuisineClashLobby.Instance.KickPlayer(playerData.playerId.ToString());
			CuisineClashMultiplayer.Instance.KickPlayer(playerData.clientId);
		});
	}


	private void CuisineClashMultiplayer_OnPlayerDataNetworkListChanged(object sender, System.EventArgs e)
	{
		UpdatePlayer();
	}

	private void UpdatePlayer()
	{
		if (CuisineClashMultiplayer.Instance.IsPlayerIndexConnected(playerIndex))
		{
			Show();

			PlayerData playerData = CuisineClashMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
			playerVisual.SetPlayerColor(CuisineClashMultiplayer.Instance.GetPlayerOuterColor(playerData.outerColorId), CuisineClashMultiplayer.Instance.GetPlayerInnerColor(playerData.innerColorId));

			playerNameText.text = playerData.playerName.ToString();
		}
		else
		{
			Hide();
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
		CuisineClashMultiplayer.Instance.OnPlayerDataNetworkListChanged -= CuisineClashMultiplayer_OnPlayerDataNetworkListChanged;
	}
}
