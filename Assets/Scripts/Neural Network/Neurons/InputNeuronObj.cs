using System;
using System.Collections.Generic;

namespace Neural_Network.Neurons
{
    public class InputNeuronObj : NeuronObj
    {
        private readonly List<NeuronObj> _children = new();

        public void AddChild(NeuronObj child)
        {
            _children.Add(child);
        }

        public void RemoveChild(NeuronObj child)
        {
            _children.Remove(child);
        }
        
        public List<NeuronObj> GetChildren()
        {
            return _children;
        }

        public override Neuron Clone()
        {
            var neuron = new InputNeuron
            {
                Value = value
            };

            return neuron;
        }
    }

    public class InputNeuron : Neuron
    {
        
    }
}