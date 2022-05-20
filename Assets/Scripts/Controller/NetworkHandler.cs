using System;
using System.Collections.Generic;
using System.Linq;
using Neural_Network;
using UnityEngine;

namespace Controller
{
    public class NetworkHandler : MonoBehaviour
    {
        [SerializeField] private NeuralNetworkObj reference;
        [SerializeField] private bool randomInit;

        [SerializeField] private float timeScale;
        [SerializeField] private int populationSize = 20;
        [SerializeField] private GameObject learnerPrefab;
        [SerializeField] private int generation;

        [SerializeField, Range(0f, 1f)] private float mutationChance = 0.01f;
        [SerializeField, Range(0f, 1f)] private float mutationStrength = 0.5f;

        private TestNeuralNetwork bestNet;
        private List<TestNeuralNetwork> networks;
        private readonly List<Learner> learners = new();

        public static Action OnNewGeneration;

        #region Unity Methods

        private void Start()
        {
            Time.timeScale = timeScale;
            InitNetworks();
        }

        private void FixedUpdate()
        {
            var anyAlive = false;
            // Check if not null and count > 0
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
            networks = new List<TestNeuralNetwork>();
            for (var i = 0; i < populationSize; i++)
            {
                //var net = reference.Clone(randomInit);
                var testNet = new TestNeuralNetwork();
                testNet.Initialize(reference);
                networks.Add(testNet);
            }

            bestNet = new TestNeuralNetwork();
            bestNet.Initialize(reference);
            bestNet.Copy(networks[0]);
            CreateLearners();
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
            OnNewGeneration?.Invoke();
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
            if (bestNet.fitness < lastNetwork.fitness)
            {
                bestNet.Copy(lastNetwork);
                //reference.Save(bestNet);
                Debug.Log(bestNet.Name);
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
                learner.Network.fitness = 0;
                learners.Add(learner);
                learner.name = "Learner " + i + " Generation " + generation;
                learner.Network.Name = learner.name;
                learner.transform.parent = gameObject.transform;
            }
        }

        #endregion
    }
}