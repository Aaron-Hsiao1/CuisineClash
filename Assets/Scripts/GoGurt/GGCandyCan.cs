using UnityEngine;

public class CandyCan : MonoBehaviour
{
    [Header("Pendulum Settings")]
    [Tooltip("Maximum angle (in degrees) from the vertical position.")]
    public float swingAngle = 30.0f;
    [Tooltip("Speed of the pendulum swing.")]
    public float speed = 2.0f;

    [Header("Push Settings")]
    [Tooltip("Force applied to the player on collision.")]
    public float pushForce = 1000000.0f;
    private float startTime;

    void Start()
    {
        startTime = Time.time;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true;
    }

    void Update()
    {
        float angle = swingAngle * Mathf.Sin((Time.time - startTime) * speed);
        if (angle < 2)
        {
            transform.localRotation = Quaternion.Euler(angle, -160, 90);
    
                }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody playerRb = collision.gameObject.GetComponentInParent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = collision.contacts[0].normal * -1;
                Debug.DrawRay(collision.contacts[0].point, pushDirection * 2, Color.red, 2.0f);
                float adjustedPushForce = Mathf.Max(pushForce, 1.0f);
                playerRb.AddForce(pushDirection * adjustedPushForce, ForceMode.Impulse);

                Debug.Log($"Push applied: {pushDirection}, Force: {adjustedPushForce}");
            }
        }
    }
}