using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Neurons
{
    [InlineEditor(InlineEditorModes.FullEditor)]
    public class Neuron : ScriptableObject
    {
        [HideInInspector] public Vector2 neuronPosition;
        [HideInInspector] public string guid;
        
        public float value;
        
        public Action<Neuron> OnDelete;
        
        [Button]
        public void DeleteNeuron()
        {
            OnDelete?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
        }
    }
}
