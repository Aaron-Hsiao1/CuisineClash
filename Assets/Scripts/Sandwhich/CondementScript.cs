using UnityEngine;
using System;

public class CondementScript : MonoBehaviour
{
    public bool isEmpty = false; // Track if the bottle is empty
    public string condimentType = "Ketchup";

    public void UseCondiment(GameObject bread)
    {
        Debug.Log("UseCondiment");
        if (!isEmpty && bread != null)
        {
            // Add condiment to the bread
            BreadScriptC breadScript = bread.GetComponent<BreadScriptC>();
            if (breadScript != null)
            {
                breadScript.AddCondiment(condimentType);
                isEmpty = true;
                Debug.Log($"{condimentType} applied to bread.");
            }
        }
    }
}
