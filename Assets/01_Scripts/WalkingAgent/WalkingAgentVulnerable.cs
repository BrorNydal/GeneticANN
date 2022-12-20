using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ANN.Training;

public class WalkingAgentVulnerable : MonoBehaviour
{
    NeuralNetworkTraining trainer;

    private void Awake()
    {
        trainer = GetComponentInParent<NeuralNetworkTraining>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Wall"))
        {
            trainer.Success();
        }
    }
}
