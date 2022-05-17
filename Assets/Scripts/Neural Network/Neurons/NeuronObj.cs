using System;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Neurons
{
    public abstract class NeuronObj : ScriptableObject
    {
        [HideInInspector] public Vector2 neuronPosition;
        [HideInInspector] public string guid;
        public Action<NeuronObj> OnDelete;
        
        public float value;

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
        public float Value;
    }
}
