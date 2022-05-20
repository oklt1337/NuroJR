using System;
using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Neural_Network
{
    public class ConnectionObj : ScriptableObject
    {
        private NeuronObj child;
        private NeuronObj parent;
        
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

        public Connection Clone(bool random)
        {
            if (random)
                weight = Random.Range(-1, 1);

            var connection = new Connection
            {
                Weight = weight,
                ChildObj = GetChild(),
                ParentObj = GetParent()
            };

            return connection;
        }
    }

    public class Connection
    {
        public float Weight;
        public NeuronObj ParentObj { get; set; }
        public NeuronObj ChildObj { get; set; }
    }
}