using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class KOTGFling : MonoBehaviour
{
    public float horizontalLaunchForce = 0.001f; // Stronger horizontal push
    public float launchForceUp = 0.001f; // Reduced vertical force
    private bool hit = false;
    public Rigidbody rb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hit)
        {
            rb = other.GetComponentInParent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = GetRandomHorizontalDirection();

                // Apply mostly horizontal force with a slight upward push
                Vector3 totalForce = (randomDirection * horizontalLaunchForce) + (Vector3.up * launchForceUp);
                Debug.Log("hit, starting coroutine");
                //rb.AddForce(totalForce, ForceMode.Impulse); 
#if UNITY_EDITOR
                EditorGUIUtility.PingObject(other);
#endif
                StartCoroutine(LaunchPlayer(other, totalForce, rb));
                

                
            }
        }
    }

    private IEnumerator LaunchPlayer(Collider other, Vector3 totalForce, Rigidbody rb)
    {
        Debug.Log("ddd");
        Debug.Log("total force; " + totalForce);
        other.gameObject.GetComponentInParent<PlayerMovement>().SetLaunching(true);
        Debug.Log(" rb name: " + rb.name);
        rb.velocity = new Vector3(0, 0, 0);
        rb.AddForce(totalForce, ForceMode.Impulse);
        hit = true;
        Debug.Log("force added");
        yield return new WaitForSeconds(0.5f);
        other.gameObject.GetComponentInParent<PlayerMovement>().SetLaunching(false);
    }

    Vector3 GetRandomHorizontalDirection()
    {
        Vector2 randomXZ = Random.insideUnitCircle.normalized; // Get a random 2D direction
        return new Vector3(randomXZ.x, 0, randomXZ.y); // Convert to 3D
    }
}
