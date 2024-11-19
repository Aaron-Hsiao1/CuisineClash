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
                if (hit.collider.CompareTag("CanPickUp") || hit.collider.CompareTag("Tomato"))
                {
                    PickupItem(hit.collider.gameObject);
                }
            }
        }
    }
}

void PickupItem(GameObject item)
{
    heldItem = item;

    // Disable item's physics (e.g., gravity) and make it a child of the player
    Rigidbody itemRb = item.GetComponent<Rigidbody>();
    if (itemRb != null)
    {
        itemRb.isKinematic = true;
    }

    // Attach the item to the player
    item.transform.SetParent(transform);

    // Set item's position in front and above the player
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


    void DropItem()
{
    // Detach the item from the player
    heldItem.transform.SetParent(null);

    // Re-enable physics for the item
    Rigidbody itemRb = heldItem.GetComponent<Rigidbody>();
    if (itemRb != null)
    {
        itemRb.isKinematic = false;
    }

    // Place the item on the ground in front of the player
    heldItem.transform.position = transform.position + transform.forward * 2f;

    // Clear the reference to the held item
    heldItem = null;
}



}