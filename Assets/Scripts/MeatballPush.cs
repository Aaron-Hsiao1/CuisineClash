using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatballController : MonoBehaviour
{
    public float pushForce = 75f; // Force to push the other player
    public float pushUpForce = 5f;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.GetComponent<Collider>().CompareTag("Player"))
        {
            Rigidbody rb = collision.gameObject.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                Vector3 pushDirection = (collision.gameObject.transform.position - transform.position).normalized;
                pushDirection.y = 0; 
                rb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
                rb.AddForce(Vector3.up * pushUpForce, ForceMode.Impulse);
            }
        }
    }
}
