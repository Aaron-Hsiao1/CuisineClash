using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotatoOrientationFix : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(0.35f, 0.35f, 0.35f);
        transform.localPosition = new Vector3(0.0204f, 0.0111f, 0.0106f);
        transform.localRotation = Quaternion.Euler(-97.53f, 125.794f, -292.38f);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
