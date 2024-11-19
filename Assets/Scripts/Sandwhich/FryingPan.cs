using UnityEngine;
using System.Collections;
public class FryingPan : MonoBehaviour
{
    private GameObject itemOnPan = null;
    
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is a tomato
        if (other.CompareTag("Bacon"))
        {
            PlaceItemOnPan(other.gameObject);
        }
    }

    public void PlaceItemOnPan(GameObject item)
    {
        if (itemOnPan == null)
        {
            itemOnPan = item;

            // Position and lock the item on the board
            item.transform.position = transform.position + new Vector3(0, 1f, 0);
            item.transform.rotation = Quaternion.identity;
            item.transform.SetParent(transform);

            Rigidbody itemRb = item.GetComponent<Rigidbody>();
            if (itemRb != null)
            {
                itemRb.isKinematic = true;
            }
        }
    }

    void Update()
    {
        // Handle cooking if an item is on the pan and the player clicks
        if (itemOnPan != null && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(CookBaconWithDelay());
        }
    }

    IEnumerator CookBaconWithDelay()
    {
        if (itemOnPan != null)
        {
            Debug.Log("Cooking started...");
            yield return new WaitForSeconds(5f); // Wait for 5 seconds
            Debug.Log("Cooking complete!");
            CreateCookedBacon();
        }
    }
    void CreateCookedBacon()
    {
        if (itemOnPan != null)
        {
            // Replace the tomato with sliced tomato
            GameObject bacon = Instantiate(itemOnPan.GetComponent<CookedVersion>().cookedVersionPrefab, transform.position, Quaternion.identity);
            Destroy(itemOnPan);
            itemOnPan = null;
        }
    }
}
