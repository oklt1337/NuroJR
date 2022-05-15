using System;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Neurons
{
    public class Neuron : ScriptableObject
    {
        [HideInInspector] public Vector2 neuronPosition;
        [HideInInspector] public string guid;
        public Action<Neuron> OnDelete;
        
        public float value;
        
        public void DeleteNeuron()
        {
            OnDelete?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
            
            Debug.Log($"{name} got deleted");
        }
    }
}
