using System;
using System.Collections.Generic;
using Neural_Network.Neurons;

namespace Neural_Network.Nodes
{
    public class InputNeuron : Neuron
    {
        private readonly List<Neuron> _children = new();

        public Action<Neuron, Neuron> OnChildAdded;
        public Action<Neuron, Neuron> OnChildRemoved;

        public void AddChild(Neuron child)
        {
            _children.Add(child);
            OnChildAdded?.Invoke(this, child);
        }

        public void RemoveChild(Neuron child)
        {
            _children.Remove(child);
            OnChildRemoved?.Invoke(this, child);
        }
        
        public List<Neuron> GetChildren()
        {
            return _children;
        }
    }
}