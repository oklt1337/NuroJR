using System.Linq;
using Neural_Network;
using UnityEngine;

namespace Controller
{
    public class Learner : MonoBehaviour
    {
        [SerializeField] private NeuralNetwork network;
        [SerializeField] private float fitness;
        [SerializeField] private float timeAlive;
        [SerializeField] private bool alive = true;
        
        #region Properties

        public NeuralNetwork Network
        {
            get => network;
            set => network = value;
        }

        public float Fitness
        {
            get => fitness;
            set => fitness = value;
        }
        
        public float TimeAlive
        {
            get => timeAlive;
            set => timeAlive = value;
        }

        public bool Alive
        {
            get => alive;
            set
            {
                if (value == false)
                    Network.TimeAlive = timeAlive;
                alive = value;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Generate Outputs
        /// Make sure input.Length == Neurons.Length of InputLayer
        /// </summary>
        /// <param name="input">float[] Inputs</param>
        /// <returns>float[] Outputs</returns>
        public float[] Think(float[] input)
        {
            input = NormalizeInputs(input);
            Network.SetInputs(input);
            return Network.FeedForward();
        }

        /// <summary>
        /// Set Fitness of Learner to Network.
        /// </summary>
        public void SetFitness()
        {
            Network.Fitness = fitness;
        }

        /// <summary>
        /// Normalize the inputs to be between 1 and 0
        /// </summary>
        /// <param name="input">float[]</param>
        /// <returns>normalized inputs float[]</returns>
        private static float[] NormalizeInputs(float[] input)
        {
            var sum = input.Sum();
            if (sum == 0) 
                return input;
            
            for (var i = 0; i < input.Length; i++)
            {
                input[i] /= sum;
            }
            return input;
        }

        #endregion
    }
}