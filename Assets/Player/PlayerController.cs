using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f;

    private InputAction moveAction;
    private Rigidbody2D rb;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        moveAction = new InputAction(binding: "<Gamepad>/leftStick");
        moveAction.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 dir = moveAction.ReadValue<Vector2>();
        rb.velocity = dir * moveSpeed;
    }


}
