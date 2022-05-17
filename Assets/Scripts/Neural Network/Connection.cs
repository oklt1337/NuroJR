using System;
using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network
{
    public class Connection : ScriptableObject
    {
        private NeuronObj child;
        private NeuronObj parent;
        [HideInInspector] public string guid;
        
        public float weight;

        public Action<Connection> OnDeleted;

        public void DeleteConnection()
        {
            OnDeleted?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
        }
        
        public void AddChild(NeuronObj neuronObj)
        {
            child = neuronObj;
        }

        public void AddParent(NeuronObj neuronObj)
        {
            parent = neuronObj;
        }

        public NeuronObj GetChild()
        {
            return child;
        }
        
        public NeuronObj GetParent()
        {
            return parent;
        }
    }
}