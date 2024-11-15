using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class HotPotatoTag : NetworkBehaviour
{
    public bool hasHotPotato = false; // Whether this player has the hot potato
    public float tagDistance = 2f; // Maximum distance to tag another player
    //public GameObject hotPotatoObject; // Reference to the hot potato object

    private GiveToRandomHP gameManager;

    void Start()
    {
        // Find the game manager in the scene
        gameManager = FindObjectOfType<GiveToRandomHP>();

        // Update visibility based on initial hot potato state
        UpdateHotPotatoVisibility();
    }

    void Update()
    {
        // Check if we're in the "HotPotato" scene
        if (SceneManager.GetActiveScene().name != "HotPotato")
        {
            //hasHotPotato = false;
            //UpdateHotPotatoVisibility();
        }

        // Check if the player has the hot potato and presses the "B" key
        if (hasHotPotato && Input.GetKeyDown(KeyCode.B))
        {
            TryTagAnotherPlayer();
        }
    }

    // Update visibility of the hot potato object based on hasHotPotato status
    public void UpdateHotPotatoVisibility()
    {
        //if (hotPotatoObject != null)
        //{
            //hotPotatoObject.SetActive(hasHotPotato);
        //}
    }

    // Attempt to tag another player within range
    void TryTagAnotherPlayer()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, tagDistance);

        foreach (var hitCollider in hitColliders)
        {
            HotPotatoTag otherPlayer = hitCollider.GetComponent<HotPotatoTag>();

            if (otherPlayer != null && otherPlayer != this && !otherPlayer.hasHotPotato)
            {
                // Transfer the hot potato
                hasHotPotato = false;
                otherPlayer.hasHotPotato = true;
                UpdateHotPotatoVisibility();
                otherPlayer.UpdateHotPotatoVisibility();

                Debug.Log($"{gameObject.name} tagged {otherPlayer.gameObject.name}!");

                // Notify the game manager to sync this change
                if (IsServer)
                {
                    gameManager.SetHotPotatoClientRpc(otherPlayer.OwnerClientId);
                }

                break;
            }
        }
    }
}
