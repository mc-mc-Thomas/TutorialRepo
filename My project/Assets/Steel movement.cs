using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Steelmovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Rigidbody2D rigidbody;
    private Vector2 moveinput;
    private Animator animator;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }


    void Update()
    {
        rigidbody.velocity = moveinput * moveSpeed;
       
    }



    public void Move(InputAction.CallbackContext context)
    {
        animator.SetBool("iswalking", true);
        if (context.canceled)
        {
            animator.SetBool("iswalking", false);
            animator.SetFloat("LastInputX", moveinput.x);
            animator.SetFloat("LastInputY", moveinput.y);
        }


         moveinput = context.ReadValue<Vector2>();
        animator.SetFloat("MoveX", moveinput.x);
        animator.SetFloat("MoveY", moveinput.y);
    }


}
