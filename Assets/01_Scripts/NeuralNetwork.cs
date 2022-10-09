using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork : MonoBehaviour
{
    [SerializeField] int[] shape;

    public delegate void ANNReset();
    public event ANNReset OnAnnReset; 

    public int NumberOfLayers { get { return shape.Length; } }
    public int NumberOfInputs { get { return shape[0]; } }
    public int NumberOfOutputs { get { return shape[shape.Length - 1]; } }
    public int NumberOfWeights { get { return numberOfWeights; } }
    public int NumberOfNeurons { get { return numberOfNeurons; } }

    public float[] Weights { get { return weights; } }
    public float[] Biases { get { return biases; } }

    int numberOfWeights = 0;    
    float[] weights;    

    //Excluding input and output neurons
    int numberOfNeurons = 0;
    float[] biases;

    bool dead = false;
    public bool Dead { get { return dead; } }
    float score = 0f;
    public float Score { get { return score; } set { if(!dead) score = value; } }

    //array of floats that is set to new values in prediction
    //index to this array coresponds to the index of the biases array
    //biases[index] is the bias of neuron neurons[index]
    float[] neurons;

    private void Awake()
    {
        InitializeNeuralNetworkShape();

        for(int i = 0; i < weights.Length; ++i)
        {
            weights[i] = Random.Range(-1f, 1f);
        }

        for(int i = 0; i < biases.Length; ++i)
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

        numberOfNeurons += shape[0];

        for (int i = 1; i < NumberOfLayers; i++)
        {
            numberOfWeights += shape[i] * shape[i - 1];

            //Only count neurons not in the input and output layer
            if(i > 0 && i < NumberOfLayers - 1)
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

        for(int layer = 1; layer < NumberOfLayers; ++layer)
        {
            newLayerValues = new float[shape[layer]];

            for(int neuron = 0; neuron < shape[layer]; ++neuron)
            {
                int neuronIndex = GetNeuronIndex(layer, neuron);
                neurons[neuronIndex] = 0f;

                float[] ws = GetWeights(layer, neuron);

                if (ws.Length != layerValues.Length)
                    Debug.LogWarning("Length of weight array and previous layer size is not equal!");

                for(int weight = 0; weight < ws.Length; ++weight)
                {
                    neurons[neuronIndex] += ws[weight] * layerValues[weight];
                }
                neurons[neuronIndex] += biases[neuronIndex];
                neurons[neuronIndex] = ActivationFunction.SigmaExtend(neurons[neuronIndex]);

                newLayerValues[neuron] = neurons[neuronIndex];
            }

            layerValues = newLayerValues;
        }

        return layerValues;
    }

    public void Die()
    {
        dead = true;
    }

    public void Die(float _score)
    {
        Score = _score;
        dead = true;
    }

    public void ResetANN()
    {        
        dead = false;
        Score = 0f;
        OnAnnReset();
    }

    public void Reward(float reward)
    {
        Score += reward;
    }

    public void Punish(float punishment)
    {
        Score -= punishment;
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
        int id = 0;
        for(int i = 1; i < layer; i++)
        {
            id += shape[i];
        }
        id += index;

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

    public float GetWeightPassingTo(int layer, int neuron, int index)
    {
        return 0F;
    }

    public float GetWeightPassingTo(int neuron, int index)
    {
        return 0F;
    }

    public float GetWeightPassingFrom(int layer, int neuron, int index)
    {
        return 0F;
    }

    public float GetWeightPassingFrom(int neuron, int index)
    {
        return 0F;
    }
}


public static class ActivationFunction
{
    public static float Sigma(float val)
    {
        return 1f / (1f + Mathf.Exp(-val));
    }
    public static float SigmaExtend(float val)
    {
        return ((1f / (1f + Mathf.Exp(-val))) * 2f) - 1f;
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