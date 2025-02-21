using UnityEngine;
public class GGFreeze : MonoBehaviour
{
    public float fixedRotation = 5;
    void Update()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(eulerAngles.x, fixedRotation, eulerAngles.z);
    }
}