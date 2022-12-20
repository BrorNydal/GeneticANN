using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ANN.Training;

public class Wallspawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float spawnX = 12f;
    [SerializeField] float[] spawnY;    
    [SerializeField] float spawnCooldown = 1f;

    float cdTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        NeuralNetworkTrainingManager.OnTrainingComplete += NeuralNetworkTrainingManager_OnTrainingComplete;
    }

    private void OnDestroy()
    {
        NeuralNetworkTrainingManager.OnTrainingComplete -= NeuralNetworkTrainingManager_OnTrainingComplete;
    }

    private void NeuralNetworkTrainingManager_OnTrainingComplete()
    {
        Wallmovement[] walls = GetComponentsInChildren<Wallmovement>();

        for(int i = 0; i < walls.Length; i++)
        {
            Destroy(walls[i].gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        cdTimer -= Time.deltaTime;

        if(cdTimer < 0f)
        {
            GameObject spawn = GameObject.Instantiate(prefab, new Vector3(spawnX, spawnY[Random.Range(0, 4)], 0f), Quaternion.identity, transform);
            cdTimer = spawnCooldown;
        }
    }


}
