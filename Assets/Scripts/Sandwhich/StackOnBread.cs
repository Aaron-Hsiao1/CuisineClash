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
    
    ingredient.tag = "PlacedIngredient";
    ingredient.layer = LayerMask.NameToLayer("Ignore Raycast");

    stackedIngredients.Add(ingredient);
}


    private void OnTriggerEnter(Collider other)
{
    // Check if the object has the Ingredient script attached
    Ingredient ingredientScript = other.GetComponent<Ingredient>();
    if (ingredientScript != null)
    {
        AddIngredient(other.gameObject);
    }

}
    public List<string> GetStackedIngredients()
    {
        List<string> ingredientNames = new List<string>();
        foreach (var ingredient in stackedIngredients)
        {
            ingredientNames.Add(ingredient.name);
        }
        return ingredientNames;
        
    }
}
