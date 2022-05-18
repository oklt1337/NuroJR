using Neural_Network.Neurons;
using UnityEditor;
using UnityEngine;

namespace Neural_Network.Layer
{
    public class HiddenLayerObj : NetworkLayerObj
    {
        [HideInInspector] public Vector2 position;

        public override NetworkLayer Clone(bool random)
        {
            var layer = new HiddenLayer();
            neurons.ForEach(x => layer.Neurons.Add(x.Clone(random)));
            return layer;
        }

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

            Debug.Log("Created HiddenNeuron");
            OnNeuronCreated?.Invoke(neuron);
        }
    }

    public class HiddenLayer : NetworkLayer
    {
        
    }
}