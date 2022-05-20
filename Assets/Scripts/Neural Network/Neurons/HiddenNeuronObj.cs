using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Neural_Network.Neurons
{
    public class HiddenNeuronObj : NeuronObj
    {
        [HideInInspector] public List<NeuronObj> children = new();
        public float bias;

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
    }
}