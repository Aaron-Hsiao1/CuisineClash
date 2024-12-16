using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FridgeFollow : MonoBehaviour
{
    public Vector3 Position1 = new Vector3(-25.05f, -8.95f, 29.29f);
    public Vector3 Rotation1 = new Vector3(0.396f, 38.18f, -0.372f); 

    public Vector3 Position2 = new Vector3(1.19f, 6.97f, 33.8f);
    public Vector3 Rotation2 = new Vector3(0.396f, -50.6f, -0.372f); 

    public Vector3 Position3 = new Vector3(-13.11364f, 7.366657f, 22.32742f);
    public Vector3 Rotation3 = new Vector3(6.2f, 0f, 0f); 

    public Vector3 Position4 = new Vector3(0f, 0f ,0f);
    public Vector3 Rotation4 = new Vector3(0f, 0f ,0f); 

    public Vector3 Position5 = new Vector3(0f, 0f, 0f);

    private Vector3 velocity = Vector3.zero;
    
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
        yield return new WaitForSeconds(0.2f);
        transform.localPosition = Position2;
        transform.localRotation = Quaternion.Euler(Rotation2);
        yield return new WaitForSeconds(1f);
        transform.localPosition = Position3;
        transform.localRotation = Quaternion.Euler(Rotation3);
        yield return new WaitForSeconds(0.2f);
        transform.localPosition = Position4;
        transform.localRotation = Quaternion.Euler(Rotation4);
        yield return new WaitForSeconds(0.2f);
        transform.localPosition = Vector3.SmoothDamp(transform.position, Position5, ref velocity, Time.deltaTime * 0.001f);
    }
}
