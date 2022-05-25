using System.Collections.Generic;
using UnityEngine;

namespace Neural_Network.Neurons
{
    public class HiddenNeuronObj : NeuronObj
    {
        [HideInInspector] public List<NeuronObj> children = new();
        public float bias;

        /// <summary>
        /// Add Child to List
        /// </summary>
        /// <param name="child">NeuronObj</param>
        public void AddChild(NeuronObj child)
        {
            children.Add(child);
        }

        /// <summary>
        /// Remove Child from List
        /// </summary>
        /// <param name="child">NeuronObj</param>
        public void RemoveChild(NeuronObj child)
        {
            children.Remove(child);
        }

        /// <summary>
        /// Get List of Children.
        /// </summary>
        /// <returns>List NeuronObj</returns>
        public List<NeuronObj> GetChildren()
        {
            return children;
        }
    }
}