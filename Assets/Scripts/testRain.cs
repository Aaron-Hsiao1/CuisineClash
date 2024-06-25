using UnityEngine;

public class FallDamage : MonoBehaviour
{
    public int lives = 3;
    public float fallThreshold = 1.5f; // The minimum fall distance to take damage
    public float fallDamage = 1f; // The amount of damage taken from falling
    public float groundCheckDistance = 1.1f; // The distance to check for the ground

    private bool isGrounded;
    private bool wasGrounded;
    private float fallStartHeight;
    private float currentHealth = 3f;
    public Rigidbody rb;
    
    void Update()
    {
        // Check if the player has landed
        if (isGrounded && !wasGrounded)
        {
            float fallDistance = fallStartHeight - transform.position.y;
            Debug.Log("Player landed. Fall distance: " + fallDistance);
            if (fallDistance > fallThreshold)
            {
                TakeDamage(fallDamage);
                Debug.Log("Damage Taken");
            }
        }

        // Check if the player has started falling
        if (!isGrounded && wasGrounded)
        {
            fallStartHeight = transform.position.y;
            Debug.Log("Fall start height: " + fallStartHeight);
        }

        wasGrounded = isGrounded;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Enter: " + collision.gameObject.name);
            isGrounded = true;

            // Delay to ensure the player is actually grounded
            Invoke("CheckGroundStatus", 0.1f);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Exit: " + collision.gameObject.name);
            isGrounded = false;
        }
    }

    void CheckGroundStatus()
    {
        if (isGrounded)
        {
            Debug.Log("Confirmed grounded");
            fallStartHeight = transform.position.y;
        }
    }

    void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            lives--;
            if (lives <= 0)
            {
                // Player is out of lives, handle game over
                Debug.Log("Game Over");
                // Add game over logic here
            }
            else
            {
                // Respawn or reset player
                Debug.Log("Player respawned");
                // Add respawn logic here
                currentHealth = 3f; // Reset health for the new life
            }
        }
        else
        {
            Debug.Log("Player took damage, current health: " + currentHealth);
        }
    }
}