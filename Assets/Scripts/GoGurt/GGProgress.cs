using Unity.Netcode;
using UnityEngine;
using UnityEngine.Splines; 

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
        if (IsHost || IsServer)
        {
            UpdateProgress();
        }
    }

    void UpdateProgress()
    {
        float distanceAlongSpline = GetDistanceAlongSpline(transform.position);
        if (distanceAlongSpline < lapProgress && distanceAlongSpline < 0.1f && lapProgress > 0.9f)
        {
            currentLap++;
        }
        
        lapProgress = distanceAlongSpline;
        float overallProgress = (float)currentLap / totalLaps + lapProgress / totalLaps;
        standing.progress.Value = overallProgress;
    }
    
    float GetDistanceAlongSpline(Vector3 position)
    {
        float nearestT = trackSpline.GetNearestPoint(position, out float distance);
        return nearestT;
    }
}