using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerManager : MonoBehaviour
{
    [SerializeField] private Button button;

    // Start is called before the first frame update
    void Awake()
    {
        button.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.SingleplayerConnectionLobby);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
