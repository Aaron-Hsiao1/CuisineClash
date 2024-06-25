using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CuisineClashMultiplayer : NetworkBehaviour
{
	public static CuisineClashMultiplayer Instance { get; private set; }

	private void Awake()
	{
		Instance = this;
	}


}
