using System;
using System.Collections.Generic;
using System.Linq;

namespace Neural_Network
{
    [Serializable]
    public class TestNeuralNetwork : IComparable<TestNeuralNetwork>
    {
        public float fitness;
        public List<TestLayer> testLayers = new();
        public List<float[,]> Weights = new();
        
        public string Name { get; set; }

        public void Initialize(NeuralNetworkObj networkObj)
        {
            // initialize 
            for (var i = 0; i < networkObj.layersObj.Count; i++)
            {
                var layer = new TestLayer
                {
                    neurons = new float[networkObj.layersObj[i].neurons.Count],
                    bias = new float[networkObj.layersObj[i].neurons.Count]
                };
                testLayers.Add(layer);

                for (var j = 0; j < testLayers[i].neurons.Length; j++)
                {
                    testLayers[i].neurons[j] = 0;
                }
                
                for (var j = 1; j < testLayers[i].neurons.Length; j++)
                {
                    testLayers[i].bias[j] = 1;
                }
            }

            for (var i = 0; i < testLayers.Count; i++)
            {
                if (i + 1 < testLayers.Count)
                {
                    Weights.Add(new float[testLayers[i].neurons.Length, testLayers[i + 1].neurons.Length]);
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

        public void SetInputs(IEnumerable<float> inputs)
        {
            foreach (var input in inputs)
            {
                for (var i = 0; i < testLayers[0].neurons.Length; i++)
                {
                    testLayers[0].neurons[i] = input;
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
            for (var i = 1; i < testLayers.Count; i++)
            {
                for (var j = 0; j < testLayers[i].neurons.Length; j++)
                {
                    testLayers[i].neurons[j] = 0;
                }
            }

            for (var i = 0; i < testLayers.Count; i++)
            {
                if (i + 1 < testLayers.Count)
                {
                    var input = new float[testLayers[i].neurons.Length];
                    for (var j = 0; j < testLayers[i].neurons.Length; j++)
                    {
                        for (var k = 0; k < testLayers[i + 1].neurons.Length; k++)
                        {
                            if (i == 0)
                            {
                                input[j] += Weights[i][j, k] * testLayers[i].neurons[j];
                            }
                            else
                            {
                                input[j] += ActivationFunction(Weights[i][j, k] * testLayers[i].neurons[j] + testLayers[i].bias[j]);
                            }
                        }
                    }

                    for (var j = 0; j < testLayers[i + 1].neurons.Length; j++)
                    {
                        testLayers[i + 1].neurons[j] = input[j];
                    }
                }
            }

            var output = testLayers.Last().neurons;

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

        private static float NextFloat(float min, float max)
        {
            var random = new System.Random();
            var val = random.NextDouble() * (max - min) + min;
            return (float)val;
        }

        public void Mutate(float mutationChance, float mutationStrength)
        {
            // Mutate Bias
            for (var i = 1; i < testLayers.Count; i++)
            {
                for (var j = 0; j < testLayers[i].bias.Length; j++)
                {
                    if(UnityEngine.Random.Range(0f, 1f) < mutationChance)
                    {
                        testLayers[i].bias[j] += UnityEngine.Random.Range(-mutationStrength, mutationStrength);
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

        public int CompareTo(TestNeuralNetwork other)
        {
            if (other == null)
                return 1;

            if (fitness > other.fitness)
                return 1;
            if (fitness < other.fitness)
                return -1;
            return 0;
        }

        public void Copy(TestNeuralNetwork testNeuralNetwork)
        {
            Name = testNeuralNetwork.Name;
            fitness = testNeuralNetwork.fitness;
            
            for (var i = 0; i < testNeuralNetwork.Weights.Count; i++)
            {
                for (var j = 0; j < testNeuralNetwork.Weights[i].GetLength(0); j++)
                {
                    for (var k = 0; k < testNeuralNetwork.Weights[i].GetLength(1); k++)
                    {
                        Weights[i][j, k] = testNeuralNetwork.Weights[i][j, k];
                    }
                }
            }
        }
    }

    [Serializable]
    public struct TestLayer
    {
        public float[] neurons;
        public float[] bias;
    }
}