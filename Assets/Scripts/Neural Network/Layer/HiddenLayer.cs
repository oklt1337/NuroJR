using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Layer
{
    public class HiddenLayer : NetworkLayer
    {
        [HideInInspector] public Vector2 position;
        
        public override void CreateNeuron()
        {
            // Make sure not to many Neurons can be added.
            if (neurons.Count >= 7)
                return;
            
            var neuron = CreateInstance(typeof(HiddenNeuron)) as Neuron;
            if (neuron == null)
                return;

            neuron.name = "HiddenNeuron";
            neuron.guid = GUID.Generate().ToString();

            neurons.Add(neuron);
            
            AssetDatabase.AddObjectToAsset(neuron, this);
            AssetDatabase.SaveAssets();

            Debug.Log("Created HiddenNeuron");
            OnNeuronCreated?.Invoke(neuron);
        }
    }
}