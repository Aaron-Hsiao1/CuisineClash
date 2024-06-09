using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Jobs;

public class SandwichMakerManager : MonoBehaviour //Manages everything about the game
{
	public RecipeListSO easyRecipeListSO;
	public RecipeListSO mediumRecipeListSO;
	public RecipeListSO hardRecipeListSO;

	public int roundNumber;

	public static RecipeSO currentRecipe;

	public enum recipeType
	{
		Easy,
		Medium,
		Hard
	}
	private recipeType currentRecipeType;
	// Start is called before the first frame update
	void Start()
	{
		if (roundNumber == 1)
		{
			currentRecipeType = recipeType.Easy;
		}
		else if (roundNumber == 2)
		{
			currentRecipeType = recipeType.Medium;
		}
		else if (roundNumber == 3)
		{
			currentRecipeType = recipeType.Hard;
		}
		currentRecipe = getRandomRecipe();
		Debug.Log(currentRecipe);
	}

	// Update is called once per frame
	void Update()
	{

	}

	private RecipeSO getRandomRecipe()
	{
		if (currentRecipeType == recipeType.Easy)
		{
			return easyRecipeListSO.recipeListSO[UnityEngine.Random.Range(0, easyRecipeListSO.recipeListSO.Count)];
		}
		else if (currentRecipeType == recipeType.Medium)
		{
			return mediumRecipeListSO.recipeListSO[UnityEngine.Random.Range(0, mediumRecipeListSO.recipeListSO.Count)];
		}
		else
		{
			return mediumRecipeListSO.recipeListSO[UnityEngine.Random.Range(0, mediumRecipeListSO.recipeListSO.Count)];
		}
	}
}
