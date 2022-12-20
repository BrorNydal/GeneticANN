using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingAgent : AgentANN
{
    [SerializeField] float speed = 1f;
    [SerializeField] float rotation = 1f;

    private void FixedUpdate()
    {
        transform.position += transform.right * speed;

        //stores the distances of three raycasts
        input[0] = Physics2D.Raycast(transform.position, transform.right, 100f, environment).distance;
        input[1] = Physics2D.Raycast(transform.position, (transform.right + transform.up).normalized, 100f, environment).distance;
        input[2] = Physics2D.Raycast(transform.position, (transform.right - transform.up).normalized, 100f, environment).distance;

        //predicts a value based on the three raycasts
        float prediction = ANN.predict(input)[0];
        transform.Rotate(Vector3.forward, rotation * prediction);
    }
}
