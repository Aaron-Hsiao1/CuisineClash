using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PregameLobbyManager : NetworkBehaviour
{
	public static PregameLobbyManager Instance { get; private set; }

	//private Dictionary<ulong, bool> playerReadyDictionary;

	private void Awake()
	{
		Instance = this;

		//playerReadyDictionary = new Dictionary<ulong, bool>();
	}

}
