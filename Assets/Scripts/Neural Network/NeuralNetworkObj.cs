using System;
using System.Collections.Generic;
using System.Linq;
using Neural_Network.Connection;
using Neural_Network.Layer;
using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network
{
    public class NeuralNetworkObj : ScriptableObject
    {
        [HideInInspector] public List<NetworkLayerObj> layersObj = new();
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
            layer.GenerateNewGuid();

            layersObj.Add(layer);
            layersObj.Sort(new LayerComparer());
            ConnectEvents(layer);

            AssetDatabase.AddObjectToAsset(layer, this);
            AssetDatabase.SaveAssets();

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

            foreach (var neuron in layerObj.neurons)
            {
                neuron.OnDelete += RemoveDeprecatedObjects;
            }
        }

        public void GenerateNewGuids()
        {
            foreach (var layerObj in layersObj)
            {
                foreach (var neuronObj in layerObj.neurons)
                {
                    neuronObj.GenerateNewGuid();
                }

                layerObj.GenerateNewGuid();
            }
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
            connection.OnDeleted += con => connectionsObj.Remove(con);

            parent.connectionObjs.Add(connection);
            connectionsObj.Add(connection);

            AssetDatabase.AddObjectToAsset(connection, this);
            AssetDatabase.SaveAssets();
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

        public bool Save(NeuralNetwork network)
        {
            if (network.Layers.Count != layersObj.Count)
            {
                Debug.Log("Layer count doesnt match.");
                return false;
            }

            for (var i = 0; i < layersObj.Count; i++)
            {
                if (network.Layers[i].neurons.Length != layersObj[i].neurons.Count)
                {
                    Debug.Log($"Neuron count doesnt match in layer {layersObj[i].name} at index {i}.");
                    return false;
                }

                for (var j = 0; j < layersObj[i].neurons.Count; j++)
                {
                    if (layersObj[i] is HiddenLayerObj)
                    {
                        ((HiddenNeuronObj)layersObj[i].neurons[j]).bias = network.Layers[i].bias[j];
                    }
                }
            }
            
            foreach (var connectionObj in connectionsObj)
            {
                for (var i = 0; i < layersObj.Count; i++)
                {
                    var index = layersObj[i].neurons.FindIndex(x => x == connectionObj.parent);
                    if (index == -1) continue;
                    {
                        var index1 = layersObj[i + 1].neurons.FindIndex(x => x == connectionObj.child);
                        if (index1 != -1)
                        {
                            connectionObj.weight = network.Weights[i][index, index1];
                        }
                    }
                }
            }

            fitness = network.Fitness;
            return true;
        }

        #endregion
    }
}