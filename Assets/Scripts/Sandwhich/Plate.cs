using UnityEngine;
using System.Collections.Generic;
using System;

public class Plate : MonoBehaviour
{
    private List<string> sandwichIngredients = new List<string>();
    private Boolean finished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bread"))
        {
            StackOnBread breadStack = other.GetComponent<StackOnBread>();
            if (breadStack != null)
            {
                sandwichIngredients = breadStack.GetStackedIngredients(); // No error now
            }
        }
        if (finished)
        {
           Destroy(other.gameObject);
           finished = false;
        }
    }

    public bool IsSandwichReady()
    {
        return sandwichIngredients.Count > 0;
    }

    public List<string> GetSandwichIngredients()
    {
        return sandwichIngredients;
    }

    public void ClearSandwich()
    {
        Debug.Log("Finished Making");
        sandwichIngredients.Clear();
        finished = true;
        
    }

}
