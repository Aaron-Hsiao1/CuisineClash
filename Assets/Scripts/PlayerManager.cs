using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
	public PlayerSandwich playerSandwich;
	//

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			CheckSandwich(SandwichMakerManager.currentRecipe, playerSandwich);
			if (CheckSandwich(SandwichMakerManager.currentRecipe, playerSandwich))
			{
				//ClearSandwichPrefabs();
			}
		}
	}

	private bool CheckSandwich(RecipeSO recipe, PlayerSandwich playerSandwich)
	{
		if (recipe.recipeSO.Count != playerSandwich.playerSandwich.Count) //i need better variable names this is getting out of hand
		{
			Debug.Log("Failed");
			return false;
		}
		for (int i = 0; i < recipe.recipeSO.Count; i++)
		{
			if (recipe.recipeSO[i] != playerSandwich.playerSandwich[i])
			{
				Debug.Log("Failed");
				return false;
			}
		}
		Debug.Log("Success!");
		return true;
	}

	/*public void ClearSandwichPrefabs()
	{
		GameObject[] prefabs = GameObject.FindGameObjectsWithTag("Ingredient");
		foreach (GameObject prefab in prefabs)
		{
			if (placingZone.bounds.Contains(prefab.transform.position))
			{
				Destroy(prefab);
			}
		}
	}*/
}
