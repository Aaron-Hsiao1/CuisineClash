using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class FridgeSpawnAnimation : MonoBehaviour
{

    private VisualEffect vfx;
    // Start is called before the first frame update
    void Start()
    {
        vfx = GetComponent<VisualEffect>();
        StopVFX();
    }

    public void PlayVFX()
    {
        if (vfx != null)
        {
            vfx.Play(); // Play the VFX graph
        }
    }

    public void StopVFX()
    {
        if (vfx != null)
        {
            vfx.Stop(); // Stop the VFX graph
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayVFX();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            StopVFX();
        }
    }
}
