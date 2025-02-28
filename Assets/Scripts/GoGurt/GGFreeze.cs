using UnityEngine;
public class GGFreeze : MonoBehaviour
{
    public float fixedRotation = 5;
    public float fixedX = 0;
    void Update()
    {
        Vector3 eulerAngles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(fixedX, fixedRotation, eulerAngles.z);
    }
}