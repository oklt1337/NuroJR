using Model.Neurons;
using UnityEditor;
using UnityEngine;

namespace Model.Layer
{
    public class InputLayerObj : NetworkLayerObj
    {
        [HideInInspector] public Vector2 position = new(0, 0);
        
        /// <summary>
        /// Create a InputNeuron and add it to Asset
        /// Invoke OnNeuronCreated Event
        /// </summary>
        public override void CreateNeuron()
        {
            // Make sure not to many Neurons can be added.
            if (neurons.Count >= 7)
                return;
            
            var neuron = CreateInstance(typeof(InputNeuronObj)) as NeuronObj;
            if (neuron == null)
                return;

            neuron.name = "InputNeuron" + neurons.Count;
            neuron.GenerateNewGuid();

            neurons.Add(neuron);
            
            AssetDatabase.AddObjectToAsset(neuron, this);
            AssetDatabase.SaveAssets();

            OnNeuronCreated?.Invoke(neuron);
        }
    }
}