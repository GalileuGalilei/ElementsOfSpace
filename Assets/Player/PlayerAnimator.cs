using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if(rb.velocity.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        
        if (rb.velocity.x < 0)
        {
            spriteRenderer.flipX = true;
        }
    }

}
