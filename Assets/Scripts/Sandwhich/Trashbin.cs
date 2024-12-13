using UnityEngine;

public class Trashbin : MonoBehaviour
{
    private GameObject itemToDestroy;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Bread"))
        {
            itemToDestroy = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Bread"))
        {
            if (itemToDestroy == other.gameObject)
            {
                itemToDestroy = null;
            }
        }
    }

    private void Update()
    {
        if (itemToDestroy != null && Input.GetMouseButtonDown(0))
        {
            Destroy(itemToDestroy);
            itemToDestroy = null; 
        }
    }
}
