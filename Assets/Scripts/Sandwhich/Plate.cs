using System.Collections.Generic;
using UnityEngine;

public class Plate : MonoBehaviour
{
    public RecipeManager recipeManager; // Reference to RecipeManager

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bread"))
        {
            StackOnBread breadStack = other.GetComponent<StackOnBread>();
            if (breadStack != null)
            {
                List<string> ingredients = breadStack.GetStackedIngredients();
                if (recipeManager.CheckSandwich(ingredients, out RecipeManager.Recipe matchedRecipe))
                {
                    Debug.Log($"Sandwich completed! Recipe: {matchedRecipe.recipeName}");
                    Destroy(other.gameObject); // Destroy the sandwich
                }
                else
                {
                    Debug.Log("Sandwich does not match any recipe!");
                }
            }
        }
    }
}
