using System.Collections.Generic;
using UnityEngine;

public class StackOnBread : MonoBehaviour
{
    public float stackHeight = 0.2f; // Height between stacked items
    private List<GameObject> stackedIngredients = new List<GameObject>();
    private Vector3 basePosition;

    private void Start()
    {
        basePosition = transform.position; // Initial position of the bread
    }

    public void AddIngredient(GameObject ingredient)
{
    // Check if the ingredient object has the Ingredient script attached
    Ingredient ingredientScript = ingredient.GetComponent<Ingredient>();
    if (ingredientScript == null)
    {
        // If the object does not have the Ingredient script, ignore it
        Debug.Log("This object is not a valid ingredient. Ignoring it.");
        return;
    }

    // If the ingredient is already stacked, do nothing
    if (stackedIngredients.Contains(ingredient))
    {
        Debug.Log("Ingredient is already stacked!");
        return;
    }

    Debug.Log($"Adding ingredient: {ingredient.name}");

    // Calculate the stacking position (height adjustment based on the number of stacked ingredients)
    Vector3 stackPosition = basePosition + Vector3.up * (stackHeight * stackedIngredients.Count);
    
    // Set the position and parent of the ingredient
    ingredient.transform.SetParent(transform);
    ingredient.transform.localPosition = new Vector3(0, stackHeight * stackedIngredients.Count, 0);
    ingredient.transform.localRotation = Quaternion.identity;

    // Disable Rigidbody to stop physics interference
    Rigidbody ingredientRb = ingredient.GetComponent<Rigidbody>();
    if (ingredientRb != null)
    {
        ingredientRb.isKinematic = true;
    }

    // Optionally, disable the collider (so it doesn't interfere with stacking)
    Collider ingredientCollider = ingredient.GetComponent<Collider>();
    if (ingredientCollider != null)
    {
        ingredientCollider.enabled = false;
    }

    // Change the tag to prevent re-picking
    ingredient.tag = "PlacedIngredient";

    // Add the ingredient to the stacked ingredients list
    stackedIngredients.Add(ingredient);

    Debug.Log($"Ingredient {ingredient.name} successfully added to stack.");
}


    private void OnTriggerEnter(Collider other)
{
    // Check if the object has the Ingredient script attached
    Ingredient ingredientScript = other.GetComponent<Ingredient>();
    if (ingredientScript != null)
    {
        Debug.Log($"{other.name} is a valid ingredient and will be added to the bread stack.");
        AddIngredient(other.gameObject);
    }
    else
    {
        Debug.Log($"{other.name} is not a valid ingredient, so it will be ignored.");
    }
}
}
