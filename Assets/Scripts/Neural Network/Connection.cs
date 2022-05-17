using System;
using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network
{
    public class Connection : ScriptableObject
    {
        private Neuron child;
        private Neuron parent;
        [HideInInspector] public string guid;
        
        public float weight;

        public Action<Connection> OnDeleted;

        public void DeleteConnection()
        {
            OnDeleted?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
        }
        
        public void AddChild(Neuron neuron)
        {
            child = neuron;
        }

        public void AddParent(Neuron neuron)
        {
            parent = neuron;
        }

        public Neuron GetChild()
        {
            return child;
        }
        
        public Neuron GetParent()
        {
            return parent;
        }
    }
}