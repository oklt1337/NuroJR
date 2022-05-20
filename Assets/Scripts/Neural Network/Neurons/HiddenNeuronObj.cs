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

        public override Neuron Clone(bool random)
        {
            if (random)
                bias = UnityEngine.Random.Range(0, 5);
            
            var neuron = new HiddenNeuron
            {
                NeuronObj = this,
                Bias = bias
            };

            connectionObjs.ForEach(x => neuron.Connections.Add(x.Clone(random)));
            return neuron;
        }
    }

    public class HiddenNeuron : Neuron
    {
        public float Bias;
        private float value;

        public void SumInputs(List<Neuron> inputs)
        {
            var sum = 0f;
            foreach (var input in inputs)
            {
                switch (input)
                {
                    case InputNeuron inputNeuron:
                        sum += inputNeuron.GetValue(this);
                        break;
                    case HiddenNeuron hiddenNeuron:
                        sum += hiddenNeuron.GetValue(this) + Bias;
                        break;
                }
            }

            Calculate(sum);
        }

        private void Calculate(float input)
        {
            value = Sigmoid(input);

            //value = (float) Math.Tanh(input);
        }

        private static float Sigmoid(float input)
        {
            return 1.0f / (1.0f + (float)Math.Exp(-input));
        }

        public override float GetValue(Neuron neuron)
        {
            foreach (var connection in Connections.Where(connection => connection.ChildObj == neuron.NeuronObj))
            {
                return value * connection.Weight;
            }

            return value;
        }
    }
}