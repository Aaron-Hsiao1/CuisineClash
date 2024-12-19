using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachScript : MonoBehaviour
{
    public GameObject childObject; // Assign the child object (weapon)
    public GameObject parentObject; // Assign the parent object (character's hand)
    public HotPotatoTag HPT;
    private bool potatocreated = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (HPT.hasHotPotato && !potatocreated)
        {
            if (childObject != null && parentObject != null)
            {
                childObject = Instantiate(childObject, transform.position, Quaternion.identity);
                childObject.transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
                childObject.transform.SetParent(parentObject.transform, true);
                childObject.transform.localPosition = new Vector3(0.0204f, 0.0111f, 0.0106f);
                childObject.transform.localRotation = Quaternion.Euler(-97.53f, 125.794f, -292.38f);

            }
            potatocreated = true;
        }
        if (!HPT.hasHotPotato && potatocreated)
        {
            Destroy(childObject);
        }
    }
}
