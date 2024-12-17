using System.Collections;
using UnityEngine;
public class FryingPan : MonoBehaviour
{
    private GameObject itemOnPan = null;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is a tomato
        if (other.CompareTag("Bacon"))
        {
            PlaceItemOnPan(other.gameObject);
            Debug.Log("Found Bacon");
        }
    }

    public void PlaceItemOnPan(GameObject item)
    {
        if (itemOnPan == null)
        {
            itemOnPan = item;
            // Position and lock the item on the board
            item.transform.position = transform.position;
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
        if (itemOnPan != null)
        {
            Debug.Log("Coroutine Started");
            StartCoroutine(CookBaconWithDelay());
        }
    }

    IEnumerator CookBaconWithDelay()
    {
        if (itemOnPan != null)
        {
            Debug.Log("Cooking started...");
            yield return new WaitForSeconds(3f); // Wait for 5 seconds
            Debug.Log("Cooking complete!");
            CreateCookedBacon();
        }
    }
    void CreateCookedBacon()
    {
        if (itemOnPan != null)
        {
            Debug.Log("Made Meat");
            GameObject CookedMeat = Instantiate(itemOnPan.GetComponent<CookedVersion>().cookedVersionPrefab, transform.position, Quaternion.identity);
            Destroy(itemOnPan);
            itemOnPan = null;
        }
    }
}