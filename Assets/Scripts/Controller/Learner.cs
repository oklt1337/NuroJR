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
        [SerializeField] private SpriteRenderer renderer;

        public TestNeuralNetwork Network;
        
        #region Properties

        //public TestNeuralNetwork Network { get; set; }

        public float Fitness
        {
            get => fitness;
            set => fitness = value;
        }

        public bool Alive
        {
            get => alive;
            set
            {
                alive = value;
                if (value == false)
                {
                    renderer.color = Color.red;
                }
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

            /*foreach (var layer in Network.Layers)
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
            }*/

            Network.SetInputs(input);
            return Network.FeedForward();
        }

        /// <summary>
        /// Set Fitness of Learner to Network.
        /// </summary>
        public void SetFitness()
        {
            Network.fitness = fitness;
        }

        private float[] NormalizeInputs(float[] input)
        {
            var sum = input.Sum();
            if (sum != 0)
            {
                for (var i = 0; i < input.Length; i++)
                {
                    input[i] /= sum;
                }
            }
            
            return input;
        }

        #endregion
    }
}