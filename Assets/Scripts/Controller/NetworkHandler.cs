using System.Collections.Generic;
using System.Linq;
using Neural_Network;
using UnityEngine;

namespace Controller
{
    public class NetworkHandler : MonoBehaviour
    {
        [SerializeField] private NeuralNetworkObj reference;

        [SerializeField] private  int populationSize = 20;
        [SerializeField] private  GameObject learnerPrefab;
        [SerializeField] private  int generation;

        [SerializeField, Range(0f, 1f)] private float mutationChance = 0.01f;

        [SerializeField, Range(0f, 1f)] private float mutationStrength = 0.5f;

        private NeuralNetwork bestNet;
        private List<NeuralNetwork> networks;
        private readonly List<Learner> learners = new();

        #region Unity Methods

        private void Start()
        {
            InitNetworks();
        }
        
        private void FixedUpdate()
        {
            var anyAlive = false;
            // Check if not null and count > 0
            if (learners is not { Count: > 0 }) 
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
                var net = reference.Clone();
                networks.Add(net);
            }

            bestNet = networks[0];
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
                bestNet = lastNetwork;
                reference.Save(bestNet);
            }

            // Set Networks to best one and Mutate it.
            for (var i = 0; i < networks.Count; i++)
            {
                networks[i] = bestNet;
                networks[i].Mutate(mutationChance, mutationStrength);
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
                learners.Add(learner);
                learner.name = "Learner " + i + " Generation " + generation;
                learner.transform.parent = gameObject.transform;
            }
        }

        #endregion
    }
}