using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TestingManager : MonoBehaviour
{
	// Start is called before the first frame update
	void Start()
	{
		Debug.Log("HOST");
		NetworkManager.Singleton.StartHost();
		//Hide();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
