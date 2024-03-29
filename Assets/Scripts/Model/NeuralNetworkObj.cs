using System;
using System.Collections.Generic;
using System.Linq;
using Controller;
using Model.Connection;
using Model.Layer;
using Model.Neurons;
using UnityEditor;
using UnityEngine;

namespace Model
{
    public class NeuralNetworkObj : ScriptableObject
    {
        [HideInInspector] public List<NetworkLayerObj> layersObj = new();
        [HideInInspector] public List<ConnectionObj> connectionsObj = new();
        
        [Header("Best Network")]
        public Algorithm algorithm;
        public int generation;
        public float fitness;
        public float lifeTime;
        
        [Header("Avg in Last Generation")]
        public float avgFitness;
        public float avgLifeTime;

        public event Action<NetworkLayerObj> OnLayerCreated;
        public event Action<NeuronObj> OnConnectionCreated;
        public event Action<NeuralNetworkObj> OnDestroyed;

        #region Layer

        /// <summary>
        /// Create A Layer and add it to Asset
        /// Invoke OnLayerCreated Event
        /// </summary>
        /// <param name="type">Type</param>
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
        }

        /// <summary>
        /// Remove Layer and DeprecatedObjects from Asset and List
        /// </summary>
        /// <param name="networkLayerObj">NetworkLayerObj</param>
        public void RemoveLayer(NetworkLayerObj networkLayerObj)
        {
            RemoveDeprecatedObjects(networkLayerObj);
            networkLayerObj.DeleteLayer();
            layersObj.Remove(networkLayerObj);
        }

        /// <summary>
        /// Get Layers
        /// </summary>
        /// <returns>List NetworkLayerObj</returns>
        public List<NetworkLayerObj> GetLayers()
        {
            return layersObj;
        }

        /// <summary>
        /// Connect Layer and Neuron Events
        /// </summary>
        /// <param name="layerObj">NetworkLayerObj</param>
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

        /// <summary>
        /// Generate New GUIDs for NeuronObjs and LayerObjs
        /// </summary>
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

        /// <summary>
        /// Removes Deprecated Objects
        /// </summary>
        /// <param name="networkLayerObj">NetworkLayerObj</param>
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
        }

        #endregion

        #region Connections

        /// <summary>
        /// Get Connections
        /// </summary>
        /// <returns>List ConnectionObj</returns>
        public List<ConnectionObj> GetConnections()
        {
            return connectionsObj;
        }

        /// <summary>
        /// Create ConnectionObj and add it to List and Asset
        /// </summary>
        /// <param name="parent">NeuronObj</param>
        /// <param name="child">NeuronObj</param>
        private ConnectionObj CreateConnection(NeuronObj parent, NeuronObj child)
        {
            var connection = CreateInstance<ConnectionObj>();
            if (connection == null)
                return null;

            connection.name = "Connection";
            connection.AddParent(parent);
            connection.AddChild(child);
            connection.OnDeleted += con => connectionsObj.Remove(con);

            parent.connectionObjs.Add(connection);
            connectionsObj.Add(connection);

            AssetDatabase.AddObjectToAsset(connection, this);
            AssetDatabase.SaveAssets();

            return connection;
        }

        /// <summary>
        /// Remove Connection form Asset and List
        /// </summary>
        /// <param name="connectionObj">ConnectionObj</param>
        private void RemoveConnection(ConnectionObj connectionObj)
        {
            foreach (var networkLayerObj in layersObj)
            {
                networkLayerObj.RemoveConnectionValue(connectionObj);
            }
            
            connectionsObj.Remove(connectionObj);
            connectionObj.GetParent().connectionObjs.Remove(connectionObj);
            connectionObj.DeleteConnection();
        }

        /// <summary>
        /// Reconnect Neurons in Layer with next and previous Layer
        /// </summary>
        /// <param name="networkLayerObj">NetworkLayerObj</param>
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
        }

        /// <summary>
        /// Checks if Connections are missing and creates them.
        /// Invoke OnConnectionCreated Event
        /// </summary>
        /// <param name="neuronObj"></param>
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

        /// <summary>
        /// Creates a Multiple Connections between two layer
        /// </summary>
        /// <param name="parent">NetworkLayerObj</param>
        /// <param name="child">NetworkLayerObj</param>
        private void CreateConnections(NetworkLayerObj parent, NetworkLayerObj child)
        {
            var parentNeurons = parent.GetNeurons();
            var childNeurons = child.GetNeurons();

            foreach (var connection in from parentNeuron in parentNeurons from childNeuron in from neuron in childNeurons
                         let found = connectionsObj.Where(connection =>
                             connection.GetChild() == neuron && connection.GetParent() == parentNeuron)
                         where !found.Any()
                         select neuron select CreateConnection(parentNeuron, childNeuron))
            {
                parent.CreateConnectionValue(connection);
                child.CreateConnectionValue(connection);
            }
        }

        /// <summary>
        /// Removes Deprecated Objects
        /// </summary>
        /// <param name="neuronObj">NeuronObj</param>
        private void RemoveDeprecatedObjects(NeuronObj neuronObj)
        {
            var deprecatedConnections = connectionsObj.Where(connection =>
                connection.GetChild() == neuronObj || connection.GetParent() == neuronObj).ToList();

            for (var i = deprecatedConnections.Count - 1; i >= 0; i--)
            {
                RemoveConnection(deprecatedConnections[i]);
            }
        }

        #endregion

        #region General

        /// <summary>
        /// Called When Unity Deletes Object
        /// </summary>
        public void Delete()
        {
            OnDestroyed?.Invoke(this);
        }

        /// <summary>
        /// Saves Values of Neural Network in Scriptable Object
        /// </summary>
        /// <param name="network">NeuralNetwork</param>
        /// <returns>bool false == couldn't save</returns>
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
                    if (layersObj[i] is not HiddenLayerObj hiddenLayerObj) 
                        continue;
                    ((HiddenNeuronObj)hiddenLayerObj.neurons[j]).bias = network.Layers[i].bias[j];
                    hiddenLayerObj.UpdateNeuronValue((HiddenNeuronObj)hiddenLayerObj.neurons[j]);
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
            
            foreach (var networkLayerObj in layersObj)
            {
                foreach (var connectionObj in connectionsObj)
                {
                    networkLayerObj.UpdateConnectionValue(connectionObj);
                }
            }

            algorithm = network.Algorithm;
            fitness = network.Fitness;
            lifeTime = network.TimeAlive;
            generation = network.Generation;
            avgFitness = network.AverageFitnessInLastGeneration;
            avgLifeTime = network.AverageLifeTimeInLastGeneration;
            
            return true;
        }

        #endregion
    }
}