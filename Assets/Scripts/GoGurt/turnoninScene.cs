using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TurnOnInScene : MonoBehaviour
{
    public GameObject goKart;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "GoGurt")
        {
            goKart.SetActive(true);
        }
        else
        {
            goKart.SetActive(false);
        }
    }
}