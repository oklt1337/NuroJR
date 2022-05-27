using Model.Neurons;
using UnityEditor;
using UnityEngine;

namespace Model.Layer
{
    public class HiddenLayerObj : NetworkLayerObj
    {
        [HideInInspector] public Vector2 position;

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
            
            neuron.name = "HiddenNeuron";
            neuron.guid = GUID.Generate().ToString();

            neurons.Add(neuron);
            
            AssetDatabase.AddObjectToAsset(neuron, this);
            AssetDatabase.SaveAssets();
            
            OnNeuronCreated?.Invoke(neuron);
        }
    }
}