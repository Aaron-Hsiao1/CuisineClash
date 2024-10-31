using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachScript : MonoBehaviour
{
    public GameObject childObject; // Assign the child object (weapon)
    public GameObject parentObject; // Assign the parent object (character's hand)
    // Start is called before the first frame update
    void Start()
    {
         if (childObject != null && parentObject != null)
        {
            childObject.transform.SetParent(parentObject.transform, false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
