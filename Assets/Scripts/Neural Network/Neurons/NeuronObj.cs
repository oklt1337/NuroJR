using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Neurons
{
    public abstract class NeuronObj : ScriptableObject
    {
        [HideInInspector] public Vector2 neuronPosition;
        [HideInInspector] public string guid;
        
        public List<ConnectionObj> connectionObjs = new();

        public Action<NeuronObj> OnDelete;

        public abstract Neuron Clone();
        
        public void DeleteNeuron()
        {
            OnDelete?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"{name} got deleted");
        }
    }

    public abstract class Neuron
    {
        public NeuronObj NeuronObj;
        
        public List<Connection> Connections;
        public abstract float GetValue(Neuron neuron);
    }
}
