using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingAgent : MonoBehaviour
{
    [SerializeField] protected NeuralNetwork ANN;
    [SerializeField] protected NeuralNetworkTraining training;
    [SerializeField] protected LayerMask environment;

    protected Vector2 start;
    protected float[] input;
    protected float timer = 0f;
    protected float bonus = 0f;

    private void Start()
    {
        if (training != null)
        {
            training.OnAnnReset += ANN_OnAnnReset;
        }
        else Debug.LogError($"Training network null on training agent {gameObject.name}!");
        if (ANN != null)
        {
            input = new float[ANN.Shape[0]];
        }
        else Debug.LogError($"ANN null on training agent {gameObject.name}!");

        start = transform.position;       
    }

    private void OnDestroy()
    {
        if(training != null)
            training.OnAnnReset -= ANN_OnAnnReset;
    }

    private void Update()
    {
        if(training != null && !training.Finished)
        {
            timer += Time.deltaTime;
        }
    }

    /// <summary>
    /// Must be set before calling Output for sensible predictions.
    /// </summary>
    /// <param name="inputs"> Inputs for the neural network </param>
    protected void Input(params float[] inputs)
    {
        if (inputs.Length != input.Length)
        {
            Debug.LogWarning("Inputs does not match network inputs. Canceling...");
            return;
        }

        input = inputs;
    }

    protected float[] Output
    {
        get { return ANN.predict(input); }
    }

    protected virtual void FinishTraining()
    {
        if(!training.Finished)
            training?.Finish(bonus);
    }

    protected void FinishTraining(float extra)
    {
        bonus += extra;
        FinishTraining();
    }

    protected virtual void ANN_OnAnnReset()
    {
        timer = 0f;
        bonus = 0f;
        transform.position = start;
        transform.rotation = Quaternion.identity;
    }
}
