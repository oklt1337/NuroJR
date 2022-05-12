using System;
using Neural_Network.Neurons;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Neural_Network
{
    [InlineEditor(InlineEditorModes.FullEditor)]
    public class Connection : ScriptableObject
    {
        public Neuron _child;
        public Neuron _parent;
        [HideInInspector] public string guid;
        
        [HideInInspector] public float weight;

        public Action<Connection> OnDeleted;

        public void DeleteConnection()
        {
            OnDeleted?.Invoke(this);
            AssetDatabase.RemoveObjectFromAsset(this);
            AssetDatabase.SaveAssets();
        }
        
        public void AddChild(Neuron child)
        {
            _child = child;
        }

        public void AddParent(Neuron parent)
        {
            _parent = parent;
        }

        public Neuron GetChild()
        {
            return _child;
        }
        
        public Neuron GetParent()
        {
            return _parent;
        }
    }
}