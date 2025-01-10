using System.Collections;
using UnityEngine;
using static UnityEditor.Progress;

public class Jump2Kitchen : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(JumpToTarget(other.transform.parent));
        }
    }

    private IEnumerator JumpToTarget(Transform playerTransform)
    {
        if (targetObject == null)
        {
            yield break;
        }

        Vector3 startPosition = playerTransform.position;
        Vector3 targetPosition = targetObject.transform.position;
        targetPosition.y += 5f;

        float distance = Vector3.Distance(startPosition, targetPosition);
        float movementSpeed = 50f;
        float duration = distance / movementSpeed;

        for (float t = 0; t < 1; t += Time.deltaTime / duration)
        {
            playerTransform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        playerTransform.position = targetPosition;

    }
}

