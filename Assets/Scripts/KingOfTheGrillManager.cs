using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KingOfTheGrillManager : NetworkBehaviour
{
    [SerializeField] private GameObject pillarPrefab; // Prefab of the pillar to spawn
    [SerializeField] private GameObject fireIndicatorPrefab;
    [SerializeField] private float moveSpeed = 1000f;    // Speed at which the pillar moves upwards
    [SerializeField] private float stopHeight = 10f; // Height at which the pillar will be deleted
    [SerializeField] private float indToSpawnDelay = 3f;


    private Vector3 indSpawn = new Vector3(-14, -52, -1);
    private Vector3 boxSpawn = new Vector3(-14, -132, -1);
    private float delay = 2f;

    private float spawnDelay = 2f;
    private float elapsedTime;

    private void Start()
    {
        elapsedTime = 0f;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsHost)
        {
            return;
        }
        SpawnPillarClientRpc();
        Debug.Log("on network spawn");
    }

    IEnumerator InstantiatePillar()
    {
        Vector3 pillarSpawnPoint = GetPillarSpawnPoint();

        GameObject indicatorPillar = Instantiate(fireIndicatorPrefab, pillarSpawnPoint, Quaternion.identity);
        var indicatorPillarNetworkObject = indicatorPillar.GetComponent<NetworkObject>();
        indicatorPillarNetworkObject.Spawn(true);

        yield return new WaitForSeconds(1);

        GameObject pillar = Instantiate(pillarPrefab, pillarSpawnPoint, Quaternion.identity);
        var pillarNetworkObject = pillar.GetComponent<NetworkObject>();
        pillarNetworkObject.Spawn(true);

        indicatorPillarNetworkObject.Despawn(true);
        Destroy(indicatorPillar);

        pillar.transform.parent = transform;

        // Start moving the pillar upwards
        StartCoroutine(MovePillar(pillar.transform, indicatorPillar.transform));


    }

    IEnumerator MovePillar(Transform pillarTransform, Transform indicatorTransform)
    {
        //yield return new WaitForSeconds(indToSpawnDelay);
        //Destroy(indicatorTransform.gameObject);
        //indicatorTransform.gameObject.GetComponent<NetworkObject>().Despawn(true); 

        while (pillarTransform.position.y < stopHeight)
        {
            // Move the pillar upwards
            pillarTransform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            // Wait for the next frame
            yield return null;
        }

        yield return new WaitForSeconds(delay);

        Destroy(pillarTransform.gameObject);
        pillarTransform.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

    IEnumerator SpawnPillarLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnDelay);

            StartCoroutine(InstantiatePillar());

            elapsedTime += spawnDelay; //adds time to the elapsed time

            if (elapsedTime >= spawnDelay) // if the elapsed time is greater than the time needed to speed up, spawn cd is halved, and elapsed time resets
            {
                elapsedTime = 0f;
            }
        }
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

    [ClientRpc]
    private void SpawnPillarClientRpc()
    {
        StartCoroutine(SpawnPillarLoop());
        Debug.Log("spawn pillar client rpc");
    }

    [ServerRpc]
    private void SpawnPillarServerRpc()
    {
        SpawnPillarClientRpc();
        Debug.Log("spawn pillar server rpc");
    }
}
