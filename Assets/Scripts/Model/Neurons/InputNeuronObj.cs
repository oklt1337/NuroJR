﻿using System.Collections.Generic;
using UnityEngine;

namespace Model.Neurons
{
    public class InputNeuronObj : NeuronObj
    {
        [HideInInspector] public List<NeuronObj> children = new();

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