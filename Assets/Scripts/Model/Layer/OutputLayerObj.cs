using Model.Neurons;
using UnityEditor;
using UnityEngine;

namespace Model.Layer
{
    public class OutputLayerObj : NetworkLayerObj
    {
        [HideInInspector] public Vector2 position = new(150, 0);

        /// <summary>
        /// Create a OutputNeuron and add it to Asset
        /// Invoke OnNeuronCreated Event
        /// </summary>
        public override void CreateNeuron()
        {
            // Make sure not to many Neurons can be added.
            if (neurons.Count >= 7)
                return;
            
            var neuron = CreateInstance(typeof(OutputNeuronObj)) as NeuronObj;
            if (neuron == null)
                return;

            neuron.name = "OutputNeuron";
            neuron.guid = GUID.Generate().ToString();

            neurons.Add(neuron);
            
            AssetDatabase.AddObjectToAsset(neuron, this);
            AssetDatabase.SaveAssets();
            
            OnNeuronCreated?.Invoke(neuron);
        }
    }
}