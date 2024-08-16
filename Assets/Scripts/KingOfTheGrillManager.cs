using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingOfTheGrillManager : MonoBehaviour
{
    [SerializeField] private GameObject pillarPrefab; // Prefab of the pillar to spawn
    [SerializeField] private GameObject fireIndicatorPrefab;
    [SerializeField] private float moveSpeed = 2f;    // Speed at which the pillar moves upwards
    [SerializeField] private float stopHeight = 10f; // Height at which the pillar will be deleted
    [SerializeField] private float indToSpawnDelay = 3f;


    private Vector3 indSpawn = new Vector3(-14, -52, -1);
    private Vector3 boxSpawn = new Vector3(-14, -132, -1);
    private float delay = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void SpawnPillar()
    {
        // Instantiate the pillar at this GameObject's position (you can adjust this as needed)
        Vector3 pillarSpawnPoint = GetPillarSpawnPoint();

        GameObject pillar = Instantiate(pillarPrefab, pillarSpawnPoint, Quaternion.identity);
        GameObject indicatorPillar = Instantiate(fireIndicatorPrefab, pillarSpawnPoint, Quaternion.identity);

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

    private Vector3 GetPillarSpawnPoint()
    {
        float _radius = 90;
        float _grillY = 115;


        //center: -12.7, Y, -114.8
        Vector3 _centerPoint = new Vector3(-12.7f, 0, -114.8f);
        //57.2, -81.9

        //77.25 radius

        //Vector2 randomPoint = centerPoint + Random.insideUnitCircle * radius * 0.5f;

        Vector3 randomSpawnPoint = _centerPoint += _radius * 0.5f * Random.insideUnitSphere;
        randomSpawnPoint.y = _grillY;
        return randomSpawnPoint;
    }
}
