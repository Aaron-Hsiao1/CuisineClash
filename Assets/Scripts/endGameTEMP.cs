using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class endGameTEMP : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void OnCollisionEnter(Collision other)
	{
		if (other.gameObject.CompareTag("DeathZone"))
		{
			Loader.Load(Loader.Scene.TestingLobby);
		}
	}
}
