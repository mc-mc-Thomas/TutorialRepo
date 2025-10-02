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



    //public void Move(InputAction.CallbackContext context)
    //{
    //    animator.SetBool("iswalking", true);
    //    if (context.canceled)
    //    {
    //        animator.SetBool("iswalking", false);
    //        animator.SetFloat("LastinputX", moveinput.x);
    //        animator.SetFloat("LastinputY", moveinput.y);
    //    }


    //     moveinput = context.ReadValue<Vector2>();
    //    animator.SetFloat("InputX", moveinput.x);
    //    animator.SetFloat("InputY", moveinput.y);
    //}
    public void Move(InputAction.CallbackContext context)
    {
        // Lese Input-Vektor
        moveinput = context.ReadValue<Vector2>();

        if (context.performed)
        {
            animator.SetBool("iswalking", true);
            animator.SetFloat("InputX", moveinput.x);
            animator.SetFloat("InputY", moveinput.y);

            // Nur wenn wirklich Bewegung stattfindet, Richtung merken
            if (moveinput != Vector2.zero)
            {
                animator.SetFloat("LastinputX", moveinput.x);
                animator.SetFloat("LastinputY", moveinput.y);
            }
        }

        if (context.canceled)
        {
            animator.SetBool("iswalking", false);
            // KEIN Setzen von LastinputX/Y hier, da moveinput == (0,0)
        }
    }

}
