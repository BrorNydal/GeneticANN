using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] float simulationSpeed = 1f;
    [SerializeField] int randomSeed;

    bool paused = false;


    private void Awake()
    {
        Time.timeScale = simulationSpeed;
        Random.InitState(randomSeed);
    }

    public void IncreaseSimulationSpeed(float val)
    {
        simulationSpeed *= val;
        Time.timeScale = simulationSpeed;
    }
    public void DecreaseSimulationSpeed(float val)
    {
        simulationSpeed /= val;
        Time.timeScale = simulationSpeed;
    }
    public void ResetSimulationSpeed()
    {
        simulationSpeed = 1f;
        Time.timeScale = simulationSpeed;
    }

    public void PauseAndPlay()
    {
        if (!paused)
        {
            Time.timeScale = 0f;
            paused = true;
        }
        else
        {
            Time.timeScale = simulationSpeed;
            paused = false;
        }
    }
}
