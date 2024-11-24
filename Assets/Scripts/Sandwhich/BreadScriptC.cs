using UnityEngine;

public class BreadScriptC : MonoBehaviour
{
    public bool hasCondiment = false;
    public string appliedCondiment;
    public GameObject ketchupLayer; 
    private bool isPlayerNear = false; 
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            isPlayerNear = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
        }
    }

    public void AddCondiment(string condimentType)
{
    if (isPlayerNear && !hasCondiment) 
    {
        hasCondiment = true; 
        appliedCondiment = condimentType;
        Debug.Log($"{condimentType} added to the bread!");

        if (condimentType == "Ketchup" && ketchupLayer != null)
        {
            ketchupLayer.SetActive(true); 
        }
    }
}
}
