﻿using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network
{
    public class ConnectionObj : ScriptableObject
    {
        private NeuronObj child;
        private NeuronObj parent;
        
        public float weight;

        public void DeleteConnection()
        {
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

        public Connection Clone()
        {
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