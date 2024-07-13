using UnityEngine;

public class CakeEating : MonoBehaviour
{
    public float eatTime = 5f; // Time required to fully eat the cake
    public float interactionDistance = 1.5f; // Maximum distance for interaction
    private float currentTime = 0f;
    private bool isEating = false;
    private bool isFullyEaten = false;

    public float raycastDistance = 10f; // Distance for the raycast

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            // Create a ray from the main camera to the mouse position
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit; // Variable to store information about what the raycast hits
            // Perform the raycast
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                // Check if the hit object is another player
                if (hit.collider.CompareTag("Cake") && !isEating && !isFullyEaten)
                {
                    isEating = true;
                    currentTime = 0f;
                    // Call EatCake and pass the hit object as an argument
                    if(isEating){
                        currentTime += Time.deltaTime;
                        if (currentTime>= eatTime){
                            isEating = false;
                            isFullyEaten = true;
                            EatCake(hit.collider.gameObject);
                        }
                    }
                    
                }
            }
        }


    void EatCake(GameObject cake)
    {
        // Destroy the cake object that was hit by the raycast
        Destroy(cake);
        Debug.Log("Cake fully eaten!");
    }
}
}