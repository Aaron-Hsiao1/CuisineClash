using Unity.Mathematics;
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



    void Start()
    {
        GameObject splineObject = GameObject.Find("GGTrackSpline");
        if (splineObject == null)
        {
            Debug.LogError("Didn't find the spline object GGTrackSpline");
            return;
        }
        trackSpline = splineObject.GetComponent<SplineContainer>();

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
        float nearestT = GetNearestPoint(position, out float distance);
        return nearestT;
    }

    float GetNearestPoint(Vector3 position, out float distance)
    {
        if (trackSpline == null || trackSpline.Splines.Count == 0)
        {
            Debug.LogError(trackSpline);
            Debug.LogError(trackSpline.Splines.Count);

            Debug.LogError("No valid splines found in trackSpline!");
            distance = 0f;
            return 0f;
        }

        Spline spline = trackSpline.Splines[0]; // Assuming the track follows the first spline
        SplineUtility.GetNearestPoint(spline, position, out float3 nearestPoint, out float t);
        distance = Vector3.Distance(position, (Vector3)nearestPoint);
        return t;
    }
}