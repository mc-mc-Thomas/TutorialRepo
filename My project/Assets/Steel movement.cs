using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Steelmovement : MonoBehaviour
{
    public float moveSpeed;
    public Rigidbody2D rigidbody;
    private Vector2 movedirection;


    public Sprite frontSprite;
    public Sprite backSprite;
    public Sprite rightSprite;
    public Sprite leftSprite;
    public SpriteRenderer spriteRenderer;


    // Update is called once per frame
    void Update()
    {
        //Processing Inputs
        ProcessInputs();
        UpdateSpriteDirection();

    }

    void FixedUpdate()
    {
        //Pysiks Calculations
        Move();
    }

    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        movedirection = new Vector2(moveX, moveY);
    }

    void Move()
    {
        rigidbody.velocity = new Vector2(movedirection.x * moveSpeed, movedirection.y * moveSpeed);
    }


    void UpdateSpriteDirection()
    {
        if (movedirection.x > 0)
        {
            spriteRenderer.sprite = rightSprite;
        }
        else if (movedirection.x < 0)
        {
            spriteRenderer.sprite = leftSprite;
        }
        else if (movedirection.y > 0)
        {
            spriteRenderer.sprite = backSprite;
        }
        else if (movedirection.y < 0)
        {
            spriteRenderer.sprite = frontSprite;
        }
    }



}
