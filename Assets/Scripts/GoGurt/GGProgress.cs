using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Splines;

public class GGProgress : NetworkBehaviour
{
    private SplineContainer trackSpline;
    private GGStanding standing;

    [SerializeField] private int totalLaps = 3;

    // Progress tracking variables
    private float lastSplinePosition = 0f;
    private int currentLap = 0;
    private bool hasCompletedFirstPass = false;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Attempt to initialize immediately after spawn
        InitializeTrackData();
    }

    void Start()
    {
        Debug.Log($"GGProgress Start method called - IsOwner: {IsOwner}, IsServer: {IsServer}, IsClient: {IsClient}");
        InitializeTrackData();
    }

    void InitializeTrackData()
    {
        Debug.Log("Attempting to initialize track data...");

        // Try finding spline multiple ways
        trackSpline = GameObject.Find("GGTrackSpline")?.GetComponent<SplineContainer>();

        if (trackSpline == null)
        {
            // Try finding any SplineContainer in the scene
            trackSpline = FindObjectOfType<SplineContainer>();
        }

        if (trackSpline == null)
        {
            Debug.LogError("CRITICAL: No spline container found in the scene!");
            return;
        }

        standing = GetComponent<GGStanding>();

        if (standing == null)
        {
            Debug.LogError($"CRITICAL: GGStanding component missing from player {gameObject.name}!");
            return;
        }
    }

    void Update()
    {
        // Only update on server or host
        if (!IsServer && !IsHost) return;

        try
        {
            UpdateProgress();
        }
        catch (System.Exception e)
        {
        }
    }

    void UpdateProgress()
    {
        if (trackSpline == null)
        {
            InitializeTrackData();
            return;
        }

        if (standing == null)
        {
            return;
        }

        // Ensure we have at least one spline
        if (trackSpline.Splines.Count == 0)
        {
            return;
        }

        // Get current position on spline
        float currentSplinePosition = GetDistanceAlongSpline(transform.position);

        // Debug logging for spline position

        // Detect lap completion
        if (currentSplinePosition < 0.1f && lastSplinePosition > 0.9f)
        {
            currentLap++;
            hasCompletedFirstPass = true;
        }

        // Prevent lap count from exceeding total laps
        currentLap = Mathf.Min(currentLap, totalLaps);

        // Calculate overall progress
        float lapProgress = hasCompletedFirstPass ? currentSplinePosition : 0f;
        float overallProgress = (float)currentLap / totalLaps + lapProgress / totalLaps;

        // Update standing progress
        standing.progress.Value = Mathf.Clamp01(overallProgress);

        // Debug logging for progress

        // Store current position for next frame's comparison
        lastSplinePosition = currentSplinePosition;
    }

    float GetDistanceAlongSpline(Vector3 position)
    {
        return GetNearestPoint(position, out _);
    }

    float GetNearestPoint(Vector3 position, out float distance)
    {
        if (trackSpline == null || trackSpline.Splines.Count == 0)
        {
            distance = 0f;
            return 0f;
        }

        Spline spline = trackSpline.Splines[0];
        SplineUtility.GetNearestPoint(spline, position, out float3 nearestPoint, out float t);

        distance = Vector3.Distance(position, (Vector3)nearestPoint);
        return t;
    }
}