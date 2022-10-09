using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

[System.Serializable]
public enum TrainingMethod
{
    NoTraining,
    Breed,
    Study
}

public class NerualNetworkManager : MonoBehaviour
{    
    public const int MaxHorizontalSimulations = 5;
    
    [SerializeField] int randomSeed;

    [Space, Header("Simulation")]
    [SerializeField, Min(1)] int simulations = 1;
    [SerializeField] float simulationSpeed = 1f;

    [SerializeField] GameObject environmentPrefab = null;
    [SerializeField] bool duplicateEnviorment = true;
    [SerializeField] float environmentWidth = 20f;
    [SerializeField] float environmentHeight = 10f;

    [SerializeField] GameObject agentPrefab = null;
    [SerializeField] Vector3 agentRelativeSpawn = Vector3.zero;

    [Space, Header("ANN Training")]
    [SerializeField] TrainingMethod trainingMethod;
    [SerializeField, Range(-1f, 1f)] float tweakScale = 0.1f;
    [SerializeField] int keepTop = 3;
    [SerializeField] int transformLowerScore = 3;
    [SerializeField] bool randomMutateBelowTopThree = true;

    List<SimulationEnviorment> environments = new List<SimulationEnviorment>();
    List<NeuralNetwork> neuralNetworks;

    NeuralNetwork winner = null;
    bool nextStage = false;
    int iterations = 0;
    int improvements = 0;

    private void Awake()
    {
        Random.InitState(randomSeed);
        Time.timeScale = simulationSpeed;
        
        neuralNetworks = new List<NeuralNetwork>(simulations);

        if (duplicateEnviorment)
        {
            CreateDuplicateEnvironments();
        }
        else
        {
            CreateSingleSimulationEnvironment();
        }
    }

    private void CreateSingleSimulationEnvironment()
    {
        GameObject goEnvironment = GameObject.Instantiate(environmentPrefab, Vector2.zero, Quaternion.identity, transform);
        environments.Add(goEnvironment.GetComponent<SimulationEnviorment>());

        for (int i = 0; i < simulations; i++)
        {
            GameObject goAgent = GameObject.Instantiate(agentPrefab, (Vector2)agentRelativeSpawn, Quaternion.identity, transform);
            neuralNetworks.Add(goAgent.GetComponent<NeuralNetwork>());
        }
    }

    private void CreateDuplicateEnvironments()
    {
        Vector2 loc = Vector2.zero;

        for (int i = 0; i < simulations; i++)
        {
            GameObject goEnvironment = GameObject.Instantiate(environmentPrefab, loc, Quaternion.identity, transform);
            environments.Add(goEnvironment.GetComponent<SimulationEnviorment>());

            GameObject goAgent = GameObject.Instantiate(agentPrefab, loc + (Vector2)agentRelativeSpawn, Quaternion.identity, transform);
            neuralNetworks.Add(goAgent.GetComponent<NeuralNetwork>());

            loc += Vector2.right * environmentWidth;
            if (i % MaxHorizontalSimulations == 4)
            {
                loc = new Vector2(0f, loc.y);
                loc += Vector2.down * environmentHeight;
            }
        }
    }

    public void NextStage()
    {
        nextStage = true;
    }

    private void FixedUpdate()
    {
        bool allFinish = true;        

        for(int i = 0; i < neuralNetworks.Count; i++)
        {
            if (!neuralNetworks[i].Dead)
            {
                allFinish = false;
                break;
            }
        }

        if (allFinish)
        {
            float[] scores = new float[simulations];
            int best = 0;
            iterations++;

            for (int i = 0; i < neuralNetworks.Count; i++)
            {
                scores[i] = neuralNetworks[i].Score;

                if (scores[i] > scores[best])
                    best = i;
            }

            switch (trainingMethod)
            {
                case TrainingMethod.NoTraining:
                    break;
                case TrainingMethod.Breed:
                    Breed();
                    break;
                case TrainingMethod.Study:
                    Study();
                    break;
            }

            for (int i = 0; i < neuralNetworks.Count; i++)
            {
                neuralNetworks[i].ResetANN();
            }

            if(nextStage)
                environments.ForEach(x => x.NextStage());

            nextStage = false;
        }
    }

    private void Breed()
    {
        neuralNetworks.Sort((x,y) => x.Score > y.Score ? -1 : 1);  

        if (neuralNetworks[0] != winner)
        {
            improvements++;

            Debug.Log("New Winner!");

            if(winner)
                Debug.Log($"Highest score : {neuralNetworks[0].Score}, Previous : {winner.Score}, Improvements : {improvements}, Iterations : {iterations}.");
            else
                Debug.Log($"Highest score : {neuralNetworks[0].Score}, Improvements : {improvements}, Iterations : {iterations}.");

            winner = neuralNetworks[0];

            SpriteRenderer sr = neuralNetworks[0].GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.color = Color.green;
                sr.sortingOrder = 1;
            }
            for (int i = 1; i < neuralNetworks.Count; i++)
            {
                SpriteRenderer sprite = neuralNetworks[i].GetComponent<SpriteRenderer>();
                if (sprite != null)
                {
                    sprite.color = Color.white;
                    sprite.sortingOrder = 0;
                }
            }
        }

        //TWEEK TOP, EXCLUDING TOP1
        for(int i = 1; i < keepTop; ++i)
        {
            neuralNetworks[i].TweakRandom(tweakScale);
        }

        //BREED
        for (int i = keepTop; i < neuralNetworks.Count - transformLowerScore; ++i)
        {
            int randomWeight = Random.Range(0, neuralNetworks[i].NumberOfWeights);
            int randomBias = Random.Range(0, neuralNetworks[i].NumberOfNeurons);
            neuralNetworks[i].InheritWeight(randomWeight, neuralNetworks[0].GetWeight(randomWeight));
            neuralNetworks[i].InheritBias(randomBias, neuralNetworks[0].GetBias(randomBias));

            if (i % 2 == 0)
                neuralNetworks[i].MutateRandom();            
        }

        //TRANSFORM
        for (int i = 0; i < transformLowerScore; ++i)
        {
            int index = neuralNetworks.Count - (i + 1);
            if (index > keepTop)
            {
                neuralNetworks[index].InheritNetwork(neuralNetworks[0]);
                neuralNetworks[index].MutateRandom();
            }
        }        
    }

    private void Study()
    {

    }
}


