using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentMovement : MonoBehaviour
{
    [SerializeField] NeuralNetwork ANN;
    [SerializeField] float speed = 1f;
    [SerializeField] float rotation = 1f;
    [SerializeField] LayerMask environment;

    Vector2 start;
    float[] input = new float[3];
    float timer = 0f;
    float bonus = 0f;

    private void Start()
    {
        start = transform.position;
        ANN.OnAnnReset += ANN_OnAnnReset;
    }

    private void OnDestroy()
    {
        ANN.OnAnnReset -= ANN_OnAnnReset;
    }

    private void ANN_OnAnnReset()
    {
        timer = 0f;
        bonus = 0f;
        transform.position = start;
        transform.rotation = Quaternion.identity;
    }

    private void FixedUpdate()
    {
        if (ANN != null && !ANN.Dead)
        {
            timer += 0.016f;

            transform.position += transform.right * speed;

            //stores the distances of three raycasts
            input[0] = Physics2D.Raycast(transform.position, transform.right, 100f, environment).distance;
            input[1] = Physics2D.Raycast(transform.position, (transform.right + transform.up).normalized, 100f, environment).distance;
            input[2] = Physics2D.Raycast(transform.position, (transform.right - transform.up).normalized, 100f, environment).distance;

            //predicts a value based on the three raycasts
            float prediction = (float)ANN.predict(input)[0];
            transform.Rotate(Vector3.forward, rotation * prediction);
            bonus += Mathf.Abs(prediction * 0.016f) + 0.016f;
            //Debug.Log($"Input : ({input[0]},{input[1]},{input[2]}), Prediction : {(float)prediction}");
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            ANN.Die(bonus);
        }
        else if (collision.CompareTag("Finish"))
        {
            ANN.Die(bonus + (1f - timer/100f) * 100f);
        }
    }
}
