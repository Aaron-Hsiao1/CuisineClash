using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPlayerSandwich : MonoBehaviour
{
	public Collider placingZone;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	public void ClearPrefabs()
	{
		GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Ingredient");
		foreach (GameObject prefab in prefabs)
		{
			if (placingZone.bounds.Contains(prefab.transform.position))
			{
				Destroy(prefab);
			}
		}
	}

}

