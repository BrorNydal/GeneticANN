using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NeuralNetwork))]
[CanEditMultipleObjects]
public class NeuralNetworkEditor : Editor
{
    NeuralNetwork ANN;

    SerializedProperty ANN_Shape;
    SerializedProperty ANN_Model;

    private void OnEnable()
    {
        ANN_Shape = serializedObject.FindProperty("shape");
        ANN_Model = serializedObject.FindProperty("model");
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
        }

        serializedObject.ApplyModifiedProperties();
    }
}
