using System;
using System.Collections.Generic;
using Neural_Network.Connection;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Neurons
{
    public abstract class NeuronObj : ScriptableObject
    {
        [HideInInspector] public Vector2 neuronPosition;
        [HideInInspector] public string guid;
        [HideInInspector] public List<ConnectionObj> connectionObjs = new();

        public Action<NeuronObj> OnDelete;

        /// <summary>
        /// Delete Neuron from Asset
        /// Invoke OnDelete Event
        /// </summary>
        public void DeleteNeuron()
        {
            OnDelete?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// Generate a GUID
        /// </summary>
        public void GenerateNewGuid()
        {
            guid = GUID.Generate().ToString();
        }
    }
}
