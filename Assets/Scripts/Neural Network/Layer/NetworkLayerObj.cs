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
        [HideInInspector] public List<NeuronObj> neurons = new();
        
        public Action<NetworkLayerObj> OnDelete;
        public Action<NeuronObj> OnNeuronCreated;
        
        public void DeleteLayer()
        {
            OnDelete?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"{name} got deleted");
        }

        public abstract NetworkLayer Clone();

        #region Neuron

        public abstract void CreateNeuron();
        
        public void RemoveNeuron(NeuronObj neuronObj)
        {
            neuronObj.DeleteNeuron();
            neurons.Remove(neuronObj);
        }
        
        public List<NeuronObj> GetNeurons()
        {
            return neurons;
        }

        #endregion
    }

    public abstract class NetworkLayer
    {
        public List<Neuron> Neurons;
    }
}
