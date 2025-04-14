using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTGBoxFling : MonoBehaviour
{

    public float flingForce = 10f; // The force to apply to fling the object
    public Vector3 direction = Vector3.up; // The direction to fling the object (can be customized)
    public KOTGAttack KOTGA;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (KOTGA.isDashing)
        {
            // Check if the other object has a Rigidbody (this is important to apply force)
            if (other.GetComponent<Rigidbody>() != null)
            {
                Rigidbody rb = other.GetComponentInParent<Rigidbody>();

                // Calculate the direction from the center of the area to the object's position
                Vector3 flingDirection = (other.transform.position - transform.position).normalized;

                // Apply force to fling the object away from the area
                rb.AddForce(flingDirection * flingForce, ForceMode.Impulse);
                Debug.Log("flinging");
            }
        }
    }
}
