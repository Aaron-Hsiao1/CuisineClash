using System.Collections.Generic;
using UnityEngine;

public class StackOnBread : MonoBehaviour
{
    public float stackHeight = 1f; // Height between stacked items
    private List<GameObject> stackedIngredients = new List<GameObject>();
    public Vector3 basePosition = Vector3.zero;

    private void Start()
    {
        basePosition = transform.position; // Initial position of the bread
    }

public void AddIngredient(GameObject ingredient)
{

    Ingredient ingredientScript = ingredient.GetComponent<Ingredient>();
    if (ingredientScript == null)
    {
        return;
    }

    if (stackedIngredients.Contains(ingredient))
    {
        return;
    }

    Vector3 stackPosition;
    if (stackedIngredients.Count == 0)
    {
        stackPosition = basePosition + Vector3.up * 0.1f; // Slight offset above the bread
    }
    else
    {
        stackPosition = basePosition + Vector3.up * (stackHeight * stackedIngredients.Count);
    }

    Debug.Log($"Step 3: Calculated stack position: {stackPosition}");

    // Save the ingredient's original world scale before parenting
    Vector3 originalWorldScale = ingredient.transform.lossyScale;

    // Parent the ingredient to the bread
    ingredient.transform.SetParent(transform, true); // Preserve world position/rotation

    // Adjust local scale to counteract parent's scale
    Vector3 parentScale = transform.lossyScale; // Get the bread's world scale
    ingredient.transform.localScale = new Vector3(
        originalWorldScale.x / parentScale.x,
        originalWorldScale.y / parentScale.y,
        originalWorldScale.z / parentScale.z
    );

    // Set position and rotation
    ingredient.transform.localPosition = new Vector3(0, stackPosition.y, 0);
    ingredient.transform.localRotation = Quaternion.identity;


    Rigidbody ingredientRb = ingredient.GetComponent<Rigidbody>();
    if (ingredientRb != null)
    {
        ingredientRb.isKinematic = true;
    }

    Collider ingredientCollider = ingredient.GetComponent<Collider>();
    if (ingredientCollider != null)
    {
        ingredientCollider.enabled = false;
    }

    Debug.Log("Step 5: Disabled Rigidbody physics and collider.");

    ingredient.tag = "PlacedIngredient";

    stackedIngredients.Add(ingredient);

    Debug.Log($"Step 6: Ingredient {ingredient.name} successfully added to stack. Stack count: {stackedIngredients.Count}");
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
