using System;
using System.Collections.Generic;
using Model.Connection;
using Model.Neurons;
using UnityEditor;
using UnityEngine;

namespace Model.Layer
{
    public abstract class NetworkLayerObj : ScriptableObject
    {
        [HideInInspector] public string guid;
        [HideInInspector] public List<NeuronObj> neurons = new();

        public List<ConnectionValues> connectionValues = new();

        public Action<NetworkLayerObj> OnDelete;
        public Action<NeuronObj> OnNeuronCreated;
        
        /// <summary>
        /// Delete Layer from Asset
        /// Invoke OnDelete Event
        /// </summary>
        public void DeleteLayer()
        {
            OnDelete?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
        }

        #region Connection Values

        public void CreateConnectionValue(ConnectionObj connectionObj)
        {
            var obj = new ConnectionValues
            {
                weight = connectionObj.weight,
                connectionFromTo = string.Concat(connectionObj.parent.name, "_", connectionObj.child.name),
                connection = connectionObj
            };
            connectionValues.Add(obj);
        }

        public void UpdateConnectionValue(ConnectionObj connectionObj)
        {
            var index = connectionValues.FindIndex(x => x.connection == connectionObj);
            if (index == -1)
                return;
            
            var obj = connectionValues[index];
            obj.weight = connectionObj.weight;
            connectionValues[index] = obj;
        }

        public void RemoveConnectionValue(ConnectionObj connectionObj)
        {
            var index = connectionValues.FindIndex(x => x.connection == connectionObj);
            if (index == -1)
                return;
            
            connectionValues.RemoveAt(index);
        }

        #endregion

        #region Neuron

        /// <summary>
        /// Create a Neuron and Add it to Asset.
        /// </summary>
        public abstract void CreateNeuron();
        
        /// <summary>
        /// Remove Neuron From List and Asset.
        /// </summary>
        /// <param name="neuronObj">NeuronObj</param>
        public void RemoveNeuron(NeuronObj neuronObj)
        {
            if (neuronObj is HiddenNeuronObj hiddenNeuronObj)
            {
                (this as HiddenLayerObj)?.RemoveNeuronValue(hiddenNeuronObj);
            }
            
            neuronObj.DeleteNeuron();
            neurons.Remove(neuronObj);
        }
        
        /// <summary>
        /// Get List of Neurons in layer
        /// </summary>
        /// <returns>List NeuronObj</returns>
        public List<NeuronObj> GetNeurons()
        {
            return neurons;
        }

        /// <summary>
        /// Generate a GUID
        /// </summary>
        public void GenerateNewGuid()
        {
            guid = GUID.Generate().ToString();
        }

        #endregion
    }

    [Serializable]
    public struct ConnectionValues
    {
        [HideInInspector] public ConnectionObj connection;
        public string connectionFromTo;
        public float weight;
    }
}
