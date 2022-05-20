using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Layer
{
    public class InputLayerObj : NetworkLayerObj
    {
        [HideInInspector] public Vector2 position = new(0, 0);
        
        public override void CreateNeuron()
        {
            // Make sure not to many Neurons can be added.
            if (neurons.Count >= 7)
                return;
            
            var neuron = CreateInstance(typeof(InputNeuronObj)) as NeuronObj;
            if (neuron == null)
                return;

            neuron.name = "InputNeuron";
            neuron.GenerateNewGuid();

            neurons.Add(neuron);
            
            AssetDatabase.AddObjectToAsset(neuron, this);
            AssetDatabase.SaveAssets();

            Debug.Log("Created InputNeuron");
            OnNeuronCreated?.Invoke(neuron);
        }
    }
}