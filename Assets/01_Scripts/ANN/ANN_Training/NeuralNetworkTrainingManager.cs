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

namespace ANN.Training
{
    public class NeuralNetworkTrainingManager : MonoBehaviour
    {
        public const int MaxHorizontalSimulations = 5;

        [Space, Header("Simulation")]
        [SerializeField, Min(1)] int simulations = 1;

        [SerializeField] GameObject environmentPrefab;
        [SerializeField] bool duplicateEnviorment = true;
        [SerializeField] float environmentWidth = 20f;
        [SerializeField] float environmentHeight = 10f;

        [SerializeField] GameObject agentPrefab;
        [SerializeField] Vector3 agentRelativeSpawn = Vector3.zero;

        [Space, Header("ANN Training")]
        [SerializeField] TrainingMethod trainingMethod;
        [SerializeField, Range(-1f, 1f)] float tweakScale = 0.1f;
        [SerializeField] int keepTop = 3;
        [SerializeField] int transformLowerScore = 3;

        List<NeuralNetworkTraining> neuralNetworks;

        NeuralNetworkTraining winner = null;
        int iterations = 0;
        int improvements = 0;

        public delegate void TrainingComplete();
        public static event TrainingComplete OnTrainingComplete;

        private void Awake()
        {
            neuralNetworks = new List<NeuralNetworkTraining>(simulations);

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
            if (environmentPrefab != null)
            {
                GameObject goEnvironment = GameObject.Instantiate(environmentPrefab, Vector2.zero, Quaternion.identity, transform);
            }
            else Debug.LogWarning("No environment prefab selected!");

            if (agentPrefab != null)
            {
                for (int i = 0; i < simulations; i++)
                {
                    GameObject goAgent = GameObject.Instantiate(agentPrefab, (Vector2)agentRelativeSpawn, Quaternion.identity, transform);
                    neuralNetworks.Add(goAgent.GetComponent<NeuralNetworkTraining>());
                }
            }
            else Debug.LogError("No agent prefab selected!");
        }

        private void CreateDuplicateEnvironments()
        {
            Vector2 loc = Vector2.zero;

            if (environmentPrefab != null && agentPrefab != null)
            {
                for (int i = 0; i < simulations; i++)
                {
                    GameObject goEnvironment = GameObject.Instantiate(environmentPrefab, loc, Quaternion.identity, transform);

                    GameObject goAgent = GameObject.Instantiate(agentPrefab, loc + (Vector2)agentRelativeSpawn, Quaternion.identity, transform);
                    neuralNetworks.Add(goAgent.GetComponent<NeuralNetworkTraining>());

                    loc += Vector2.right * environmentWidth;
                    if (i % MaxHorizontalSimulations == 4)
                    {
                        loc = new Vector2(0f, loc.y);
                        loc += Vector2.down * environmentHeight;
                    }
                }
            }
            else Debug.LogError("No agent/environment selected!");
        }

        public void ExtractBest(string name)
        {
            neuralNetworks[0].ANN.ExtractNeuralNetwork(name);
        }

        private void FixedUpdate()
        {
            bool allFinish = true;

            for (int i = 0; i < neuralNetworks.Count; i++)
            {
                if (!neuralNetworks[i].Finished)
                {
                    allFinish = false;
                    break;
                }
            }

            if (allFinish)
            {
                CompleteTraining();
            }
        }

        public void CompleteTraining()
        {
            Debug.Log("Training complete!");

            for (int i = 0; i < neuralNetworks.Count; i++)
            {
                neuralNetworks[i].gameObject.SetActive(true);
            }

            if (OnTrainingComplete != null)
                OnTrainingComplete();

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
        }

        private void Breed()
        {
            neuralNetworks.Sort((x, y) => x.Score > y.Score ? -1 : 1);

            if (neuralNetworks[0] != winner)
            {
                improvements++;

                Debug.Log("New Winner!");

                if (winner)
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
            for (int i = 1; i < keepTop; ++i)
            {
                neuralNetworks[i].ANN.TweakRandom(tweakScale);
            }

            //BREED
            for (int i = keepTop; i < neuralNetworks.Count - transformLowerScore; ++i)
            {
                int randomWeight = Random.Range(0, neuralNetworks[i].ANN.NumberOfWeights);
                int randomBias = Random.Range(0, neuralNetworks[i].ANN.NumberOfNeurons);
                neuralNetworks[i].ANN.InheritWeight(randomWeight, neuralNetworks[0].ANN.GetWeight(randomWeight));
                neuralNetworks[i].ANN.InheritBias(randomBias, neuralNetworks[0].ANN.GetBias(randomBias));

                if (i % 2 == 0)
                    neuralNetworks[i].ANN.MutateRandom();
            }

            //TRANSFORM
            for (int i = 0; i < transformLowerScore; ++i)
            {
                int index = neuralNetworks.Count - (i + 1);
                if (index > keepTop)
                {
                    neuralNetworks[index].ANN.InheritNetwork(neuralNetworks[0].ANN);
                    neuralNetworks[index].ANN.MutateRandom();
                }
            }
        }

        private void Study()
        {

        }
    }
}


