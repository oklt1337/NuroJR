using System;
using System.Collections.Generic;
using System.Linq;
using Neural_Network.Layer;
using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network
{
    #region Object

    public class NeuralNetworkObj : ScriptableObject
    {
        public List<NetworkLayerObj> layersObj = new();
        public List<ConnectionObj> connectionsObj = new();
        public float fitness;

        public Action<NetworkLayerObj> OnLayerCreated;
        public Action<NeuronObj> OnConnectionCreated;

        #region Layer

        public void CreateLayer(Type type)
        {
            // Make sure not to many Layers can be added.
            if (layersObj.Count >= 8)
                return;

            var layer = CreateInstance(type) as NetworkLayerObj;
            if (layer == null)
                return;

            layer.name = type.Name;
            layer.guid = GUID.Generate().ToString();

            layersObj.Add(layer);
            layersObj.Sort(new LayerComparer());

            AssetDatabase.AddObjectToAsset(layer, this);
            AssetDatabase.SaveAssets();

            ConnectEvents(layer);

            OnLayerCreated?.Invoke(layer);

            layer.CreateNeuron();
            Debug.Log($"Layer has been Created: {layer.name}");
        }

        public void RemoveLayer(NetworkLayerObj networkLayerObj)
        {
            RemoveDeprecatedObjects(networkLayerObj);
            networkLayerObj.DeleteLayer();
            layersObj.Remove(networkLayerObj);
        }

        public List<NetworkLayerObj> GetLayer()
        {
            return layersObj;
        }

        public void ConnectEvents(NetworkLayerObj layerObj)
        {
            layerObj.OnNeuronCreated += CheckConnections;
            layerObj.OnNeuronCreated += neuron => neuron.OnDelete += RemoveDeprecatedObjects;
            layerObj.OnDelete += ReconnectLayer;
        }

        private void RemoveDeprecatedObjects(NetworkLayerObj networkLayerObj)
        {
            var deprecatedNeurons = networkLayerObj.GetNeurons();
            foreach (var deprecatedNeuron in deprecatedNeurons)
            {
                RemoveDeprecatedObjects(deprecatedNeuron);
            }

            for (var i = deprecatedNeurons.Count - 1; i >= 0; i--)
            {
                networkLayerObj.RemoveNeuron(deprecatedNeurons[i]);
            }

            Debug.Log("Deprecated Neurons and Connections got deleted.");
        }

        #endregion

        #region Connections

        public List<ConnectionObj> GetConnections()
        {
            return connectionsObj;
        }

        private void CreateConnection(NeuronObj parent, NeuronObj child)
        {
            var connection = CreateInstance<ConnectionObj>();
            if (connection == null)
                return;

            connection.name = "Connection";
            connection.AddParent(parent);
            connection.AddChild(child);

            AssetDatabase.AddObjectToAsset(connection, this);
            AssetDatabase.SaveAssets();

            parent.connectionObjs.Add(connection);
            connectionsObj.Add(connection);
        }

        private void RemoveConnection(ConnectionObj connectionObj)
        {
            connectionsObj.Remove(connectionObj);
            connectionObj.GetParent().connectionObjs.Remove(connectionObj);
            connectionObj.DeleteConnection();
        }

        private void ReconnectLayer(NetworkLayerObj networkLayerObj)
        {
            var index = layersObj.FindIndex(x => x == networkLayerObj);
            if (index == -1)
                return;

            layersObj.Remove(networkLayerObj);
            
            if (index - 1 < 0 || index >= layersObj.Count)
                return;

            foreach (var neuronObj in layersObj[index].neurons)
            {
                CheckConnections(neuronObj);
            }

            Debug.Log($"{layersObj[index - 1].name} and {layersObj[index].name} got reconnected.");
        }

        private void CheckConnections(NeuronObj neuronObj)
        {
            // get layer of neuron
            var layerOfNeuron =
                (from layer in layersObj
                    let neurons = layer.GetNeurons()
                    where neurons.Contains(neuronObj)
                    select layer)
                .FirstOrDefault();

            // null if no layer found
            if (layerOfNeuron == null)
                return;

            //check if is 1st neuron of layer
            if (layerOfNeuron.GetNeurons().Count == 1)
            {
                var index = layersObj.FindIndex(x => x == layerOfNeuron);
                if (index == -1)
                    return;

                // make sure there is a layer
                if (index - 1 < 0)
                    return;

                // get neurons that are the parents
                var parentNeurons = layersObj[index - 1].GetNeurons();

                // check if parent is in list and Delete Connection form asset
                var connectionsToRemove = connectionsObj.Where(connection =>
                    parentNeurons.Contains(connection.GetParent())).ToList();

                for (var i = connectionsToRemove.Count - 1; i >= 0; i--)
                {
                    RemoveConnection(connectionsToRemove[i]);
                }

                // create new connections 
                CreateConnections(layersObj[index - 1], layersObj[index]);

                if (index + 1 < layersObj.Count)
                    CreateConnections(layersObj[index], layersObj[index + 1]);
            }
            else
            {
                // find layer index
                var index = layersObj.FindIndex(x => x == layerOfNeuron);
                if (index == -1)
                    return;

                // check if layer is not first
                if (index - 1 >= 0)
                    CreateConnections(layersObj[index - 1], layersObj[index]);

                // check if layer is not last
                if (index + 1 < layersObj.Count)
                    CreateConnections(layersObj[index], layersObj[index + 1]);
            }
            OnConnectionCreated?.Invoke(neuronObj);
        }

        private void CreateConnections(NetworkLayerObj parent, NetworkLayerObj child)
        {
            var parentNeurons = parent.GetNeurons();
            var childNeurons = child.GetNeurons();

            foreach (var parentNeuron in parentNeurons)
            {
                // check if connection already exists if not create it
                foreach (var childNeuron in from neuron in childNeurons
                         let found = connectionsObj.Where(connection =>
                             connection.GetChild() == neuron && connection.GetParent() == parentNeuron)
                         where !found.Any()
                         select neuron)
                {
                    CreateConnection(parentNeuron, childNeuron);
                }
            }
        }

        private void RemoveDeprecatedObjects(NeuronObj neuronObj)
        {
            var deprecatedConnections = connectionsObj.Where(connection =>
                connection.GetChild() == neuronObj || connection.GetParent() == neuronObj).ToList();

            for (var i = deprecatedConnections.Count - 1; i >= 0; i--)
            {
                RemoveConnection(deprecatedConnections[i]);
            }

            Debug.Log("Deprecated Connections got deleted.");
        }

        #endregion

        #region General

        public NeuralNetwork Clone(bool random)
        {
            var network = new NeuralNetwork();
            layersObj.ForEach(x => network.Layers.Add(x.Clone(random)));
            return network;
        }

        public void Save(NeuralNetwork network)
        {
            fitness = network.Fitness;

            // Saving bias
            for (var i = 0; i < layersObj.Count; i++)
            {
                for (var j = 0; j < layersObj[i].neurons.Count; j++)
                {
                    if (layersObj[i].neurons[j] is HiddenNeuronObj hiddenNeuronObj)
                    {
                        hiddenNeuronObj.bias = (network.Layers[i].Neurons[j] as HiddenNeuron)!.Bias;
                    }
                }
            }

            // Save Weights
            foreach (var layer in network.Layers)
            {
                foreach (var neuron in layer.Neurons)
                {
                    foreach (var connection in neuron.Connections)
                    {
                        foreach (var connectionObj in connectionsObj.Where(connectionObj =>
                                     connectionObj.GetChild() == connection.ChildObj &&
                                     connectionObj.GetParent() == connection.ChildObj))
                        {
                            connectionObj.weight = connection.Weight;
                        }
                    }
                }
            }
        }

        #endregion
    }

    #endregion

    #region Neural Network

    public class NeuralNetwork : IComparable<NeuralNetwork>
    {
        public readonly List<NetworkLayer> Layers = new();
        public float Fitness;

        public string Name { get; set; }

        /// <summary>
        /// Generate Outputs
        /// </summary>
        /// <returns>float[] Outputs</returns>
        public float[] FeedForward()
        {
            var inputLayer = Layers[0] as InputLayer;
            var outputLayer = Layers.Last() as OutputLayer;
            var hiddenLayers = new List<HiddenLayer>();
            for (var i = 1; i < Layers.Count - 1; i++)
            {
                hiddenLayers.Add(Layers[i] as HiddenLayer);
            }

            if (outputLayer == null || inputLayer == null)
            {
                Debug.Log("output or input layer == null");
                return null;
            }


            var output = new List<float>();
            for (var i = 0; i < hiddenLayers.Count; i++)
            {
                if (i == 0)
                {
                    foreach (var hiddenNeuron in hiddenLayers[0].Neurons.Select(neuron => neuron as HiddenNeuron))
                    {
                        hiddenNeuron?.SumInputs(inputLayer.Neurons);
                    }
                }
                else
                {
                    foreach (var hiddenNeuron in hiddenLayers[i].Neurons.Select(neuron => neuron as HiddenNeuron))
                    {
                        hiddenNeuron?.SumInputs(hiddenLayers[i - 1].Neurons);
                    }
                }
            }

            foreach (var outputNeuron in outputLayer.Neurons.Select(neuron => neuron as OutputNeuron))
            {
                outputNeuron?.SumInputs(hiddenLayers.Last().Neurons);
            }

            output.AddRange(outputLayer.Neurons.Select(neuron => (neuron as OutputNeuron)!.GetValue()));
            if (!output.Any())
                Debug.Log("outputs are null");
            
            return output.ToArray();
        }

        public void Mutate(float mutationChance, float mutationStrength)
        {
            // Mutate Bias
            foreach (var neuron in Layers.SelectMany(layer => layer.Neurons))
            {
                if (neuron is not HiddenNeuron hiddenNeuron)
                    continue;

                if (UnityEngine.Random.Range(0f, 1f) < mutationChance)
                {
                    hiddenNeuron.Bias += UnityEngine.Random.Range(-mutationStrength, mutationStrength);
                }
            }

            // Mutate weight
            foreach (var connection in from layer in Layers
                     from neuron in layer.Neurons
                     where neuron.Connections != null
                     from connection in neuron.Connections
                     where connection != null
                     where UnityEngine.Random.Range(0f, 1f) < mutationChance
                     select connection)
            {
                connection.Weight += UnityEngine.Random.Range(-mutationStrength, mutationStrength);
            }
        }

        public int CompareTo(NeuralNetwork other)
        {
            if (other == null)
                return 1;

            if (Fitness > other.Fitness)
                return 1;
            if (Fitness < other.Fitness)
                return -1;
            return 0;
        }
    }

    #endregion
}