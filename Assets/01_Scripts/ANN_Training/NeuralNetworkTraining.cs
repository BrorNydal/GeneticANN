using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System;
using System.IO;

public class NeuralNetworkTraining : MonoBehaviour
{
    [SerializeField] public NeuralNetwork ANN;

    public delegate void ANNReset();
    public event ANNReset OnAnnReset;

    public bool Finished { get { return finished; } }
    public float Score { get { return score; } set { if (!finished) score = value; } }

    bool finished = false;
    float score = 0f;    

    public void Finish()
    {
        finished = true;
    }

    public void Finish(float _score)
    {
        Score = _score;
        finished = true;
    }

    public void ResetANN()
    {
        finished = false;
        Score = 0f;
        OnAnnReset();
    }

    /// <summary>
    /// Extracts and saves the network, so that we can use it in non-simulated scenarios.
    /// </summary>
    public void ExtractNeuralNetwork(string name)
    {
        Debug.Log("Extracting model...");

        string path = Path.Combine("Assets/ANN_Models/", name+".txt");

        try
        {
            //Directory.CreateDirectory(Path.GetDirectoryName(path));
            //JsonUtility.ToJson(ANN, true);

            NeuralNetworkData data = ANN.CreateNeuralNetworkData();
            string dataToStore = JsonUtility.ToJson(data, true);

            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error on saving file to : {path}\n {ex.Message}");
        }
    }
}
