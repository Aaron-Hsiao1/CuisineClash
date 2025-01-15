using UnityEngine;
using UnityEngine.SceneManagement;

public class HandleMovementChanges : MonoBehaviour
{
    private PlayerMovement playerMovement; // For normal movement
    private GoKartMovement goKartMovement; // For go-kart movement

    private void Awake()
    {
        // Get references to both movement scripts
        playerMovement = GetComponent<PlayerMovement>();
        goKartMovement = GetComponent<GoKartMovement>();
    }

    private void Start()
    {
        UpdateMovementScript(); // Set the correct script at the start
    }

    private void OnEnable()
    {
        // Subscribe to scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // Unsubscribe from scene changes
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateMovementScript(); // Update the script when the scene changes
    }

    private void UpdateMovementScript()
    {
        // Get the active scene's name
        string currentScene = SceneManager.GetActiveScene().name;

        // Enable/Disable movement scripts based on the scene
        if (currentScene == "GoGurt")
        {
            playerMovement.enabled = false;
            goKartMovement.enabled = true;
        }
        else
        {
            playerMovement.enabled = true;
            goKartMovement.enabled = false;
        }
    }
}
