using System;
using System.Collections.Generic;
using System.Linq;
using Neural_Network.Layer;
using Neural_Network.Neurons;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Neural_Network
{
    [CreateAssetMenu]
    public class NeuralNetwork : ScriptableObject
    {
        public List<NetworkLayer> layers = new();
        [TableList] public List<Connection> connections = new();

        public Action<NetworkLayer> OnLayerCreated;
        public Action<Connection> OnConnectionCreated;

        #region Layer

        [Button]
        public void CreateInputLayer()
        {
            CreateLayer(typeof(InputLayer));
        }

        [Button]
        public void CreateHiddenLayer()
        {
            CreateLayer(typeof(HiddenLayer));
        }

        [Button]
        public void CreateOutputLayer()
        {
            CreateLayer(typeof(OutputLayer));
        }

        public void CreateLayer(Type type)
        {
            // Make sure not to many Layers can be added.
            if (layers.Count >= 8)
                return;

            var layer = CreateInstance(type) as NetworkLayer;
            if (layer == null)
                return;

            layer.name = type.Name;
            layer.guid = GUID.Generate().ToString();

            layers.Add(layer);
            layers.Sort(new LayerComparer());

            AssetDatabase.AddObjectToAsset(layer, this);
            AssetDatabase.SaveAssets();

            layer.OnNeuronCreated += CheckConnections;
            layer.OnNeuronCreated += neuron => neuron.OnDelete += RemoveDeprecatedObjects;
            layer.OnDelete += ReconnectLayer;
            layer.CreateNeuron();

            OnLayerCreated?.Invoke(layer);
            Debug.Log($"Layer has been Created: {layer.name}");
        }

        [Button]
        public void RemoveLayer(NetworkLayer networkLayer)
        {
            RemoveDeprecatedObjects(networkLayer);
            networkLayer.DeleteLayer();
            layers.Remove(networkLayer);
        }

        public List<NetworkLayer> GetLayer()
        {
            return layers;
        }

        private void RemoveDeprecatedObjects(NetworkLayer networkLayer)
        {
            var deprecatedNeurons = networkLayer.GetNeurons();
            foreach (var deprecatedNeuron in deprecatedNeurons)
            {
                RemoveDeprecatedObjects(deprecatedNeuron);
            }
            
            for (var i = deprecatedNeurons.Count - 1; i >= 0; i--)
            {
                networkLayer.RemoveNeuron(deprecatedNeurons[i]);
            }

            Debug.Log("Deprecated Neurons and Connections got deleted.");
        }

        #endregion

        #region Connections

        public List<Connection> GetConnections()
        {
            return connections;
        }

        private void CreateConnection(Neuron parent, Neuron child)
        {
            var connection = CreateInstance<Connection>();
            if (connection == null)
                return;

            connection.guid = GUID.Generate().ToString();
            connection.name = "Connection";
            connection.AddParent(parent);
            connection.AddChild(child);

            AssetDatabase.AddObjectToAsset(connection, this);
            AssetDatabase.SaveAssets();

            connections.Add(connection);
            OnConnectionCreated?.Invoke(connection);
        }

        private void RemoveConnection(Connection connection)
        {
            connections.Remove(connection);
            connection.DeleteConnection();
        }

        private void ReconnectLayer(NetworkLayer networkLayer)
        {
            var index = layers.FindIndex(x => x == networkLayer);
            if (index == -1)
                return;

            if (index - 1 <= 0 || index + 1 >= layers.Count)
                return;

            CreateConnections(layers[index - 1], layers[index + 1]);
            Debug.Log($"{layers[index - 1].name} and {layers[index + 1].name} got reconnected.");
        }

        private void CheckConnections(Neuron neuron)
        {
            // get layer of neuron
            var layerOfNeuron =
                (from layer in layers let neurons = layer.GetNeurons() where neurons.Contains(neuron) select layer)
                .FirstOrDefault();

            // null if no layer found
            if (layerOfNeuron == null)
                return;

            //check if is 1st neuron of layer
            if (layerOfNeuron.GetNeurons().Count == 1)
            {
                var index = layers.FindIndex(x => x == layerOfNeuron);
                if (index == -1)
                    return;

                // make sure there is a layer
                if (index - 1 < 0)
                    return;

                // get neurons that are the parents
                var parentNeurons = layers[index - 1].GetNeurons();

                //TODO: InvalidOperationException: Collection was modified; enumeration operation may not execute.
                // check if parent is in list and Delete Connection form asset
                var connectionsToRemove = connections.Where(connection =>
                    parentNeurons.Contains(connection.GetParent())).ToList();

                for (var i = connectionsToRemove.Count - 1; i >= 0; i--)
                {
                    RemoveConnection(connectionsToRemove[i]);
                }

                // create new connections 
                CreateConnections(layers[index - 1], layers[index]);

                if (index + 1 >= layers.Count)
                    return;

                CreateConnections(layers[index], layers[index + 1]);
            }
            else
            {
                // find layer index
                var index = layers.FindIndex(x => x == layerOfNeuron);
                if (index == -1)
                    return;

                // check if layer is not first
                if (index - 1 >= 0)
                    CreateConnections(layers[index - 1], layers[index]);

                // check if layer is not last
                if (index + 1 >= layers.Count)
                    return;
                CreateConnections(layers[index], layers[index + 1]);
            }
        }

        private void CreateConnections(NetworkLayer parent, NetworkLayer child)
        {
            var parentNeurons = parent.GetNeurons();
            var childNeurons = child.GetNeurons();

            foreach (var parentNeuron in parentNeurons)
            {
                // check if connection already exists if not create it
                foreach (var childNeuron in from neuron in childNeurons
                         let found = connections.Where(connection =>
                             connection.GetChild() == neuron && connection.GetParent() == parentNeuron)
                         where !found.Any()
                         select neuron)
                {
                    CreateConnection(parentNeuron, childNeuron);
                }
            }
        }
        
        private void RemoveDeprecatedObjects(Neuron neuron)
        {
            var deprecatedConnections = connections.Where(connection => connection._child == neuron || connection._parent == neuron).ToList();

            for (var i = deprecatedConnections.Count - 1; i >= 0; i--)
            {
                RemoveConnection(deprecatedConnections[i]);
            }
            
            Debug.Log("Deprecated Connections got deleted.");
        }

        #endregion

        /*public NeuralNetwork Clone()
        {
            var network = Instantiate(this);
            //network.rootNeuron = network.rootNeuron.Clone();

            return network;
        }*/
    }
}