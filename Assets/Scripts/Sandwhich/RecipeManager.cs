using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [System.Serializable]
    public class Recipe
    {
        public string recipeName;
        public List<string> requiredIngredients;
    }

    public List<Recipe> recipes; // List of available recipes

    public bool CheckSandwich(List<string> sandwichIngredients, out Recipe matchedRecipe)
    {
        foreach (var recipe in recipes)
        {
            if (IsMatch(sandwichIngredients, recipe.requiredIngredients))
            {
                matchedRecipe = recipe;
                return true;
            }
        }
        matchedRecipe = null;
        return false;
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
