using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndedUI : MonoBehaviour
{
    [SerializeField] Button mainMenuButton;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            CuisineClashLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenu);
        });
    }
}
