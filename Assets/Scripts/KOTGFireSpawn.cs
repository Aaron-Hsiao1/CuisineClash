using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTGFireSpawn : MonoBehaviour
{
    public GameObject pillarPrefab; // Prefab of the pillar to spawn
    public float moveSpeed = 2f;    // Speed at which the pillar moves upwards
    public float stopHeight = 10f; // Height at which the pillar will be deleted
    public float deleteTime = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SpawnPillar();
        }
    }

    void SpawnPillar()
    {
        // Instantiate the pillar at this GameObject's position (you can adjust this as needed)
        GameObject pillar = Instantiate(pillarPrefab, transform.position, Quaternion.identity);

        // Set the pillar's parent to this GameObject (optional, for organization in the Hierarchy)
        pillar.transform.parent = transform;

        // Start moving the pillar upwards
        StartCoroutine(MovePillar(pillar.transform));
    }

    IEnumerator MovePillar(Transform pillarTransform)
    {
        while (pillarTransform.position.y < stopHeight)
        {
            // Move the pillar upwards
            pillarTransform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }

        // Once it reaches the delete height, destroy the pillar
        //Destroy(pillarTransform.gameObject);
    }
}
