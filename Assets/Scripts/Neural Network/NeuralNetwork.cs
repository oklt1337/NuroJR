using System;
using System.Collections.Generic;
using System.Linq;
using Neural_Network.Layer;
using Neural_Network.Neurons;
using UnityEngine;

namespace Neural_Network
{
    [Serializable]
    public class NeuralNetwork : IComparable<NeuralNetwork>
    {
        [SerializeField] private float fitness;
        [SerializeField] private List<NetworkLayer> layers = new();

        public string Name { get; set; }

        public float Fitness
        {
            get => fitness;
            set => fitness = value;
        }

        public List<NetworkLayer> Layers => layers;
        public List<float[,]> Weights { get; } = new();

        public void Initialize(NeuralNetworkObj networkObj)
        {
            // initialize 
            for (var i = 0; i < networkObj.layersObj.Count; i++)
            {
                var layer = new NetworkLayer
                {
                    neurons = new float[networkObj.layersObj[i].neurons.Count],
                    bias = new float[networkObj.layersObj[i].neurons.Count]
                };

                for (var j = 0; j < layer.neurons.Length; j++)
                {
                    layer.neurons[j] = 0;
                }

                if (i != 0 && i != networkObj.layersObj.Count - 1)
                {
                    {
                        for (var j = 0; j < layer.neurons.Length; j++)
                        {
                            layer.bias[j] = 1;
                        }
                    }
                }
                
                layers.Add(layer);
            }

            for (var i = 0; i < layers.Count; i++)
            {
                if (i + 1 < layers.Count)
                {
                    Weights.Add(new float[layers[i].neurons.Length, layers[i + 1].neurons.Length]);
                }
            }

            foreach (var floatArray in Weights)
            {
                for (var i = 0; i < floatArray.GetLength(0); i++)
                {
                    for (var j = 0; j < floatArray.GetLength(1); j++)
                    {
                        floatArray[i, j] = NextFloat(-1, 1);
                    }
                }
            }
        }

        public void Load(NeuralNetworkObj networkObj)
        {
            Initialize(networkObj);
            fitness = networkObj.fitness;
            
            for (var i = 0; i < networkObj.layersObj.Count; i++)
            {
                if (i == 0 || i == networkObj.layersObj.Count - 1) 
                    continue;
                
                for (var j = 0; j < networkObj.layersObj[i].neurons.Count; j++)
                {
                    if (networkObj.layersObj[i] is HiddenLayerObj)
                    {
                        ((HiddenNeuronObj)networkObj.layersObj[i].neurons[j]).bias = Layers[i].bias[j];
                    }
                }
            }
            
            foreach (var connectionObj in networkObj.connectionsObj)
            {
                for (var i = 0; i < networkObj.layersObj.Count; i++)
                {
                    var index = networkObj.layersObj[i].neurons.FindIndex(x => x == connectionObj.parent);
                    if (index == -1) continue;
                    {
                        var index1 = networkObj.layersObj[i + 1].neurons.FindIndex(x => x == connectionObj.child);
                        if (index1 != -1)
                        {
                            Weights[i][index, index1] = connectionObj.weight;
                        }
                    }
                }
            }
        }

        public void SetInputs(IEnumerable<float> inputs)
        {
            foreach (var input in inputs)
            {
                for (var i = 0; i < layers[0].neurons.Length; i++)
                {
                    layers[0].neurons[i] = input;
                }
            }
        }

        /// <summary>
        /// Generate Outputs
        /// </summary>
        /// <returns>float[] Outputs</returns>
        public float[] FeedForward()
        {
            // reset values
            for (var i = 1; i < layers.Count; i++)
            {
                for (var j = 0; j < layers[i].neurons.Length; j++)
                {
                    layers[i].neurons[j] = 0;
                }
            }

            for (var i = 0; i < layers.Count; i++)
            {
                if (i + 1 < layers.Count)
                {
                    var input = new float[layers[i].neurons.Length];
                    for (var j = 0; j < layers[i].neurons.Length; j++)
                    {
                        for (var k = 0; k < layers[i + 1].neurons.Length; k++)
                        {
                            if (i == 0)
                            {
                                input[j] += Weights[i][j, k] * layers[i].neurons[j];
                            }
                            else
                            {
                                input[j] += ActivationFunction(Weights[i][j, k] * layers[i].neurons[j] +
                                                               layers[i].bias[j]);
                            }
                        }
                    }

                    for (var j = 0; j < layers[i + 1].neurons.Length; j++)
                    {
                        layers[i + 1].neurons[j] = input[j];
                    }
                }
            }

            var output = layers.Last().neurons;

            return output;
        }

        private float ActivationFunction(float value)
        {
            return Sigmoid(value);
        }

        private float Sigmoid(float value)
        {
            return (float)(1.0 / (1.0 + Math.Pow(Math.E, -value)));
        }

        private float ReLu(float value)
        {
            return Math.Max(0, value);
        }

        private static float NextFloat(float min, float max)
        {
            var random = new System.Random();
            var val = random.NextDouble() * (max - min) + min;
            return (float)val;
        }

        public void Mutate(float mutationChance, float mutationStrength)
        {
            // Mutate Bias
            for (var i = 0; i < layers.Count; i++)
            {
                if (i == 0 || i == layers.Count - 1)
                    continue;

                for (var j = 0; j < layers[i].bias.Length; j++)
                {
                    if (UnityEngine.Random.Range(0f, 1f) < mutationChance)
                    {
                        layers[i].bias[j] += UnityEngine.Random.Range(-mutationStrength, mutationStrength);
                    }
                }
            }

            // Mutate Weight
            foreach (var weight in Weights)
            {
                for (var j = 0; j < weight.GetLength(0); j++)
                {
                    for (var k = 0; k < weight.GetLength(1); k++)
                    {
                        if (UnityEngine.Random.Range(0f, 1f) < mutationChance)
                        {
                            weight[j, k] += UnityEngine.Random.Range(-mutationStrength, mutationStrength);
                        }
                    }
                }
            }
        }

        public int CompareTo(NeuralNetwork other)
        {
            if (other == null)
                return 1;

            if (fitness > other.fitness)
                return 1;
            if (fitness < other.fitness)
                return -1;
            return 0;
        }

        public void Copy(NeuralNetwork neuralNetwork)
        {
            Name = neuralNetwork.Name;
            fitness = neuralNetwork.fitness;

            for (var i = 0; i < neuralNetwork.Weights.Count; i++)
            {
                for (var j = 0; j < neuralNetwork.Weights[i].GetLength(0); j++)
                {
                    for (var k = 0; k < neuralNetwork.Weights[i].GetLength(1); k++)
                    {
                        Weights[i][j, k] = neuralNetwork.Weights[i][j, k];
                    }
                }
            }
        }
    }

    [Serializable]
    public struct NetworkLayer
    {
        public float[] neurons;
        public float[] bias;
    }
}