using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using System.Collections;

public class HotPotatoTag : NetworkBehaviour
{
    public bool hasHotPotato = false; // Whether this player has the hot potato
    public float tagDistance = 5f; // Maximum distance to tag another player
    //public GameObject hotPotatoObject; // Reference to the hot potato object

    private HotPotatoManager hotPotatoManager;

    [SerializeField] private bool canTagPlayer;
    private float tagCooldown = 1f;

    private bool isTagging = false;

    void Start()
    {
        canTagPlayer = true;
        
        // Find the game manager in the scene
        hotPotatoManager = FindObjectOfType<HotPotatoManager>();

        // Update visibility based on initial hot potato state
        UpdateHotPotatoVisibility();
    }

    void Update()
    {
        if (!IsLocalPlayer) return;
        // Check if we're in the "HotPotato" scene
        if (SceneManager.GetActiveScene().name != "HotPotato")
        {
            //UpdateHotPotatoVisibility();
        }

        // Check if the player has the hot potato and presses the "R" key
        if (SceneManager.GetActiveScene().name == "HotPotato" && OwnerClientId == hotPotatoManager.currentPlayerWithPotato.Value && canTagPlayer && Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("Key found");
            canTagPlayer = false;
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
        Debug.Log("Player id that called this: " + NetworkManager.Singleton.LocalClientId);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, tagDistance);
        foreach (var hitCollider in hitColliders)
        {
            HotPotatoExplosion otherPlayer = hitCollider.GetComponent<HotPotatoExplosion>();
            Debug.Log("FOREACH LOOP!!");
            //otherPlayer != null && otherPlayer != this && !otherPlayer.HasHotPotato() && canTagPlayer
            if (otherPlayer != null && otherPlayer != this && !otherPlayer.HasHotPotato() && otherPlayer.gameObject.CompareTag("Player"))
            {
                Debug.Log($"{gameObject.name} tagged {otherPlayer.gameObject.name}!");

                hotPotatoManager.TransferHotPotato(hitCollider.gameObject.GetComponentInParent<NetworkObject>().OwnerClientId);

                Debug.Log("new player with potato: " + hotPotatoManager.currentPlayerWithPotato.Value);
                

                Debug.Log("Exiting TryTagAnotherPlayer after tagging.");
                StartCoroutine(CanTagPlayerCooldown());
                return;
            }
        }

        StartCoroutine(CanTagPlayerCooldown());

        Debug.Log("Finished TryTagAnotherPlayer without tagging anyone.");
    }

    private IEnumerator CanTagPlayerCooldown()
    {
        Debug.Log("Reestting player cooldown...");
        Debug.Log("cantagpalyer?: " + canTagPlayer);
        yield return new WaitForSeconds(tagCooldown);
        canTagPlayer = true;
        Debug.Log("cantagplayer after setting true?:" + canTagPlayer);
    }
}
