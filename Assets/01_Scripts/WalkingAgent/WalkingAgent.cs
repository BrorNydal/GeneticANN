using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingAgent : AgentANN
{
    [SerializeField] float limbHandling = 1f;
    [SerializeField] Limb[] limbs;

    private void FixedUpdate()
    {
        float[] input = new float[ANN.NumberOfInputs];

        for(int i = 0; i < limbs.Length; i++)
        {
            input[i] = limbs[i].transform.rotation.eulerAngles.z;
        }

        Input(input);
        float[] output = Output;

        for (int i = 0; i < limbs.Length; i++)
        {            
            limbs[i].Rotate(output[i] * limbHandling);
        }
    }
}
