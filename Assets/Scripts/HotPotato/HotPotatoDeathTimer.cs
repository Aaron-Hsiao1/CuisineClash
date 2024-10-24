using UnityEngine;

public class HotPotatoDeathTimer : MonoBehaviour
{
    public GameObject hotPotato; // Reference to the hot potato child object

    // Method to check if the player has the active hot potato
    public bool HasHotPotato()
    {
        return hotPotato != null && hotPotato.activeSelf; // Return true if hot potato is active
    }

    // Method to handle player elimination
    public void Eliminate()
    {
        Debug.Log($"{name} has been eliminated!"); // Log elimination to console

        // Example: Disable the player game object or perform other actions
        gameObject.SetActive(false); // Disable the player object
    }
}