using UnityEngine;

public class Player : MonoBehaviour
{
    public float eatAmount = 10f; // Amount of HP to reduce when eating the cake
    private Cake cake; // Reference to the cake

    private void Update()
    {
        // Check for interaction with the cake
        if (Input.GetKeyDown(KeyCode.B) && cake != null)
        {
            cake.EatCake(eatAmount);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player is near the cake
        if (other.CompareTag("Cake"))
        {
            cake = other.GetComponent<Cake>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Clear reference when exiting the trigger
        if (other.CompareTag("Cake"))
        {
            cake = null;
        }
    }
}