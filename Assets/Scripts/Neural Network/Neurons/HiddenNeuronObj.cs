using System.Collections.Generic;

namespace Neural_Network.Neurons
{
    public class HiddenNeuronObj : NeuronObj
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
            var neuron = new HiddenNeuron
            {
                Value = value
            };

            return neuron;
        }
    }

    public class HiddenNeuron : Neuron
    {
        
    }
}