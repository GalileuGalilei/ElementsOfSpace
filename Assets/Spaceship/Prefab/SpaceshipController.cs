using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Class that simulates and entering in atmosfere of a planet
/// </summary>
public class SpaceshipController : MonoBehaviour
{
    [SerializeField]
    private float deacceleration = 0.1f;
    [SerializeField]
    private GameObject player;
    private Rigidbody2D rb;
    private bool onGround = false;
    private ParticleSystem particle;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        particle = GetComponentInChildren<ParticleSystem>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Block")
        {
            onGround = true;
            particle.Stop();
        }
    }

    private void FixedUpdate()
    {
        if(onGround)
        {
            return;
        }

        if(transform.position.y < 1)
        {
            onGround = true;
        }

        player.transform.position = transform.position;

        //as if approuchs the ground, the spaceship will slow down. It uses raycast to detect the ground
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector3.down, 100, LayerMask.GetMask("Block"));
        float distance = hit.distance;
        if (distance < 30)
        {
            rb.AddForce(Vector2.up * deacceleration * rb.velocity.magnitude);    
        }
        
    }
}
