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
		KingOfTheGrill,
		HotPotato
		//LoadIntoHP
		//TutorialHP
		//GameEnded
	}


	private void Awake()
	{
		Instance = this;

		DontDestroyOnLoad(gameObject);

		gamemodeList = new List<string>();
	}

	public override void OnNetworkSpawn()
	{
		//Debug.Log("on netowrk spawn: gamemode manager");
		if (IsHost)
		{
			//Debug.Log("List of enum: " + Enum.GetValues(typeof(Gamemode)).Length);
			foreach (Gamemode gamemode in Enum.GetValues(typeof(Gamemode)))
			{
				//Debug.Log("insantianteing gamemodeList...");
				//Debug.Log("Adding gamemode: " + gamemode.ToString());
				gamemodeList.Add(gamemode.ToString());
				//gamemodeListInstantiated = true;
			}
			//Debug.Log("gamemodelist count: " + gamemodeList.Count);
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

	public string GamemodeSelector() //runs every gamemode.
	{
		Debug.Log("gamemode selector running");
		if (IsHost)
		{
			//Debug.Log("Is Host ran in GamemodeSelector");

			int random = UnityEngine.Random.Range(0, gamemodeList.Count); //selects a random index from the list of gamemodes
			string nextGamemode = gamemodeList[random]; //picks the next gamemode based on the index

			//Debug.Log("Random index chosen: " + random);
			//Debug.Log("Selected nextGamemode: " + nextGamemode);
			Debug.Log("Gamemode " + nextGamemode + " removed");
			Debug.Log("Gamemdoe list count: " + gamemodeList.Count);
            foreach (Gamemode gamemode in Enum.GetValues(typeof(Gamemode)))
            {
				//Debug.Log("insantianteing gamemodeList...");
				Debug.Log("Gamemode: " + gamemode.ToString());
                //gamemodeListInstantiated = true;
            }
            gamemodeList.RemoveAt(random);
			//Debug.Log("gamemode removed");
			return nextGamemode;
		}
		//Debug.Log("Not Host");
		return "MainMenu";
	}

	public List<string> GetGamemodeList()
	{
		return gamemodeList;
	}
}
