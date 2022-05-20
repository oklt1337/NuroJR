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
    }
}