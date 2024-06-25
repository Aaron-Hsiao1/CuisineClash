using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int lives = 3;
    public float fallThreshold = 10f; // The minimum fall distance to take damage
    public float fallDamage = 1f; // The amount of damage taken from falling
    public float groundCheckDistance = 1.1f; // The distance to check for the ground

    private bool isGrounded;
    private bool wasGrounded;
    private float fallStartHeight;
    private float currentHealth = 3f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CheckGroundStatus();

        if (isGrounded && !wasGrounded)
        {
            // Player has just landed
            float fallDistance = fallStartHeight - transform.position.y;
            if (fallDistance > fallThreshold)
            {
                TakeDamage(fallDamage);
            }
        }

        if (!isGrounded && wasGrounded)
        {
            // Player has just started falling
            fallStartHeight = transform.position.y;
        }

        wasGrounded = isGrounded;
    }

    void CheckGroundStatus()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);
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
