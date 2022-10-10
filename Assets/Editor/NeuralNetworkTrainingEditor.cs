using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NeuralNetworkTraining))]
public class NeuralNetworkTrainingEditor : Editor
{
    NeuralNetworkTraining training;
    string fileName = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        training = (NeuralNetworkTraining)target;

        if (EditorApplication.isPlaying)
        {
            fileName = EditorGUILayout.TextField(fileName);

            if (GUILayout.Button("Extract Model"))
            {
                training.ExtractNeuralNetwork(fileName);
            }
        }
    }
}
