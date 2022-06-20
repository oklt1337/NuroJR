using System;
using System.Collections.Generic;
using System.Linq;
using Model;
using Model.Layer;
using Model.Neurons;
using UnityEngine;

namespace Controller
{
    public enum Algorithm
    {
        Sigmoid,
        TanH,
        ReLu
    }

    [Serializable]
    public class NeuralNetwork : IComparable<NeuralNetwork>
    {
        [SerializeField] private Algorithm algorithm = Algorithm.Sigmoid;
        [SerializeField] private float fitness;
        [SerializeField] private float averageFitnessInLastGeneration;
        [SerializeField] private int generation;
        [SerializeField] private float timeAlive;
        [SerializeField] private float averageLifeTimeInLastGeneration;
        [SerializeField] private List<NetworkLayer> layers = new();

        #region Properties

        public Algorithm Algorithm
        {
            get => algorithm;
            set => algorithm = value;
        }

        public string Name { get; set; }

        public float AverageFitnessInLastGeneration
        {
            get => averageFitnessInLastGeneration;
            set => averageFitnessInLastGeneration = value;
        }

        public float Fitness
        {
            get => fitness;
            set => fitness = value;
        }

        public int Generation
        {
            get => generation;
            set => generation = value;
        }

        public float TimeAlive
        {
            get => timeAlive;
            set => timeAlive = value;
        }

        public float AverageLifeTimeInLastGeneration
        {
            get => averageLifeTimeInLastGeneration;
            set => averageLifeTimeInLastGeneration = value;
        }

        public List<NetworkLayer> Layers => layers;
        public List<float[,]> Weights { get; } = new();

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialize Neural Network form Reference Object
        /// </summary>
        /// <param name="networkObj">NeuralNetworkObj</param>
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
                    for (var j = 0; j < layer.neurons.Length; j++)
                    {
                        layer.bias[j] = 1;
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

        /// <summary>
        /// Set input of Neural Network
        /// </summary>
        /// <param name="inputs">IEnumerable float</param>
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
            ResetValues();

            for (var i = 0; i < layers.Count; i++)
            {
                if (i + 1 < layers.Count)
                {
                    SetInputs(i, GenerateInputs(i));
                }
            }

            var output = layers.Last().neurons;
            return output;
        }

        /// <summary>
        /// Mutate the Network
        /// </summary>
        /// <param name="mutationChance">float</param>
        /// <param name="mutationStrength">float</param>
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

        /// <summary>
        /// Copy Neural Network Values to this one.
        /// </summary>
        /// <param name="neuralNetwork">NeuralNetwork</param>
        public void Copy(NeuralNetwork neuralNetwork)
        {
            Name = neuralNetwork.Name;
            fitness = neuralNetwork.fitness;
            generation = neuralNetwork.generation;
            timeAlive = neuralNetwork.timeAlive;

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

            for (var i = 0; i < neuralNetwork.Layers.Count; i++)
            {
                for (var j = 0; j < neuralNetwork.Layers[i].bias.Length; j++)
                {
                    layers[i].bias[j] = neuralNetwork.Layers[i].bias[j];
                }
            }
        }

        /// <summary>
        /// Load Values from Reference
        /// </summary>
        /// <param name="networkObj">NeuralNetworkObj</param>
        public void Load(NeuralNetworkObj networkObj)
        {
            Initialize(networkObj);
            fitness = networkObj.fitness;
            algorithm = networkObj.algorithm;

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

        /// <summary>
        /// Compare Neural Network
        /// </summary>
        /// <param name="other">NeuralNetwork</param>
        /// <returns>int</returns>
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Reset Values of Neural Network to 0
        /// </summary>
        private void ResetValues()
        {
            for (var i = 1; i < layers.Count; i++)
            {
                for (var j = 0; j < layers[i].neurons.Length; j++)
                {
                    layers[i].neurons[j] = 0;
                }
            }
        }

        /// <summary>
        /// Generate Inputs for Next Layer
        /// </summary>
        /// <param name="i">int</param>
        /// <returns>float[]</returns>
        private float[] GenerateInputs(int i)
        {
            var input = new float[layers[i + 1].neurons.Length];

            for (var j = 0; j < layers[i].neurons.Length; j++)
            {
                for (var k = 0; k < layers[i + 1].neurons.Length; k++)
                {
                    if (i == 0)
                    {
                        input[k] += Weights[i][j, k] * layers[i].neurons[j];
                    }
                    else
                    {
                        input[k] += ActivationFunction(Weights[i][j, k] * layers[i].neurons[j] +
                                                       layers[i].bias[j]);
                    }
                }
            }

            return input;
        }

        /// <summary>
        /// Set Input to Layer
        /// </summary>
        /// <param name="i">int</param>
        /// <param name="input">IReadOnlyList float</param>
        private void SetInputs(int i, IReadOnlyList<float> input)
        {
            for (var j = 0; j < layers[i + 1].neurons.Length; j++)
            {
                layers[i + 1].neurons[j] = input[j];
            }
        }

        /// <summary>
        /// Use ActivationFunction on values
        /// </summary>
        /// <param name="value">float</param>
        /// <returns>float</returns>
        /// <exception cref="ArgumentOutOfRangeException">algorithm == null</exception>
        private float ActivationFunction(float value)
        {
            return algorithm switch
            {
                Algorithm.Sigmoid => Sigmoid(value),
                Algorithm.TanH => TanH(value),
                Algorithm.ReLu => ReLu(value),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        #region ActivationFunctions

        /// <summary>
        /// Sigmoid function
        /// </summary>
        /// <param name="value">float</param>
        /// <returns>float</returns>
        private float Sigmoid(float value)
        {
            return (float)(1.0 / (1.0 + Math.Pow(Math.E, -value)));
        }

        /// <summary>
        /// TanH function
        /// </summary>
        /// <param name="value">float</param>
        /// <returns>float</returns>
        private float TanH(float value)
        {
            return (float)Math.Tanh(value);
        }

        /// <summary>
        /// ReLu function
        /// </summary>
        /// <param name="value">float</param>
        /// <returns>float</returns>
        private float ReLu(float value)
        {
            return Math.Max(0, value);
        }

        #endregion

        /// <summary>
        /// Get Float between two floats
        /// </summary>
        /// <param name="min">float</param>
        /// <param name="max">float</param>
        /// <returns>float</returns>
        private static float NextFloat(float min, float max)
        {
            var random = new System.Random();
            var val = random.NextDouble() * (max - min) + min;
            return (float)val;
        }

        #endregion

        [Serializable]
        public struct NetworkLayer
        {
            public float[] neurons;
            public float[] bias;
        }
    }
}