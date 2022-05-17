using Neural_Network;
using UnityEngine;

namespace Controller
{
    public class Learner : MonoBehaviour
    {
        public NeuralNetwork network;
        public float[] input;
        public float[] output;
        public float fitness;
        public bool alive = true;

        public float[] Think(float[] input)
        {
            this.input = input;
            //output = network.FeedForward(input);
            return output;
        }

        public void SetFitness()
        {
            //network.fitness = fitness;
        }
    }
}