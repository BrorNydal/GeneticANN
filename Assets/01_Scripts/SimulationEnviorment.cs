using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationEnviorment : MonoBehaviour
{
    [SerializeField] List<GameObject> stages;
    int currentStage = 0;

    public void NextStage()
    {
        if(currentStage < stages.Count - 1)
        {
            stages[currentStage].SetActive(false);
            currentStage++;
            stages[currentStage].SetActive(true);
        }
    }
}
