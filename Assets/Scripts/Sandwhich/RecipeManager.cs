using UnityEngine;
using System.Collections.Generic;

public class RecipeManager : MonoBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public string recipeName;
        public List<string> requiredIngredients;
    }

    public List<Recipe> recipes; // List of all recipes
    private Recipe currentRecipe;

    // Get a random recipe
    public Recipe GetRandomRecipe()
    {
        if (recipes.Count == 0) return null;
        int index = Random.Range(0, recipes.Count);
        currentRecipe = recipes[index];
        return currentRecipe;
    }

    public Recipe GetCurrentRecipe()
    {
        return currentRecipe;
    }

    public bool CheckCurrentRecipe(List<string> sandwichIngredients)
    {
        if (currentRecipe == null) return false;
        return IsMatch(sandwichIngredients, currentRecipe.requiredIngredients);
    }

    private bool IsMatch(List<string> sandwichIngredients, List<string> requiredIngredients)
    {
        if (sandwichIngredients.Count != requiredIngredients.Count)
            return false;

        foreach (var ingredient in requiredIngredients)
        {
            if (!sandwichIngredients.Contains(ingredient))
                return false;
        }
        return true;
    }
}
