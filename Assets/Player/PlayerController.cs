using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Processors;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private DrillController drillController;

    public Vector2 facingDirection { get; private set; }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        drillController = GetComponentInChildren<DrillController>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 dir = context.ReadValue<Vector2>();
        rb.velocity = dir * moveSpeed;
        
        if(context.canceled)
        {
            return;
        }

        facingDirection = dir;
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //if started
        if (context.started)
        {
            drillController.Use();
        }

        //if canceled
        if (context.canceled)
        {
            drillController.Stop();
        }
    }
}
