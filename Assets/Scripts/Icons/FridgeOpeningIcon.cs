using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeOpeningIcon : MonoBehaviour
{
    public RectTransform iconRect;  // Reference to the RectTransform of the icon
    public Vector2 startPosition;  // The starting position
    public Vector2 endPosition;    // The target position
    public float slideDuration = 0.2f; // Duration of the slide in seconds

    private float timeElapsed = 0f;
    private bool start;

    // Start is called before the first frame update
    void Start()
    {
        start = false;
        iconRect.anchoredPosition = startPosition;
        StartCoroutine(DelayCode());
    }

    // Update is called once per frame
    void Update()
    {
        if (start)
        {
            if (timeElapsed < slideDuration)
            {
                timeElapsed += Time.deltaTime;
                float t = timeElapsed / slideDuration;
                iconRect.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            }
        }
    }

    public void SetNewSlide(Vector2 newEndPosition)
    {
        startPosition = iconRect.anchoredPosition;  // Current position becomes start
        endPosition = newEndPosition;  // New end position
        timeElapsed = 0f;  // Reset the time
    }

    IEnumerator DelayCode()
    {
        // Wait for 2 seconds
        yield return new WaitForSeconds(5f);
        start = true;
    }
}
