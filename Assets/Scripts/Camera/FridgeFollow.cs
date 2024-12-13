using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeFollow : MonoBehaviour
{
    public Vector3 Position1 = new Vector3(-25.05f, -8.95f, 29.29f);
    public Vector3 Rotation1 = new Vector3(0.396f, 38.18f, -0.372f); 

    public Vector3 Position2 = new Vector3(-5.95f, 7.366657f, 27.3f);
    public Vector3 Rotation2 = new Vector3(6.2f, -27.365f, 0f); 

    public Vector3 Position3 = new Vector3(-13.11364f, 7.366657f, 22.32742f);
    public Vector3 Rotation3 = new Vector3(6.2f, 0f, 0f); 
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CameraAnimation());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator CameraAnimation()
    {
        transform.localPosition = Position1;
        transform.localRotation = Quaternion.Euler(Rotation1);
        yield return new WaitForSeconds(1f);
        transform.localPosition = Position2;
        transform.localRotation = Quaternion.Euler(Rotation2);
    }
}
