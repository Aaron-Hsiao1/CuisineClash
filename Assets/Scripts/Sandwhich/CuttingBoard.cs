    using UnityEngine;

public class CuttingBoard : MonoBehaviour
{
    private GameObject itemOnBoard = null;
    private int cuttingProgress = 0;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is a tomato
        if (other.CompareTag("Tomato"))
        {
            PlaceItemOnBoard(other.gameObject);
        }
    }

    public void PlaceItemOnBoard(GameObject item)
    {
        if (itemOnBoard == null)
        {
            itemOnBoard = item;

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
        // Handle cutting if an item is on the board and the player right-clicks
        if (itemOnBoard != null && Input.GetMouseButtonDown(0))
        {
            HandleCutting();
        }
    }

    void HandleCutting()
    {
        if (itemOnBoard.CompareTag("Tomato"))
        {
            cuttingProgress++;

            if (cuttingProgress >= 4)
            {
                CreateSlicedTomato();
            }
        }
    }

    void CreateSlicedTomato()
    {
        if (itemOnBoard != null)
        {
            // Replace the tomato with sliced tomato
            GameObject slicedTomato = Instantiate(itemOnBoard.GetComponent<GroceryItem>().slicedVersionPrefab, transform.position, Quaternion.identity);
            Destroy(itemOnBoard);
            itemOnBoard = null;
            cuttingProgress = 0;
        }
    }
}
