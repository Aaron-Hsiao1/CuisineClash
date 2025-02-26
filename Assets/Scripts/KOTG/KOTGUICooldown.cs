using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTGUICooldown : MonoBehaviour
{
    [SerializeField]
    private Image imageCooldown;

    private bool isCooldown = false;
    private float cooldownTime = 5.0f;
    private float cooldowmTimer = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        textCooldown.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
