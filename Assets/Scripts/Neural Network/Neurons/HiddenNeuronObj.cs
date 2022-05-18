using System;
using System.Collections.Generic;
using System.Linq;

namespace Neural_Network.Neurons
{
    public class HiddenNeuronObj : NeuronObj
    {
        private readonly List<NeuronObj> _children = new();
        public float bias;

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
                NeuronObj = this
            };

            connectionObjs.ForEach(x => neuron.Connections.Add(x.Clone()));
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