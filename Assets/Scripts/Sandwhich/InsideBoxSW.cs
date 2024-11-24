using UnityEngine;

public class SpeedBoostZone : MonoBehaviour
{
    public BoxCollider cubeBounds;   // Reference to the cube's BoxCollider
    public float boostedSpeed;// Speed when outside the cube
    public float normalSpeed = 5f;   // Speed inside the cube

    private void Update()
    {
        CheckPlayerPosition();
    }

    private void CheckPlayerPosition()
    {
        // Find the player object with the tag "Player"
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Access the PlayerMovement component on the player
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            if (playerMovement != null)
            {
                // Check if the player's position is outside the cube's bounds
                bool isOutsideCube = !IsInsideCube(player.transform.position);

                // Set player speed based on position
                playerMovement.SetMoveSpeed(isOutsideCube ? boostedSpeed : normalSpeed);
            }
        }
    }

    // Helper function to check if a position is inside the cube bounds
    private bool IsInsideCube(Vector3 position)
    {
        // Check if the position is within the bounds of the cube
        return cubeBounds.bounds.Contains(position);
    }
}
