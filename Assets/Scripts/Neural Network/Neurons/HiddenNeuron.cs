﻿using System;
using System.Collections.Generic;

namespace Neural_Network.Neurons
{
    public class HiddenNeuron : Neuron
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