using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Mathematics;
using UnityEngine.Splines;

public class PlayerRecoverySystem : NetworkBehaviour
{
    [Header("Track References")]
    [SerializeField] private SplineContainer trackSpline;

    [Header("Recovery Settings")]
    [SerializeField] private float recoveryHeight = 12f;  // Increased from 10f to 12f
    [SerializeField] private float liftDuration = 0.8f;
    [SerializeField] private float hoverDuration = 0.4f;
    [SerializeField] private float dropDuration = 0.6f;
    [SerializeField] private float dropHeight = 6f;       // Increased from 4f to 6f
    [SerializeField] private AnimationCurve liftCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve dropCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Detection")]
    [SerializeField] private Markers GoUp;

    [Header("Debug")]
    [SerializeField] private bool debugMode = true;

    private bool isRecovering = false;
    private Vector3 recoveryPoint;

    private void Start()
    {
        Debug.Log("Code is working for Go Up");
        // Check if we have a valid spline reference
        if (trackSpline == null)
        {
            Debug.LogError("TrackSpline reference is missing! Please assign it in the inspector.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (debugMode)
        {
            Debug.Log($"Trigger entered by {other.gameObject.name}, Layer: {LayerMask.LayerToName(other.gameObject.layer)}");
        }
        NetworkObject networkObject = other.GetComponent<NetworkObject>();
        if (debugMode)
        {
            Debug.Log($"NetworkObject found: {networkObject != null}");
        }

        // Check if the collider is a Player
        if (networkObject != null && !isRecovering && other.CompareTag("Player"))
        {
            ulong playerId = networkObject.NetworkObjectId;

            if (debugMode)
            {
                Debug.Log($"Starting recovery for player ID: {playerId}");
            }

            StartCoroutine(RecoverPlayer(playerId, other.transform));
        }
    }

    private IEnumerator RecoverPlayer(ulong playerId, Transform playerTransform)
    {
        isRecovering = true;

        if (debugMode)
        {
            Debug.Log($"Recovery started. IsServer: {IsServer}");
        }

        // Safety check outside of try-catch
        if (trackSpline == null)
        {
            Debug.LogError("TrackSpline is null in RecoverPlayer!");
            isRecovering = false;
            yield break;
        }

        // If server, disable player controls
        if (IsServer)
        {
            try
            {
                DisablePlayerControlsServerRpc(playerId);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error disabling player controls: {e.Message}");
            }
        }

        // Find recovery position
        try
        {
            recoveryPoint = FindNearestSplinePoint(playerTransform.position);

            if (debugMode)
            {
                Debug.Log($"Found recovery point: {recoveryPoint}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error finding nearest spline point: {e.Message}");
            isRecovering = false;
            yield break;
        }

        Vector3 recoveryPointWithHeight = new Vector3(recoveryPoint.x, recoveryPoint.y + recoveryHeight, recoveryPoint.z);
        Vector3 startPosition = playerTransform.position;
        Vector3 finalPosition = new Vector3(recoveryPoint.x, recoveryPoint.y + dropHeight, recoveryPoint.z);

        // Handle all the animation and movement
        if (IsServer)
        {
            // Lift up animation - add a curve to the path
            float elapsedTime = 0f;
            while (elapsedTime < liftDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = liftCurve.Evaluate(Mathf.Clamp01(elapsedTime / liftDuration));

                // Create a curved path by adjusting the horizontal position
                float curveAmount = Mathf.Sin(t * Mathf.PI) * 3.5f;  // Increased from 2f to 3.5f for a higher curve
                Vector3 horizontalOffset = Vector3.right * curveAmount;

                // Add an additional upward boost during the middle of the curve
                float verticalBoost = Mathf.Sin(t * Mathf.PI) * 2f;  // Add vertical boost
                Vector3 verticalOffset = Vector3.up * verticalBoost;

                Vector3 newPosition = Vector3.Lerp(startPosition, recoveryPointWithHeight, t) + horizontalOffset + verticalOffset;

                try
                {
                    UpdatePlayerPositionServerRpc(playerId, newPosition);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error updating position: {e.Message}");
                }

                yield return null;
            }

            if (debugMode)
            {
                Debug.Log("Lift complete, starting hover");
            }

            // Hover with a more pronounced bobbing effect
            float hoverStartTime = Time.time;
            while (Time.time < hoverStartTime + hoverDuration)
            {
                float bobAmount = Mathf.Sin((Time.time - hoverStartTime) * 6f) * 0.3f;  // Increased from 5f to 6f for frequency and 0.2f to 0.3f for amplitude
                Vector3 hoverPosition = recoveryPointWithHeight + new Vector3(0, bobAmount, 0);

                try
                {
                    UpdatePlayerPositionServerRpc(playerId, hoverPosition);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error updating hover position: {e.Message}");
                }

                yield return null;
            }

            if (debugMode)
            {
                Debug.Log("Hover complete, starting drop");
            }

            // Drop animation - to elevated position
            elapsedTime = 0f;
            while (elapsedTime < dropDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = dropCurve.Evaluate(Mathf.Clamp01(elapsedTime / dropDuration));

                // Add a small bounce effect during the drop
                float bounceEffect = 0f;
                if (t > 0.5f)
                {
                    bounceEffect = Mathf.Sin((t - 0.5f) * 2f * Mathf.PI * 2f) * 0.5f * (1f - t);
                }

                Vector3 newPosition = Vector3.Lerp(recoveryPointWithHeight, finalPosition, t);
                newPosition.y += bounceEffect; // Add the bounce effect to the y position

                try
                {
                    UpdatePlayerPositionServerRpc(playerId, newPosition);
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error updating position: {e.Message}");
                }

                yield return null;
            }

            // Final position
            try
            {
                UpdatePlayerPositionServerRpc(playerId, finalPosition);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error updating final position: {e.Message}");
            }

            if (debugMode)
            {
                Debug.Log("Drop complete, re-enabling controls");
            }

            // Re-enable player controls
            try
            {
                EnablePlayerControlsServerRpc(playerId);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error enabling player controls: {e.Message}");
            }
        }

        if (debugMode)
        {
            Debug.Log("Recovery sequence complete");
        }

        isRecovering = false;
    }

    // Rest of the code remains unchanged
    [ServerRpc(RequireOwnership = false)]
    private void DisablePlayerControlsServerRpc(ulong playerId)
    {
        try
        {
            // Use TryGetValue for safer dictionary access
            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.SpawnManager != null &&
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
            {
                // Disable player controller
                PlayerController playerController = playerObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.enabled = false;

                    if (debugMode)
                    {
                        Debug.Log($"Disabled controller for player {playerId}");
                    }
                }
                else if (debugMode)
                {
                    Debug.LogWarning($"PlayerController not found on player {playerId}");
                }

                // Broadcast to all clients
                DisablePlayerControlsClientRpc(playerId);
            }
            else if (debugMode)
            {
                Debug.LogWarning($"Player object with ID {playerId} not found in SpawnManager");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in DisablePlayerControlsServerRpc: {e.Message}");
        }
    }

    [ClientRpc]
    private void DisablePlayerControlsClientRpc(ulong playerId)
    {
        try
        {
            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.SpawnManager != null &&
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
            {
                // Handle client-side visual effects
                if (debugMode)
                {
                    Debug.Log($"Client-side: disabled visuals for player {playerId}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in DisablePlayerControlsClientRpc: {e.Message}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerPositionServerRpc(ulong playerId, Vector3 newPosition)
    {
        try
        {
            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.SpawnManager != null &&
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
            {
                // Update position
                playerObject.transform.position = newPosition;

                if (debugMode && Time.frameCount % 30 == 0) // Log only every 30 frames to avoid spam
                {
                    Debug.Log($"Updated position for player {playerId} to {newPosition}");
                }
            }
            else if (debugMode)
            {
                Debug.LogWarning($"Player object with ID {playerId} not found when updating position");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in UpdatePlayerPositionServerRpc: {e.Message}");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void EnablePlayerControlsServerRpc(ulong playerId)
    {
        try
        {
            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.SpawnManager != null &&
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
            {
                // Re-enable player controller
                PlayerController playerController = playerObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.enabled = true;

                    if (debugMode)
                    {
                        Debug.Log($"Re-enabled controller for player {playerId}");
                    }
                }

                // Broadcast to all clients
                EnablePlayerControlsClientRpc(playerId);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in EnablePlayerControlsServerRpc: {e.Message}");
        }
    }

    [ClientRpc]
    private void EnablePlayerControlsClientRpc(ulong playerId)
    {
        try
        {
            if (NetworkManager.Singleton != null &&
                NetworkManager.Singleton.SpawnManager != null &&
                NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(playerId, out NetworkObject playerObject))
            {
                // Handle client-side visual effects
                if (debugMode)
                {
                    Debug.Log($"Client-side: re-enabled visuals for player {playerId}");
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in EnablePlayerControlsClientRpc: {e.Message}");
        }
    }

    private Vector3 FindNearestSplinePoint(Vector3 playerPos)
    {
        // Safety check
        if (trackSpline == null)
        {
            Debug.LogError("TrackSpline is null in FindNearestSplinePoint!");
            return playerPos; // Return original position as fallback
        }

        // The index of the nearest point
        int nearestIndex = 0;
        float minDistance = float.MaxValue;

        // Sample points along the spline to find the closest one
        int sampleCount = 100; // Adjust based on spline complexity

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)(sampleCount - 1);

            Vector3 splinePoint;
            try
            {
                splinePoint = trackSpline.EvaluatePosition(t);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error evaluating spline position at t={t}: {e.Message}");
                continue;
            }

            float distance = Vector3.Distance(playerPos, splinePoint);

            if (distance < minDistance)
            {
                minDistance = distance;
                nearestIndex = i;
            }
        }

        // Get the nearest point
        float normalizedT = nearestIndex / (float)(sampleCount - 1);
        Vector3 nearestPoint = trackSpline.EvaluatePosition(normalizedT);

        if (debugMode)
        {
            Debug.Log($"Nearest spline point found at t={normalizedT}, position={nearestPoint}, distance={minDistance}");
        }

        return nearestPoint;
    }

    // Draw debug gizmos to visualize recovery area
    private void OnDrawGizmos()
    {
        if (!debugMode || trackSpline == null) return;

        // Draw spline sampling points
        int sampleCount = 20; // Fewer samples for visualization
        Gizmos.color = Color.blue;

        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)(sampleCount - 1);
            Vector3 splinePoint;

            try
            {
                splinePoint = trackSpline.EvaluatePosition(t);
                Gizmos.DrawSphere(splinePoint, 0.5f);
            }
            catch (System.Exception)
            {
                // Silently fail in gizmo drawing
            }
        }

        // Draw recovery point if set
        if (isRecovering)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(recoveryPoint, 1f);

            Vector3 recoveryPointWithHeight = new Vector3(recoveryPoint.x, recoveryPoint.y + recoveryHeight, recoveryPoint.z);
            Gizmos.DrawLine(recoveryPoint, recoveryPointWithHeight);
            Gizmos.DrawSphere(recoveryPointWithHeight, 1f);

            Vector3 finalPosition = new Vector3(recoveryPoint.x, recoveryPoint.y + dropHeight, recoveryPoint.z);
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(finalPosition, 1f);
        }
    }
}