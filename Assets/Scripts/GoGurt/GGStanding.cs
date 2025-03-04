using Unity.Netcode;
using UnityEngine;

public class GGStanding : NetworkBehaviour
{
    // Initialize with a default value to prevent null issues
    public NetworkVariable<float> progress = new NetworkVariable<float>(0f);
    public int currentRank = 0;

    public override void OnNetworkSpawn()
    {
        // Ensure progress is initialized
        if (IsServer)
        {
            progress.Value = 0f;
        }
    }
}