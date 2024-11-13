using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoaderHP : MonoBehaviour
{
    public string targetScene = "HotPotato"; // Name of the target scene to load
    public float delayBeforeLoad = 3f; // Optional delay before loading the scene

    void Start()
    {
        StartCoroutine(LoadTargetScene());
    }

    IEnumerator LoadTargetScene()
    {
        yield return new WaitForSeconds(delayBeforeLoad); // Optional delay for loading screen

        Loader.LoadNetwork(Loader.Scene.HotPotato);
    }
}
