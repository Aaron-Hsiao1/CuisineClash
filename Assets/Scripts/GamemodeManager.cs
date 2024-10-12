using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;
using System.Runtime.CompilerServices;

public class GamemodeManager : NetworkBehaviour
{
	public static GamemodeManager Instance { get; private set; }

	private static List<string> gamemodeList;

	private enum Gamemode
	{
		//MultiplayerTesting,
		RainingMeatball,
		KingOfTheGrill
	}

	private void Awake()
	{
		Instance = this;

		DontDestroyOnLoad(gameObject);

		gamemodeList = new List<string>();
	}

	public override void OnNetworkSpawn()
	{
		Debug.Log("on netowrk spawn: " + IsHost);
		if (IsHost)
		{
			foreach (Gamemode gamemode in Enum.GetValues(typeof(Gamemode)))
			{
				Debug.Log("insantianteing gamemodeList...");
				gamemodeList.Add(gamemode.ToString());
				//gamemodeListInstantiated = true;
			}
			Debug.Log("gamemodelist count: " + gamemodeList.Count);
		}
	}

	public void Start()
	{
		Debug.Log("start");
		//Debug.Log("Awake + gamemodeListInstnatiated: " + gamemodeListInstantiated);

	}

	private void Update()
	{
		if (Input.GetKey(KeyCode.N))
		{
			Debug.Log("gamemode list.count: " + gamemodeList.Count);
		}
	}

	public Loader.Scene gamemodeSelector()
	{
		Debug.Log("gamemode selectoer rnning");
		if (IsHost)
		{
			int random = UnityEngine.Random.Range(0, gamemodeList.Count); //selects a random index from the list of gamemodes
			string nextGamemode = gamemodeList[random]; //picks the next gamemode based on the index

			foreach (Loader.Scene scene in Enum.GetValues(typeof(Loader.Scene))) //loops through the Loader.Scene to find the scene that matches with the gamemode
			{
				if (scene.ToString() == nextGamemode)
				{
					Debug.Log("random: " + random);
					Debug.Log("gamemode count: " + gamemodeList.Count);
					gamemodeList.RemoveAt(random);
					Debug.Log("gamemode removed!");
					Debug.Log("gamemode count: " + gamemodeList.Count);
					return scene; //loads the gamemode
				}
			}
		}
		Debug.Log("Not HOst");
		return Loader.Scene.MainMenu;
	}

	public List<string> GetGamemodeList()
	{
		return gamemodeList;
	}
}
