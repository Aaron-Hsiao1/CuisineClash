using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KOTGGettingHit : MonoBehaviour, IHitable
{
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Execute(Transform executionSource)
    {
        KnockbackEntity(executionSource);
    }

    public void KnockbackEntity(Transform executionSource)
    {
        Vector3 dir = (transform.position - executionSource.transform.position).normalized;
        rb.AddForce (dir*50, ForceMode.Impulse);
    }
}

