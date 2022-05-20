using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Neural_Network.Neurons
{
    public class InputNeuronObj : NeuronObj
    {
        [HideInInspector] public List<NeuronObj> children = new();

        public void AddChild(NeuronObj child)
        {
            children.Add(child);
        }

        public void RemoveChild(NeuronObj child)
        {
            children.Remove(child);
        }
        
        public List<NeuronObj> GetChildren()
        {
            return children;
        }

        public override Neuron Clone(bool random)
        {
            var neuron = new InputNeuron
            {
                NeuronObj = this
            };
            connectionObjs.ForEach(x => neuron.Connections.Add(x.Clone(random)));
            return neuron;
        }
    }

    public class InputNeuron : Neuron
    {
        private float input;
        public override float GetValue([Optional] Neuron neuron)
        {
            return input;
        }

        public void SetInput(float value)
        {
            input = value;
        }
    }
}