using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using ANN;

[CustomEditor(typeof(NeuralNetwork))]
[CanEditMultipleObjects]
public class NeuralNetworkEditor : Editor
{
    NeuralNetwork ANN;

    SerializedProperty ANN_Shape;
    SerializedProperty ANN_Model;
    SerializedProperty ANN_hidden;
    SerializedProperty ANN_output;

    string fileName = "";

    private void OnEnable()
    {
        ANN_Shape = serializedObject.FindProperty("shape");
        ANN_Model = serializedObject.FindProperty("model");
        ANN_hidden = serializedObject.FindProperty("hiddenLayerActivation");
        ANN_output = serializedObject.FindProperty("outputActivation");
    }

    public override void OnInspectorGUI()
    {
        ANN = (NeuralNetwork) target;

        serializedObject.Update();

        if (ANN.ModelSelected)
        {
            EditorGUILayout.PropertyField(ANN_Model);
        }
        else
        {
            EditorGUILayout.PropertyField(ANN_Model);
            EditorGUILayout.PropertyField(ANN_Shape);
            EditorGUILayout.PropertyField(ANN_hidden);
            EditorGUILayout.PropertyField(ANN_output);
        }

        if (EditorApplication.isPlaying)
        {
            GUILayout.TextField("Model Name");
            fileName = EditorGUILayout.TextField(fileName);

            if (GUILayout.Button("Extract Model"))
            {
                ANN.ExtractNeuralNetwork(fileName);
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
