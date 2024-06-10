using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatballController : MonoBehaviour
{
    public float pushForce = 10f; // Force to push the other player
    public float pushUpForce = 10f;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player Hit");
            Rigidbody rb = collision.gameObject.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                Debug.Log("Body Found");
                Vector3 pushDirection = (collision.gameObject.transform.position - transform.position).normalized;
                pushDirection.y = 0; 
                rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                rb.AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
            }
        }
    }
}
