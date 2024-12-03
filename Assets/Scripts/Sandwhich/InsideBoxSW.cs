using UnityEngine;

public class SpeedBoostZone : MonoBehaviour
{
    public BoxCollider cubeBounds;       // Reference to the cube's BoxCollider
    public float boostedSpeed = 10f;     // Speed when outside the cube
    public float normalSpeed = 5f;       // Speed inside the cube
    public string shoppingCartObjectName = "ShoppingCart"; // Name of the shopping cart object in the player prefab

    private PlayerMovement playerMovement; // Reference to the player's movement script
    private GameObject shoppingCart;       // Reference to the shopping cart object

    private void Start()
    {
        // Find the player and cache the PlayerMovement component
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerMovement = player.GetComponent<PlayerMovement>();

            // Find the shopping cart object inside the player
            shoppingCart = player.transform.Find(shoppingCartObjectName)?.gameObject;

            if (shoppingCart != null)
                shoppingCart.SetActive(false);
            else
                Debug.LogWarning($"Shopping cart object '{shoppingCartObjectName}' not found in player prefab.");
        }
        else
        {
            Debug.LogWarning("Player object not found! Make sure the player has the tag 'Player'.");
        }
    }

    private void Update()
    {
        if (playerMovement != null && shoppingCart != null)
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
