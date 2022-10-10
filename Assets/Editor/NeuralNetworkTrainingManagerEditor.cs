using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NeuralNetworkTrainingManager))]
public class NeuralNetworkTrainingManagerEditor : Editor
{
    NeuralNetworkTrainingManager training;
    string fileName = "";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        training = (NeuralNetworkTrainingManager)target;

        if (EditorApplication.isPlaying)
        {
            fileName = EditorGUILayout.TextField(fileName);

            if (GUILayout.Button("Extract Model"))
            {
                training.ExtractBest(fileName);
            }
        }
    }
}
