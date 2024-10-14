using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PregameLobbySingleplayerManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    [SerializeField] private GameObject readyArea;
    
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(player, new Vector3(16, 10, 7), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ready Area"))
        {
            Loader.Load(Loader.Scene.RainingMeatball);
        }
    }
}
