using System.Collections;
using UnityEngine;

public class PastaSauceRise : MonoBehaviour
{
    public float riseSpeed = 0.1f;   // Speed at which the sauce rises
    public float riseInterval = 1.0f;  // Time interval (in seconds) between rises

    private Vector3 initialPosition;

    void Start()
    {
        initialPosition = transform.position;  // Store the initial position of the sauce
        StartCoroutine(RiseEveryFewSeconds());
    }

    // Coroutine to make the sauce rise at intervals
    IEnumerator RiseEveryFewSeconds()
    {
        while (true)
        {
            yield return new WaitForSeconds(riseInterval);  // Wait for the specified interval
            Rise();
        }
    }

    // Method to raise the sauce's Y position
    void Rise()
    {
        Vector3 newPosition = transform.position;
        newPosition.y += riseSpeed;  // Increase the Y position by the riseSpeed
        transform.position = newPosition;
    }
}
