using System;
using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Connection
{
    public class ConnectionObj : ScriptableObject
    {
        [HideInInspector] public NeuronObj child;
        [HideInInspector] public NeuronObj parent;
        
        public float weight;
        
        public Action<ConnectionObj> OnDeleted;

        /// <summary>
        /// Delete Connection Obj
        /// </summary>
        public void DeleteConnection()
        {
            OnDeleted?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
        }
        
        /// <summary>
        /// Add Child
        /// </summary>
        /// <param name="neuronObj">NeuronObj</param>
        public void AddChild(NeuronObj neuronObj)
        {
            child = neuronObj;
        }

        /// <summary>
        /// Add Parent
        /// </summary>
        /// <param name="neuronObj">NeuronObj</param>
        public void AddParent(NeuronObj neuronObj)
        {
            parent = neuronObj;
        }

        /// <summary>
        /// Get Child
        /// </summary>
        /// <returns>NeuronObj</returns>
        public NeuronObj GetChild()
        {
            return child;
        }
        
        /// <summary>
        /// Get Parent
        /// </summary>
        /// <returns>NeuronObj</returns>
        public NeuronObj GetParent()
        {
            return parent;
        }
    }
}