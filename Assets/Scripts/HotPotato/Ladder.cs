using UnityEngine;

public class Ladder : MonoBehaviour
{
    public float climbSpeed = 5f; // Speed of climbing
    public float climbDetectionRadius = 0.5f; // Radius to detect when player exits ladder
    public LayerMask ladderLayer; // Layer assigned to the ladder

    private bool isClimbing = false; // Is the player climbing?
    private Rigidbody rb; // Reference to the Rigidbody component
    private CharacterController characterController; // Reference to the CharacterController component

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        characterController = GetComponent<CharacterController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = true;
            rb.useGravity = false; // Disable gravity when climbing
            rb.velocity = Vector3.zero; // Reset velocity to prevent sliding
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ladder"))
        {
            isClimbing = false;
            rb.useGravity = true; // Re-enable gravity when exiting ladder
        }
    }

    void Update()
    {
        if (isClimbing)
        {
            float verticalInput = Input.GetAxis("Vertical"); // Get vertical input
            float horizontalInput = Input.GetAxis("Horizontal"); // Get horizontal input (optional)

            // Move the player up or down the ladder
            Vector3 climbDirection = new Vector3(0, verticalInput, 0);
            characterController.Move(climbDirection * climbSpeed * Time.deltaTime);
        }
    }
}
