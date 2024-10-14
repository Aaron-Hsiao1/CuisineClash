using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Unity.Netcode;

public class AnimationCode : MonoBehaviour
{
    // Start is called before the first frame update
    private Animator mAnimator;
    public GameObject otherObject;
    private bool grounded;
    public LayerMask Ground;
    public float playerHeight;
    public KeyCode jumpKey = KeyCode.Space;
    void Start()
    {
        mAnimator = otherObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (!IsLocalPlayer)
        {   
            return;
        }*/
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, Ground);
        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        if (mAnimator != null)
        {
            if (movementDirection != Vector3.zero)
            {
                mAnimator.SetBool("IsMoving", true);
            }
            else
            {
                mAnimator.SetBool("IsMoving", false);
            }
            if (grounded)
            {
                if (Input.GetKey(jumpKey))
                {
                    mAnimator.SetBool("IsInAir", true);
                }
                else
                {
                    mAnimator.SetBool("IsInAir", false);
                }
            }
        }
    }
}
