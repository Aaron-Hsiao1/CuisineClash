using UnityEngine;
using TMPro; // Import TextMeshPro namespace
using System.Collections;
using System.Collections.Generic;

public class RecipeDisplay : MonoBehaviour
{
    public RecipeManager recipeManager; 
    public TextMeshPro recipeText;
    public TextMeshProUGUI congratsText; 
    public Plate plate;
    public TextMeshProUGUI failedText;

    private bool isMessageActive = false; 
    private bool isSandwichProcessed = false;
    private RecipeManager.Recipe currentRecipe;

    void Start()
    {
        congratsText.text = ""; 
        congratsText.gameObject.SetActive(false);
        failedText.text = ""; 
        failedText.gameObject.SetActive(false); 
        ShowNewRecipe();
    }

    void Update()
    {
        if (plate.IsSandwichReady() && !isSandwichProcessed && !isMessageActive) 
        {
            List<string> sandwichIngredients = plate.GetSandwichIngredients();

            if (recipeManager.CheckCurrentRecipe(sandwichIngredients))
            {
                Debug.Log("Recipe completed: " + currentRecipe.recipeName);
                isSandwichProcessed = true; 
                StartCoroutine(ShowCongratsMessage("Congrats! Recipe completed: " + currentRecipe.recipeName));
                ShowNewRecipe();
                plate.ClearSandwich();
            }
            else
            {
                isSandwichProcessed = true; 
                StartCoroutine(ShowFailedMessage("Sorry! You got it WRONG!!!"));
            }
        }
        else if (!plate.IsSandwichReady()) 
        {
            isSandwichProcessed = false;
        }
    }

    void ShowNewRecipe()
    {
        currentRecipe = recipeManager.GetRandomRecipe();

        if (currentRecipe != null)
        {
            List<string> sanitizedIngredients = SanitizeIngredientNames(currentRecipe.requiredIngredients);

            string numberedIngredients = "";
            for (int i = 0; i < sanitizedIngredients.Count; i++)
            {
                numberedIngredients += $"{i + 1}.) {sanitizedIngredients[i]}\n";
            }

            recipeText.text = $"<b>Recipe:</b> {currentRecipe.recipeName}\n" + numberedIngredients;
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

    IEnumerator ShowCongratsMessage(string message)
    {
        isMessageActive = true; 
        congratsText.text = message; 
        congratsText.gameObject.SetActive(true); 

        yield return new WaitForSeconds(3f); 

        congratsText.text = ""; 
        congratsText.gameObject.SetActive(false);
        isMessageActive = false; 
    }

    IEnumerator ShowFailedMessage(string message)
    {
        isMessageActive = true; 
        failedText.text = message; 
        failedText.gameObject.SetActive(true); 

        yield return new WaitForSeconds(3f); 

        failedText.text = ""; 
        failedText.gameObject.SetActive(false); 
        isMessageActive = false;
    }
}
