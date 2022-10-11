using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingJumpingAgent : TrainingAgent
{
    [Header("JumpingAgent")]
    [SerializeField] float jumpingStrength = 1f;
    [SerializeField] Rigidbody2D rigid;
    [SerializeField] float cooldown = 0.3f;
    [SerializeField] SpriteRenderer spriteRenderer;

    float cdTimer = 0f;
    bool canJump = true;

    private void FixedUpdate()
    {
        if (training != null && !training.Finished)
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
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {

            FinishTraining(timer);
            rigid.simulated = false;
        }
    }

    protected override void ANN_OnAnnReset()
    {
        base.ANN_OnAnnReset();

        rigid.velocity = Vector2.zero;
        canJump = true;
        rigid.simulated = true;
    }
}


