using Unity.Netcode;
using UnityEngine;
using UnityEngine.Splines; // Requires Unity's Spline package

public class GGProgress : NetworkBehaviour
{
    [SerializeField] private SplineContainer trackSpline;
    private GGStanding standing;
    private float totalSplineLength;
    private float lapProgress = 0f;
    private int currentLap = 0;
    [SerializeField] private int totalLaps = 3;

    [SerializeField] private string trackSplineName = "GGTrackSpline";


    void Start()
    {
        GameObject splineObject = GameObject.Find(trackSplineName);
        if (splineObject == null)
        {
            Debug.Log("Didn't find the spline object" + trackSplineName);
            return;
        }
        
        standing = GetComponent<GGStanding>();
        if (standing == null)
        {
            Debug.LogError("GGStanding component missing from player!");
            return;
        }
        
        totalSplineLength = trackSpline.CalculateLength();
    }

    void Update()
    {
        if (IsOwner) // Only the owner updates their own progress
        {
            UpdateProgress();
        }
    }

    void UpdateProgress()
    {
        // Find closest point on spline to player position
        float distanceAlongSpline = GetDistanceAlongSpline(transform.position);
        
        // Handle lap completion
        if (distanceAlongSpline < lapProgress && distanceAlongSpline < 0.1f && lapProgress > 0.9f)
        {
            currentLap++;
        }
        
        lapProgress = distanceAlongSpline;
        
        // Calculate overall progress (completed laps + progress in current lap)
        float overallProgress = (float)currentLap / totalLaps + lapProgress / totalLaps;
        
        // Update NetworkVariable (will sync automatically)
        standing.progress.Value = overallProgress;
    }
    
    float GetDistanceAlongSpline(Vector3 position)
    {
        // Project position onto spline to find closest point
        float nearestT = trackSpline.GetNearestPoint(position, out float distance);
        
        // Calculate distance along spline (0 to 1)
        return nearestT;
    }
}