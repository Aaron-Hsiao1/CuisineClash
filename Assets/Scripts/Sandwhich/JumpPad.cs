using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [Header("Launch Settings")]
    public float upwardForce = 10f;
    public float forwardForce = 5f;

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Rigidbody playerRigidbody = other.GetComponentInParent<Rigidbody>();

            if (playerRigidbody != null)
            {
                Vector3 currentVelocity = playerRigidbody.velocity;
                currentVelocity.y = 0;
                Vector3 launchVelocity = transform.up * upwardForce + transform.forward * forwardForce;
                playerRigidbody.velocity = currentVelocity + launchVelocity;
            }
        }
    }
}
