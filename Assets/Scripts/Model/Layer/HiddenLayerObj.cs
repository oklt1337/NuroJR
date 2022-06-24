using System;
using System.Collections.Generic;
using Model.Neurons;
using UnityEditor;
using UnityEngine;

namespace Model.Layer
{
    public class HiddenLayerObj : NetworkLayerObj
    {
        [HideInInspector] public Vector2 position;
        public List<NeuronValues> neuronValues = new();

        /// <summary>
        /// Create a HiddenNeuron and add it to Asset.
        /// Invoke OnNeuronCreated Event
        /// </summary>
        public override void CreateNeuron()
        {
            // Make sure not to many Neurons can be added.
            if (neurons.Count >= 7)
                return;
            
            var neuron = CreateInstance(typeof(HiddenNeuronObj)) as NeuronObj;
            if (neuron == null)
                return;
            
            neuron.name = "HiddenNeuron" + neurons.Count;
            neuron.guid = GUID.Generate().ToString();

            neurons.Add(neuron);
            CreateNeuronValue(neuron as HiddenNeuronObj);
            
            AssetDatabase.AddObjectToAsset(neuron, this);
            AssetDatabase.SaveAssets();
            
            OnNeuronCreated?.Invoke(neuron);
        }
        
        #region NeuronValues

        private void CreateNeuronValue(HiddenNeuronObj neuronObj)
        {
            var obj = new NeuronValues
            {
                bias = neuronObj.bias,
                neuronName = neuronObj.name,
                neuron = neuronObj
            };
            neuronValues.Add(obj);
        }

        public void UpdateNeuronValue(HiddenNeuronObj neuronObj)
        {
            var index = neuronValues.FindIndex(x => x.neuron == neuronObj);
            if (index == -1)
                return;
            
            var obj = neuronValues[index];
            obj.bias = neuronObj.bias;
            neuronValues[index] = obj;
        }

        public void RemoveNeuronValue(HiddenNeuronObj neuronObj)
        {
            var index = neuronValues.FindIndex(x => x.neuron == neuronObj);
            if (index == -1)
                return;
            
            neuronValues.RemoveAt(index);
        }

        #endregion
    }
    
    [Serializable]
    public struct NeuronValues
    {
        [HideInInspector] public NeuronObj neuron;
        public string neuronName;
        public float bias;
    }
}