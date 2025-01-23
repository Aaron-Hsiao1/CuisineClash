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
        if (Input.GetKeyDown(KeyCode.D))
        {
            animator.SetBool("TurningRight", true);
            Debug.Log("TurningRight");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            animator.SetBool("TurningLeft", true);
            Debug.Log("TurningLeft");
        }
    }
}
