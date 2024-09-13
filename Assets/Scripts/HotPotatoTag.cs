using UnityEngine;

public class HotPotatoTag : MonoBehaviour
{
    public bool hasHotPotato = false; // Whether this player has the hot potato
    public float tagDistance = 2f; // Maximum distance to tag another player
    public GameObject hotPotatoObject; // Reference to the hot potato object

    void Start()
    {
        // Ensure the hot potato is initially hidden
        if (hotPotatoObject != null)
        {
            hotPotatoObject.SetActive(false);
        }
    }

    void Update()
    {
        // Show or hide the hot potato object based on whether the player has the potato
        if (hotPotatoObject != null)
        {
            hotPotatoObject.SetActive(hasHotPotato);
        }

        // Check if the player has the hot potato and presses the "B" key
        if (hasHotPotato && Input.GetKeyDown(KeyCode.B))
        {
            TryTagAnotherPlayer();
        }
    }

    void TryTagAnotherPlayer()
    {
        // Find all players within a certain distance
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, tagDistance);

        foreach (var hitCollider in hitColliders)
        {
            HotPotatoTag otherPlayer = hitCollider.GetComponent<HotPotatoTag>();

            if (otherPlayer != null && otherPlayer != this) // Ensure it's another player
            {
                // Transfer the hot potato
                hasHotPotato = false;
                otherPlayer.hasHotPotato = true;
                Debug.Log($"{gameObject.name} tagged {otherPlayer.gameObject.name}!");

                // End the loop after tagging
                break;
            }
        }
    }
}
