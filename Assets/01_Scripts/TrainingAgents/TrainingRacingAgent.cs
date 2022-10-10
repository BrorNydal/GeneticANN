using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingRacingAgent : TrainingAgent
{    
    [SerializeField] float speed = 1f;
    [SerializeField] float rotation = 1f;        

    private void FixedUpdate()
    {
        if (training != null && !training.Finished)
        {
            timer += 0.016f;

            transform.position += transform.right * speed;

            //stores the distances of three raycasts
            Input
            (
                Physics2D.Raycast(transform.position, transform.right, 100f, environment).distance,
                Physics2D.Raycast(transform.position, (transform.right + transform.up).normalized, 100f, environment).distance,
                Physics2D.Raycast(transform.position, (transform.right - transform.up).normalized, 100f, environment).distance
            );

            //predicts a value based on the three raycasts
            float prediction = Output[0];

            transform.Rotate(Vector3.forward, rotation * prediction);
            bonus += Mathf.Abs(prediction * 0.016f) + 0.016f;
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            FinishTraining();
        }
        else if (collision.CompareTag("Finish"))
        {
            FinishTraining((1f - timer/100f) * 100f);
        }
    }    
}
