using System;
using System.Collections.Generic;
using Neural_Network;
using UnityEngine;

namespace Controller
{
    public class NetworkHandler : MonoBehaviour
    {
        public static NetworkHandler Instance;

        public NeuralNetworkObj reference;

        public string nameOfNetwork = "brain"; //This is for the playerPrefs so you can save multiple neural networks
        public int populationSize = 20;
        public GameObject learner;
        public int generation;
        public float bestFitness;

        [Range(0f, 1f)] public float MutationChance = 0.01f;

        [Range(0f, 1f)] public float MutationStrength = 0.5f;

        public NeuralNetwork BestNet;

        public List<NeuralNetwork> Networks;
        private List<Learner> learners = new List<Learner>();

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(Instance);
            }

            Instance = this;
        }

        private void Start()
        {
            InitNetworks();
        }

        private void InitNetworks()
        {
            Networks = new List<NeuralNetwork>();
            for (var i = 0; i < populationSize; i++)
            {
                var net = reference.Clone();
                Networks.Add(net);
            }
            BestNet = reference.Clone();
            bestFitness = BestNet.fitness = Networks[0].fitness;
            CreateLearners();
        }

        public void ClearLearners()
        {
            
        }

        public void CreateLearners()
        {
        }

        public void SortNetworks()
        {
        }

        public void FixedUpdate()
        {
        }
    }
}