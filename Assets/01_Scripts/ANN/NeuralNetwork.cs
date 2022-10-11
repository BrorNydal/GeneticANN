using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Playables;
using static Unity.Burst.Intrinsics.Arm;
using static UnityEditor.Experimental.GraphView.GraphView;

[System.Serializable]
public enum Activation : int
{
    Sigma = 0, SigmaExtended = 1, BinaryStep = 2
}

[System.Serializable]
public class NeuralNetworkData
{
    public int[] shape;
    public float[] weights;
    public float[] biases;
    public Activation hiddenLayerActivation;
    public Activation outputActivation;
}

[System.Serializable]
public class NeuralNetwork : MonoBehaviour
{
    [SerializeField] TextAsset model;
    [SerializeField] int[] shape;
    [SerializeField] Activation hiddenLayerActivation = Activation.Sigma;
    [SerializeField] Activation outputActivation = Activation.Sigma;

    public int[] Shape { get { return shape; } }
    public int NumberOfLayers { get { return shape.Length; } }
    public int NumberOfInputs { get { return shape[0]; } }
    public int NumberOfOutputs { get { return shape[shape.Length - 1]; } }
    public int NumberOfWeights { get { return weights.Length; } }
    public int NumberOfNeurons { get { return biases.Length; } }

    public bool ModelSelected { get { return model != null; } }

    public float[] Weights { get { return weights; } }
    public float[] Biases { get { return biases; } }
   
    float[] weights;

    //Excluding input and output neurons
    float[] biases;    

    //array of floats that is set to new values in prediction
    //index to this array coresponds to the index of the biases array
    //biases[index] is the bias of neuron neurons[index]
    float[] neurons;

    private void Awake()
    {
        if (ModelSelected)
        {
            //Try import the selected model
            try
            {
                string dataToLoad = model.text;
                NeuralNetworkData ann = JsonUtility.FromJson<NeuralNetworkData>(dataToLoad);
                shape = ann.shape;
                weights = ann.weights;
                biases = ann.biases;
                neurons = new float[NumberOfNeurons];
                hiddenLayerActivation = ann.hiddenLayerActivation;
                outputActivation = ann.outputActivation;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error on loading model \n {ex.Message}");
            }            
        }
        else
        {
            //Create a new model
            InitializeNeuralNetworkShape();
            RandomizeAll();
        }
    }

    private void RandomizeAll()
    {
        for (int i = 0; i < weights.Length; ++i)
        {
            weights[i] = Random.Range(-1f, 1f);
        }
        for (int i = 0; i < biases.Length; ++i)
        {
            biases[i] = Random.Range(-1f, 1f);
        }
    }

    private void InitializeNeuralNetworkShape()
    {
        if (shape.Length <= 0)
        {
            Debug.LogError("Length of shape is null, must be minimum 2");
            return;
        }

        int numberOfNeurons = 0;
        int numberOfWeights = 0;        

        for (int i = 1; i < NumberOfLayers; i++)
        {
            numberOfWeights += shape[i] * shape[i - 1];
            numberOfNeurons += shape[i];
        }

        weights = new float[numberOfWeights];
        neurons = new float[numberOfNeurons];
        biases = new float[numberOfNeurons];
    }

    public float[] predict(float[] input)
    {
        if(input.Length != NumberOfInputs)
        {
            Debug.LogError("Number of inputs in prediction does not match the number of inputs in the ANN input layer!");
            return new float[NumberOfOutputs];
        }

        float[] layerValues = input;
        float[] newLayerValues;
        float[] ws;

        for (int layer = 1; layer < NumberOfLayers; ++layer)
        {
            newLayerValues = new float[shape[layer]];

            for(int neuron = 0; neuron < shape[layer]; ++neuron)
            {
                //TODO: We need outp
                int neuronIndex = GetNeuronIndex(layer, neuron);
                neurons[neuronIndex] = 0f;

                ws = GetWeights(layer, neuron);

                if (ws.Length != layerValues.Length)
                    Debug.LogWarning("Length of weight array and previous layer size is not equal!");

                for(int weight = 0; weight < ws.Length; ++weight)
                {
                    neurons[neuronIndex] += ws[weight] * layerValues[weight];
                }
                neurons[neuronIndex] += biases[neuronIndex];
                neurons[neuronIndex] = ExecActivation(layer, neurons[neuronIndex]);

                newLayerValues[neuron] = neurons[neuronIndex];
            }

            layerValues = newLayerValues;
        }

        return layerValues;
    }    

    float ExecActivation(int layer, float val)
    {
        Activation activation;

        if (layer == NumberOfLayers - 1)
            activation = outputActivation;
        else
            activation = hiddenLayerActivation;

        return ActivationFunction.ExecActivation(activation, val);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="layer"> If 0, then the weights between 0 and 1. </param>
    /// <returns> Number of weights between layer and layer + 1 </returns>
    public int WeightCount(int layer)
    {
        if(layer < 0 || layer >= NumberOfLayers)
        {
            Debug.LogError("Requesting weight count outside the range of the network!");
            return -1;
        }

        return shape[layer] * shape[layer + 1];
    }

    public int WeightCountPerNeuron(int layer)
    {
        if (layer < 0 || layer >= NumberOfLayers)
        {
            Debug.LogError("Requesting weight count per neuron, outside the range of the network!");
            return -1;
        }

        return shape[layer];
    }

    public float[] GetWeights(int layer, int neuron)
    {
        if(layer <= 0 || layer >= NumberOfLayers)
        {
            Debug.LogError("Trying to access weight outside range!");
            return new float[0];
        }

        int startIndex = 0;
        for(int i = 1; i < layer; ++i)
        {
            startIndex += shape[i] * shape[i - 1];
        }

        int weightsPerNeuron = WeightCountPerNeuron(layer - 1);
        startIndex += neuron * weightsPerNeuron;

        float[] result = new float[weightsPerNeuron];
        for(int i = 0; i < weightsPerNeuron; ++i)
        {
            result[i] = weights[startIndex + i];
        }

        return result;
    }

    public float GetWeight(int index)
    {
        return weights[index];
    }

    public float GetBias(int index)
    {
        return biases[index];
    }

    public int GetNeuronIndex(int layer, int index)
    {
        int id = index;
        for(int i = 1; i < layer; i++)
        {
            id += shape[i];
        }

        return id;
    }

    public void InheritNetwork(NeuralNetwork ann)
    {
        for(int i = 0; i < NumberOfWeights; i++)
            weights[i] = ann.Weights[i];
        for(int i = 0; i < NumberOfNeurons; i++)
            biases[i] = ann.Biases[i];
    }

    public void InheritWeight(int weightIndex, float value)
    {
        weights[weightIndex] = value;
    }

    public void InheritBias(int biasIndex, float value)
    {
        biases[biasIndex] = value;
    }

    public void MutateRandom()
    {
        if(0.5f < Random.Range(0f, 1f))
        {
            MutateRandomWeight();
        }
        else
        {
            MutateRandomBias();
        }
    }

    public void TweakRandom(float tweak)
    {
        if (0.5f < Random.Range(0f, 1f))
        {
            TweakRandomWeight(tweak);
        }
        else
        {
            TweakRandomBias(tweak);
        }
    }

    public void MutateRandomWeight()
    {
        int w = Random.Range(0, weights.Length);
        weights[w] = Random.Range(-1f, 1f);
    }

    public void TweakRandomWeight(float tweak)
    {
        int w = Random.Range(0, weights.Length);
        weights[w] += Random.Range(-tweak, tweak);
        weights[w] = Mathf.Clamp(weights[w], -1f, 1f);
    }

    public void MutateRandomBias()
    {
        int b = Random.Range(0, biases.Length);
        biases[b] = Random.Range(-1f, 1f);
    }

    public void TweakRandomBias(float tweak)
    {
        int b = Random.Range(0, biases.Length);
        biases[b] += Random.Range(-tweak, tweak);
        biases[b] = Mathf.Clamp(biases[b], -1f, 1f);
    }



    public NeuralNetworkData CreateNeuralNetworkData()
    {
        return new NeuralNetworkData { shape = Shape, weights = Weights, biases = Biases, hiddenLayerActivation = this.hiddenLayerActivation, outputActivation = this.outputActivation};
    }
}


public static class ActivationFunction
{
    public static float ExecActivation(Activation act, float val)
    {
        switch (act)
        {
            case Activation.Sigma:
                return Sigma(val);
            case Activation.SigmaExtended:
                return SigmaExtend(val);
        }
        return 0f;
    }

    public static float Sigma(float val)
    {
        return 1f / (1f + Mathf.Exp(-val));
    }
    public static float SigmaExtend(float val)
    {
        return ((1f / (1f + Mathf.Exp(-val))) * 2f) - 1f;
    }

    public static float BinaryStep(float val)
    {
        return val >= 0 ? 1f : 0f;
    }

    public static float Tanh(float val)
    {
        return (float)System.Math.Tanh(val);
    }
}

public static class DerivationFunction
{
    public static float Sigma(float val)
    {
        return val * (1 - val);
    }
}