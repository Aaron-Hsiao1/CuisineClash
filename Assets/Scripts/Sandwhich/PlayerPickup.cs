using System;
using UnityEngine;

public class PlayerPickup : MonoBehaviour
{
    public float raycastRange = 100f; // Range for the raycast to detect items
    private GameObject heldItem = null; // Reference to the currently held item


void Update()
{
    // Check for right mouse button click
    if (Input.GetMouseButtonDown(1))
    {
        // If holding an item, drop it
        if (heldItem != null)
        {
            DropItem();
        }
        else
        {
            // Perform a raycast from the camera in the forward direction to pick up an item
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Draw the raycast line in the Scene view for debugging
            Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastRange, Color.green, 0.1f);

            if (Physics.Raycast(ray, out hit, raycastRange))
            {
                // Check if the hit object is a grocery item and player isn't holding anything
                if ((hit.collider.CompareTag("CanPickUp") || hit.collider.CompareTag("Tomato") || hit.collider.CompareTag("Bacon") || hit.collider.CompareTag("Bread")))
                {
                   PickupItem(hit.collider.gameObject);
                }
            }

        }
    }
    if (Input.GetKeyDown(KeyCode.B) && heldItem != null){
        TryUseCondiment();
        Debug.Log("trying to use condiment");
    }
}

void PickupItem(GameObject item)
{
    heldItem = item;

    Rigidbody itemRb = item.GetComponent<Rigidbody>();
    if (itemRb != null)
    {
        itemRb.isKinematic = true;
    }

    item.transform.SetParent(transform);
    item.transform.localPosition = new Vector3(0, 2f, 2.5f); // Adjusted position
}

    void HandleDrop()
    {
        // Raycast to check if the held item is over the cutting board
        if (CompareTag("CuttingBoard") && heldItem.CompareTag("Tomato"))
            {
             CuttingBoard cuttingBoard = GetComponent<Collider>().GetComponent<CuttingBoard>();
             if (cuttingBoard != null)
             {
              cuttingBoard.PlaceItemOnBoard(heldItem);
              heldItem = null;
              return;
              }
            }
        // If not dropping on cutting board, drop on ground as usual
        DropItem();
    }

    void HandlePan()
    {
        if (CompareTag("FryingPan") && heldItem.CompareTag("Bacon"))
        {
            FryingPan fryingPan = GetComponent<Collider>().GetComponent<FryingPan>();
            if (fryingPan != null)
            {
                fryingPan.PlaceItemOnPan(heldItem);
                Debug.Log("OnPan");
                heldItem = null;
                return;
            }
        }
        DropItem();
    }


void DropItem()
{
    if (heldItem == null) return;

    // Detach the item from the player
    heldItem.transform.SetParent(null);

    Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
    if (itemRb != null)
    {
        itemRb.isKinematic = false; // Enable physics for the dropped item
    }

    if (heldItem.CompareTag("Bread"))
    {
        Vector3 dropPosition = transform.position + transform.forward * 2f;
        if (Physics.Raycast(dropPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity))
        {
            dropPosition.y = hit.point.y + 0.1f; 
        }
        heldItem.transform.position = dropPosition;
        StackOnBread breadStack = heldItem.GetComponent<StackOnBread>();
        if (breadStack != null)
        {
            foreach (Transform ingredient in heldItem.transform)
            {
                Rigidbody ingredientRb = ingredient.GetComponent<Rigidbody>();
                if (ingredientRb != null)
                {
                    ingredientRb.isKinematic = true; 
                }
            }
        }
    }
    else
    {
        heldItem.transform.position = transform.position + transform.forward * 2f;
    }
    heldItem = null;
}

    void TryUseCondiment()
    {
        BreadScriptC breadScript = null;
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, 2f); 
        Debug.Log("Try to use condiment again");
        foreach (var collider in nearbyColliders)
        {
            if (collider.CompareTag("Bread"))
            {
                Debug.Log("Once more");
                breadScript = collider.GetComponent<BreadScriptC>();
                if (breadScript != null && heldItem != null)
                {
                    Debug.Log("Last one");
                    breadScript.AddCondiment(heldItem.GetComponent<CondementScript>().condimentType);
                    heldItem.SetActive(false); 
                    break;
                }
            }
        }
    }
}