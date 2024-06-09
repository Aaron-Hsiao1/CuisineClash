using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SandwichMakerManager : MonoBehaviour //Manages everything about the game
{
	public PlayerSandwich playerSandwich;
	public RecipeListSO recipeListSO;

	private RecipeSO currentRecipe;
	// Start is called before the first frame update
	void Start()
	{
		currentRecipe = getRandomRecipe();
		Debug.Log(currentRecipe.ToString());
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.G))
		{
			CheckSandwich(currentRecipe, playerSandwich);
		}
	}

	bool CheckSandwich(RecipeSO recipe, PlayerSandwich playerSandwich)
	{
		if (recipe.recipeSO.Count != playerSandwich.playerSandwich.Count) //i need better variable names this is getting out of hand
		{
			return false;
		}
		for (int i = 0; i < recipe.recipeSO.Count; i++)
		{
			if (recipe.recipeSO[i] != playerSandwich.playerSandwich[i])
			{
				return false;
			}
		}
		return true;
	}

	private RecipeSO getRandomRecipe()
	{
		return recipeListSO.recipeListSO[UnityEngine.Random.Range(0, recipeListSO.recipeListSO.Count)];
	}
}
