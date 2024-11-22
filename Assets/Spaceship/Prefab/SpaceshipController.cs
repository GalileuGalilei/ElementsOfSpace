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
    
    private Rigidbody2D rb;
    private bool onGround = false;
    private PlayerController player = null;
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

        //as it approuchs the ground, the spaceship will slow down. It uses raycast to detect the ground
        RaycastHit2D hit;
        hit = Physics2D.Raycast(transform.position, Vector3.down, 100, LayerMask.GetMask("Block"));
        float distance = hit.distance;
        if (distance < 30)
        {
            rb.AddForce(Vector2.up * deacceleration * rb.velocity.magnitude);    
        }

        if(player == null)
        {
            return;
        }

        player.transform.position = transform.position;
        
    }
     
    public void SetPlayer(PlayerController player)
    {
        this.player = player;
    }
}
