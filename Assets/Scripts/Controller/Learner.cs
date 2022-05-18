using System.Linq;
using Neural_Network;
using Neural_Network.Layer;
using Neural_Network.Neurons;
using UnityEngine;

namespace Controller
{
    public class Learner : MonoBehaviour
    {
        [SerializeField] private float fitness;
        [SerializeField] private bool alive = true;

        #region Properties

        public NeuralNetwork Network { get; set; }

        public float Fitness
        {
            get => fitness;
            set => fitness = value;
        }

        public bool Alive
        {
            get => alive;
            set => alive = value;
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

            foreach (var layer in Network.Layers)
            {
                // get inputLayer
                if (layer is not InputLayer inputLayer)
                    continue;

                // check if inputs match neurons in input layer
                if (inputLayer.Neurons.Count != input.Length)
                    return null;

                // Set input
                for (var i = 0; i < inputLayer.Neurons.Count; i++)
                {
                    if (inputLayer.Neurons[i] is InputNeuron inputNeuron)
                        inputNeuron.SetInput(input[i]);
                }
            }

            return Network.FeedForward();
        }

        /// <summary>
        /// Set Fitness of Learner to Network.
        /// </summary>
        public void SetFitness()
        {
            Network.Fitness = fitness;
        }

        private static float[] NormalizeInputs(float[] input)
        {
            var sum = input.Sum();
            for (var i = 0; i < input.Length; i++)
            {
                input[i] /= sum;
            }
            return input;
        }

        #endregion
    }
}