using UnityEngine;

public class BreadScriptC : MonoBehaviour
{
    public bool hasCondiment = false;
    public string appliedCondiment;
    public GameObject ketchupLayer; // Reference to the ketchup layer object
    private bool isPlayerNear = false; // To check if player is near the bread

    // This will be triggered when the player enters the bread's collider
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Check if the player entered the trigger area
        {
            isPlayerNear = true;
        }
    }

    // This will be triggered when the player exits the bread's collider
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

    public void AddCondiment(string condimentType)
{
    if (isPlayerNear && !hasCondiment) // Check if player is near and bread doesn't already have a condiment
    {
        hasCondiment = true; // Set to true because condiment is now applied
        appliedCondiment = condimentType;
        Debug.Log($"{condimentType} added to the bread!");

        if (condimentType == "Ketchup" && ketchupLayer != null)
        {
            ketchupLayer.SetActive(true); // Turn on the ketchup layer
        }
    }
}
}
