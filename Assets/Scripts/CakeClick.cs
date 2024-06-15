using UnityEngine;

public class CakeEating : MonoBehaviour
{
    public float eatTime = 5f; // Time required to fully eat the cake
    public float interactionDistance = 1.5f; // Maximum distance for interaction
    private float currentTime = 0f;
    private bool isEating = false;
    private bool isFullyEaten = false;
    private Transform player; // Reference to the player's transform

    void Start()
    {
        // Find the player object using its tag in the parent hierarchy
        GameObject playerObject = gameObject.transform.root.gameObject;
        player = playerObject.GetComponent<Transform>();
    }

    void Update()
    {
        private void OnTriggerStay(Collider other){
        if(other.gameObject.tag == "Player" && !isEating && !isFullyEaten){
            if(Input.GetKey(KeyCode.E)){
                isEating = true;
                currentTime = 0f;
            }
        }
    }
}


        if (isEating)
        {
            currentTime += Time.deltaTime;
            if (currentTime >= eatTime)
            {
                isEating = false;
                isFullyEaten = true;
                EatCake();
            }
        }
    }

    void EatCake()
    {
        // Add code here to destroy or hide the cake object
        Destroy(gameObject);
        Debug.Log("Cake fully eaten!");
    }
}
