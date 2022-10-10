using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentController : MonoBehaviour
{
    [SerializeField] NeuralNetwork ANN;
    [SerializeField] float speed = 1f;
    [SerializeField] float rotation = 1f;
    [SerializeField] LayerMask environment;

    Vector2 start;
    float[] input = new float[3];

    private void Start()
    {
        start = transform.position;
    }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            transform.position = start;
            transform.rotation = Quaternion.identity;
        }
        else if (collision.CompareTag("Finish"))
        {
            transform.position = start;
            transform.rotation = Quaternion.identity;
        }
    }
}
