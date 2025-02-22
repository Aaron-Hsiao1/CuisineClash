using UnityEngine;

public class GGStanding : MonoBehaviour
{
    public string racerName; // Racer's name
    public float progress;   // Progress in the race
    public int currentRank;  // Current rank in the race

    void Update()
    {
        // Example: Update progress based on distance traveled (you should modify this based on your game logic)
        progress = transform.position.z; 
    }
}
