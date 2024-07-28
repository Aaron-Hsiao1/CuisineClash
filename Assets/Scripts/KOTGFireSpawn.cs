using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KOTGFireSpawn : MonoBehaviour
{
    public GameObject pillarPrefab; // Prefab of the pillar to spawn
    public GameObject fireIndicatorPrefab;
    public float moveSpeed = 2f;    // Speed at which the pillar moves upwards
    public float stopHeight = 10f; // Height at which the pillar will be deleted
    private float delay = 1f;
    public float indToSpawnDelay = 3f;
    private Vector3 indSpawn = new Vector3(-14, -52, -1);
    private Vector3 boxSpawn = new Vector3(-14, -132, -1);

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
        GameObject pillar = Instantiate(pillarPrefab, boxSpawn, Quaternion.identity);
        GameObject indicatorPillar = Instantiate(fireIndicatorPrefab, indSpawn, Quaternion.identity);

        // Set the pillar's parent to this GameObject (optional, for organization in the Hierarchy)
        pillar.transform.parent = transform;

        // Start moving the pillar upwards
        StartCoroutine(MovePillar(pillar.transform, indicatorPillar.transform));
    }

    IEnumerator MovePillar(Transform pillarTransform, Transform indicatorTransform)
    {
        yield return new WaitForSeconds(indToSpawnDelay);
        Destroy(indicatorTransform.gameObject);

        while (pillarTransform.position.y < stopHeight)
            {
                // Move the pillar upwards
                pillarTransform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

                // Wait for the next frame
                yield return null;
            }
            yield return new WaitForSeconds(delay);

            Destroy(pillarTransform.gameObject);
    }

}