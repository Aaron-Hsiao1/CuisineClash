using System;
using UnityEngine;

public class Trashbin : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bread"))
        {
            CheckifPressed(other.gameObject);
            Debug.Log("triggered");
        }
    }
    private void CheckifPressed(GameObject item)
    {
        Debug.Log("CheckIfpressed");
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("destroy bread");
            Destroy(item.gameObject);
        }

    }
}
