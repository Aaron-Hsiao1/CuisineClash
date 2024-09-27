using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MeatballController : MonoBehaviour
{
    public float pushForce = 75f; // Force to push the other player
    public float pushUpForce = 5f;
    public Rigidbody rb;
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Meatball"))
        {
            //Rigidbody rb = collision.gameObject.GetComponentInParent<Rigidbody>();

            if (rb != null)
            {

                Destroy(gameObject);

            }
        }
    }
}
