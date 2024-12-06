using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using System.Collections.Generic;

public class RecipeDisplay : MonoBehaviour
{
    public RecipeManager recipeManager; // Reference to the RecipeManager
    public TextMeshPro recipeText; // Reference to the TextMeshPro component
    public Plate plate; // Reference to the Plate script

    private RecipeManager.Recipe currentRecipe;

    void Start()
    {
        ShowNewRecipe();
    }

    void Update()
    {
        if (plate.IsSandwichReady()) // Check if a sandwich is ready
        {
            List<string> sandwichIngredients = plate.GetSandwichIngredients();

            if (recipeManager.CheckCurrentRecipe(sandwichIngredients))
            {
                Debug.Log("Recipe completed: " + currentRecipe.recipeName);
                
                ShowNewRecipe();
                plate.ClearSandwich(); // Clear the sandwich
            }
        }
    }

    void ShowNewRecipe()
    {
        currentRecipe = recipeManager.GetRandomRecipe();

        if (currentRecipe != null)
        {
            // Sanitize ingredient names for display
            List<string> sanitizedIngredients = SanitizeIngredientNames(currentRecipe.requiredIngredients);

            recipeText.text = $"<b>Recipe:</b> {currentRecipe.recipeName}\n" +
                              string.Join("\n", sanitizedIngredients);
        }
        else
        {
            recipeText.text = "<b>No recipes available!</b>";
        }
    }

    List<string> SanitizeIngredientNames(List<string> ingredients)
    {
        List<string> sanitized = new List<string>();
        foreach (string ingredient in ingredients)
        {
            sanitized.Add(ingredient.Replace("(Clone)", "").Trim());
        }
        return sanitized;
    }
}
