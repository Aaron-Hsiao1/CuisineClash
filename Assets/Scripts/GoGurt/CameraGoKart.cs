using System.Collections;
using UnityEngine;

public class ChaseCamera : MonoBehaviour
{
    public Transform target; // The target to follow
    public float distance = 5f; // The distance from the target to the camera
    public float height = 3f; // The height of the camera above the target
    public float rotationDamping = 5f; // The damping of the camera rotation

    private Vector3 offset; // The offset from the target to the camera

    private void Start()
    {
        offset = new Vector3(0f, height, -distance);
    }

    private void LateUpdate()
    {
        // Calculate the desired position of the camera
        Vector3 desiredPosition = target.position + target.rotation * offset;

        // Smoothly interpolate the camera's position towards the desired position
        transform.position = Vector3.Lerp(transform.position, desiredPosition, Time.deltaTime * rotationDamping);

        // Rotate the camera to face the target
        transform.LookAt(target);
    }
}