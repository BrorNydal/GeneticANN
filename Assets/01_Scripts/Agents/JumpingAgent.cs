using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingAgent : AgentANN
{
    [Header("JumpingAgent")]
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float jumpingStrength = 1f;    
    [SerializeField] float cooldown = 0.3f;

    float cdTimer = 0f;
    bool canJump = true;

    private void FixedUpdate()
    {
        Input
        (
            Physics2D.Raycast(transform.position, transform.up, 100f, environment).distance,
            Physics2D.Raycast(transform.position, (transform.right + transform.up * 2f).normalized, 100f, environment).distance,
            Physics2D.Raycast(transform.position, (transform.right + transform.up).normalized, 100f, environment).distance,
            Physics2D.Raycast(transform.position, (transform.right * 2f + transform.up).normalized, 100f, environment).distance,
            Physics2D.Raycast(transform.position, transform.right, 100f, environment).distance,
            Physics2D.Raycast(transform.position, (transform.right + -transform.up * 2f).normalized, 100f, environment).distance,
            Physics2D.Raycast(transform.position, (transform.right + -transform.up).normalized, 100f, environment).distance,
            Physics2D.Raycast(transform.position, (transform.right * 2f + -transform.up).normalized, 100f, environment).distance,
            Physics2D.Raycast(transform.position, -transform.up, 100f, environment).distance
        );

        bool jump = Output[0] > 0.5;

        if (jump && canJump)
        {
            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpingStrength, ForceMode2D.Impulse);

            canJump = false;
            cdTimer = cooldown;
        }

        if (!canJump)
        {
            cdTimer -= 0.016f;
            if (cdTimer < 0f)
                canJump = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            transform.position = start;
            rigid.velocity = Vector2.zero;
            canJump = true;
        }
    }
}
