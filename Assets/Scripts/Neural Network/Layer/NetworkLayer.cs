using System;
using System.Collections.Generic;
using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Layer
{
    public abstract class NetworkLayer : ScriptableObject
    {
        [HideInInspector] public string guid;
        public List<Neuron> neurons = new();
        
        public Action<NetworkLayer> OnDelete;
        public Action<Neuron> OnNeuronCreated;
        
        public void DeleteLayer()
        {
            OnDelete?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"{name} got deleted");
        }

        #region Neuron

        public abstract void CreateNeuron();
        
        public void RemoveNeuron(Neuron neuron)
        {
            neuron.DeleteNeuron();
            neurons.Remove(neuron);
        }
        
        public List<Neuron> GetNeurons()
        {
            return neurons;
        }

        #endregion
    }
}
