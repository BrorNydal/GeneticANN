using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentANN : MonoBehaviour
{
    [SerializeField] protected NeuralNetwork ANN;
    [SerializeField] protected LayerMask environment;

    protected Vector2 start;
    protected float[] input;

    // Start is called before the first frame update
    void Start()
    {
        if (ANN != null)
        {
            input = new float[ANN.Shape[0]];
        }
        else Debug.LogError($"ANN null on agent {gameObject.name}!");

        start = transform.position;
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
}
