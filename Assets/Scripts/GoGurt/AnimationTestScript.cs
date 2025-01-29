using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTestScript : MonoBehaviour
{
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("TurningRight", false);
        animator.SetBool("TurningLeft", false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("TurningLeft", false);
            animator.SetBool("TurningRight", true);
        }

        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("TurningRight", false);
            animator.SetBool("TurningLeft", true);
        }

        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            animator.SetBool("TurningLeft", false);
            animator.SetBool("TurningRight", false);
            Debug.Log("holding none");
        }

    }
}
