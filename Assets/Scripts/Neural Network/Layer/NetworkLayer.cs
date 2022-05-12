using System;
using System.Collections.Generic;
using Neural_Network.Neurons;
using Neural_Network.Nodes;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Layer
{
    [InlineEditor(InlineEditorModes.FullEditor)]
    public abstract class NetworkLayer : ScriptableObject
    {
        [HideInInspector] public string guid;
        public List<Neuron> neurons = new();
        
        public Action<NetworkLayer> OnDelete;
        public Action<Neuron> OnNeuronCreated;

        [Button]
        public void DeleteLayer()
        {
            OnDelete?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
        }

        #region Neuron

        public abstract void CreateNeuron();
        
        public void RemoveNeuron(Neuron neuron)
        {
            neurons.Remove(neuron);
            neuron.DeleteNeuron();
        }
        
        public List<Neuron> GetNeurons()
        {
            return neurons;
        }

        #endregion
    }
}
