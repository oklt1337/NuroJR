using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using UnityEngine;

namespace Controller
{
    
    
    public class NetworkHandler : MonoBehaviour
    {
        [SerializeField] private NeuralNetworkObj reference;
        [SerializeField] private Algorithm algorithm;

        [SerializeField, Range(1,250)] private int populationSize = 20;
        [SerializeField] private GameObject learnerPrefab;
        [SerializeField] private int generation;

        [SerializeField, Range(0f, 1f)] private float mutationChance = 0.01f;
        [SerializeField, Range(0f, 1f)] private float mutationStrength = 0.5f;

        private NeuralNetwork bestNet;
        private List<NeuralNetwork> networks;
        private readonly List<Learner> learners = new();

        public static event Action OnNewGenerationCreated;

        #region Unity Methods

        private void Start()
        {
            InitNetworks();
        }

        private void FixedUpdate()
        {
            var anyAlive = false;
            if (learners.Count == 0 || learners == null)
                return;

            // Check if any learner is still alive
            learners.ForEach(learner =>
            {
                if (learner.Alive)
                    anyAlive = true;
            });

            if (anyAlive)
                return;
            ClearLearners();
        }

        #endregion

        #region Private Methods
        
        /// <summary>
        /// Initialize the Networks and Creates the Leaner.
        /// </summary>
        private void InitNetworks()
        {
            networks = new List<NeuralNetwork>();
            for (var i = 0; i < populationSize; i++)
            {
                networks.Add(CreateNewNetwork(reference));
            }

            bestNet = CreateNewNetwork(reference);
            CreateLearners();
        }

        /// <summary>
        /// Saving Network to reference
        /// </summary>
        private void Save()
        {
            var fitnessSum = 0f;
            var timeSum = 0f;
                
            foreach (var learner in learners)
            {
                fitnessSum += learner.Fitness;
                timeSum += learner.TimeAlive;
            }

            fitnessSum /= learners.Count;
            timeSum /= learners.Count;
            bestNet.AverageFitnessInLastGeneration = fitnessSum;
            bestNet.AverageLifeTimeInLastGeneration = timeSum;
            
            if (!reference.Save(bestNet))
                Debug.Log("Failed To Save");
        }

        /// <summary>
        /// Load Network from Reference
        /// </summary>
        /// <returns>NeuralNetwork</returns>
        private NeuralNetwork LoadNetwork(NeuralNetworkObj networkObj)
        {
            var net = new NeuralNetwork();
            net.Load(networkObj);
            return net;
        }

        /// <summary>
        /// Create NewNetwork from Reference
        /// </summary>
        /// <returns>NeuralNetwork</returns>
        private NeuralNetwork CreateNewNetwork(NeuralNetworkObj networkObj)
        {
            var net = new NeuralNetwork();
            net.Initialize(networkObj);
            net.Algorithm = algorithm;
            return net;
        }


        /// <summary>
        /// Destroys Learner and Creates new once.
        /// </summary>
        private void ClearLearners()
        {
            // Not null and Count > 0
            if (learners is { Count: > 0 })
            {
                generation++;
                SortNetworks();

                foreach (var learner in learners)
                {
                    Destroy(learner.gameObject);
                }
            }

            learners.Clear();
            OnNewGenerationCreated?.Invoke();
            Invoke(nameof(CreateLearners), Time.fixedDeltaTime);
        }

        /// <summary>
        /// Sort Networks and check for best
        /// Creates new Mutations
        /// </summary>
        private void SortNetworks()
        {
            learners.ForEach(l => l.SetFitness());
            networks.Sort();

            // Check if new best Network
            var lastNetwork = networks.Last();
            if (bestNet.Fitness < lastNetwork.Fitness)
            {
                bestNet.Copy(lastNetwork);
                Save();
            }

            // Set Networks to best one and Mutate it.
            foreach (var network in networks)
            {
                network.Copy(bestNet);
                network.Mutate(mutationChance, mutationStrength);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Instantiate Learner
        /// </summary>
        public void CreateLearners()
        {
            for (var i = 0; i < populationSize; i++)
            {
                var learner = Instantiate(learnerPrefab, transform).GetComponent<Learner>();
                learner.Network = networks[i];
                learner.Network.Fitness = 0;
                learner.Network.TimeAlive = 0;
                learners.Add(learner);
                learner.name = "Learner " + i + " Generation " + generation;
                learner.Network.Name = learner.name;
                learner.Network.Generation = generation;
                learner.transform.parent = gameObject.transform;
            }
        }

        #endregion
    }
}