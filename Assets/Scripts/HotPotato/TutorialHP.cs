using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro; // Import TextMeshPro namespace
using System.Collections;
using Unity.Netcode;

public class WaitForAllPlayers : NetworkBehaviour
{
    public int totalPlayers = 8;               // Set this to the number of players
    public TextMeshProUGUI playerReadyText;               // Reference to a UI Text element in the scene
    private HashSet<int> playersReady = new HashSet<int>(); // Track players who pressed "R"
    private List<ulong> playerIDs;     // List of player IDs
    void Start()
    {
        UpdatePlayerReadyText();
        // Ensure we assign the potato only on the server
        if (IsServer)
        {
            playerIDs = new List<ulong>(NetworkManager.Singleton.ConnectedClientsIds);
        }
        totalPlayers = playerIDs.Count;
        Debug.Log("Newer Check" + totalPlayers);
        // Initialize the live count display
        
    }

    void Update()
    {
        // Loop through each player and check if they've pressed "R"
        for (int i = 0; i < totalPlayers; i++)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!playersReady.Contains(i))
                {
                    playersReady.Add(i);
                    Debug.Log("Player " + i + " is ready.");
                    UpdatePlayerReadyText();
                }
            }
        }

        // Check if all players are ready
        if (playersReady.Count == totalPlayers)
        {
            Debug.Log("All players are ready. Loading Hot Potato scene.");
            Loader.LoadNetwork(Loader.Scene.LoadIntoHP);
        }
    }

    // Update the player ready count display
    void UpdatePlayerReadyText()
    {
        if (playerReadyText != null)
        {
            playerReadyText.text = "Players Ready: " + (playersReady.Count) + "/" + (totalPlayers + 1);
        }
    }
}
