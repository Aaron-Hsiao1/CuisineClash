using UnityEngine;

public class SpeedBoostZone : MonoBehaviour
{
    public BoxCollider cubeBounds;       // Reference to the cube's BoxCollider
    public float boostedSpeed = 10f;     // Speed when outside the cube
    public float normalSpeed = 5f;       // Speed inside the cube

    private PlayerMovement playerMovement; // Reference to the player's movement script
    private GameObject shoppingCart;       // Reference to the shopping cart object

    private void Start()
    {
        // Find the player and cache the PlayerMovement component
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();
        }
    }

    private void Update()
    {
        if (playerMovement != null)
        {
            HandleSpeedBoostAndCart();
        }
    }

    private void HandleSpeedBoostAndCart()
    {
        // Check if the player is outside the box
        bool isOutsideCube = !cubeBounds.bounds.Contains(playerMovement.transform.position);

        // Adjust speed
        playerMovement.SetMoveSpeed(isOutsideCube ? boostedSpeed : normalSpeed);

        // Enable or disable shopping cart based on position
        shoppingCart.SetActive(isOutsideCube);
    }
}
