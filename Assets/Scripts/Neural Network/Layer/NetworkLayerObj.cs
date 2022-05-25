using System;
using System.Collections.Generic;
using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Layer
{
    public abstract class NetworkLayerObj : ScriptableObject
    {
        [HideInInspector] public string guid;
        public List<NeuronObj> neurons = new();
        
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
}
