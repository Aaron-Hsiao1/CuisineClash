using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using System;


public class ConnectionUI : MonoBehaviour
{
	/*[SerializeField] private Button hostGameButton;
	[SerializeField] private Button joinGameButton;

	public string environment = "production";

	//private string relayJoinCode;

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


	/*private async void StartClient()
	{
		JoinAllocation joinAllocation = await JoinRelay();
		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
		
		CuisineClashMultiplayer.Instance.StartClient();
	}

	private async void StartHost()
	{
		Allocation allocation = await AllocateRelay();
		Debug.Log($"relayjoincode: {GetRelayJoinCode(allocation)}");
		string relayJoinCode = await GetRelayJoinCode(allocation);
		CuisineClashMultiplayer.Instance.SetJoinCode(relayJoinCode);

		NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

		CuisineClashMultiplayer.Instance.StartHost();
	}


	private async Task<JoinAllocation> JoinRelay(string joinCode)
	{
		try
		{
			JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
			return joinAllocation;
		}
		catch (RelayServiceException e)
		{
			Debug.Log(e);
			return default;
		}

	}*/

}
