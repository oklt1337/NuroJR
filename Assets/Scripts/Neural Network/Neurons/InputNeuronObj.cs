using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

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