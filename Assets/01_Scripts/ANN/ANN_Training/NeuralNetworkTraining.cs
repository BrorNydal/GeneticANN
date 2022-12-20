using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using System;
using System.IO;


namespace ANN.Training
{
    [System.Serializable]
    public struct ScoreData
    {
        public float bonus;
        [Range(-1f, 1f)] public float scale;
        public ScoreCaluculation caluculation;
    }

    [System.Serializable]
    public enum ScoreCaluculation
    {
        Success,
        TimeAlive,
        DistanceX,
        DistanceY,
        DistanceZ
    }

    [RequireComponent(typeof(NeuralNetwork))]
    public class NeuralNetworkTraining : MonoBehaviour
    {
        public NeuralNetwork ANN { get { return ann; } }
        NeuralNetwork ann;

        public delegate void ANNReset();
        public event ANNReset OnAnnReset;

        public bool Finished { get { return finished; } }
        public float Score { get { return CalculateScore(); } }

        [SerializeField] ScoreData[] scoreCaluculations;

        bool finished = false;
        bool success = false;
        float timer = 0f;
        Vector2 startPosition;

        private void Awake()
        {
            ann = GetComponent<NeuralNetwork>();
            startPosition = transform.localPosition;
        }

        public void Fail()
        {
            if (!Finished)
            {
                finished = true;
                gameObject.SetActive(false);
            }
        }

        public void Success()
        {
            if (!Finished)
            {
                success = true;
                finished = true;
                gameObject.SetActive(false);
            }
        }

        private float CalculateScore()
        {
            float final = 0f;

            for(int i = 0; i < scoreCaluculations.Length; ++i)
            {
                switch (scoreCaluculations[i].caluculation)
                {
                    case ScoreCaluculation.Success:
                        if(success)
                            final += scoreCaluculations[i].bonus;
                        break;
                    case ScoreCaluculation.TimeAlive:
                        final += timer * scoreCaluculations[i].scale;
                        break;
                    case ScoreCaluculation.DistanceX:
                        final += transform.position.x * scoreCaluculations[i].scale + scoreCaluculations[i].bonus;
                        break;
                    case ScoreCaluculation.DistanceY:
                        final += transform.position.y * scoreCaluculations[i].scale + scoreCaluculations[i].bonus;
                        break;
                    case ScoreCaluculation.DistanceZ:
                        final += transform.position.z * scoreCaluculations[i].scale + scoreCaluculations[i].bonus;
                        break;
                }
            }

            return final;
        }

        public void ResetANN()
        {
            finished = false;
            timer = 0f;
            success = false;

            transform.localPosition = startPosition;
            transform.rotation = Quaternion.identity;

            Rigidbody2D rigid2D = GetComponent<Rigidbody2D>();
            if (rigid2D)
            {
                rigid2D.velocity = Vector2.zero;
            }

            if (OnAnnReset != null)
                OnAnnReset();
        }
    }
}
