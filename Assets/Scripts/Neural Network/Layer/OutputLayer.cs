using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Layer
{
    public class OutputLayer : NetworkLayer
    {
        [HideInInspector] public Vector2 position = new(150, 0);
        
        public override void CreateNeuron()
        {
            // Make sure not to many Neurons can be added.
            if (neurons.Count >= 7)
                return;
            
            var neuron = CreateInstance(typeof(OutputNeuron)) as Neuron;
            if (neuron == null)
                return;

            neuron.name = "OutputNeuron";
            neuron.guid = GUID.Generate().ToString();

            neurons.Add(neuron);
            
            AssetDatabase.AddObjectToAsset(neuron, this);
            AssetDatabase.SaveAssets();

            Debug.Log("Created OutputNeuron");
            OnNeuronCreated?.Invoke(neuron);
        }
    }
}