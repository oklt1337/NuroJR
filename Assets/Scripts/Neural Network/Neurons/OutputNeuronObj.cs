using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Neural_Network.Neurons
{
    public class OutputNeuronObj : NeuronObj
    {
        public override Neuron Clone(bool random)
        {
            var neuron = new OutputNeuron
            {
                NeuronObj = this
            };
            connectionObjs.ForEach(x => neuron.Connections.Add(x.Clone(random)));
            return neuron;
        }
    }

    public class OutputNeuron : Neuron
    {
        private float output;
        
        public void SumInputs(List<Neuron> inputs)
        {
            var sum = 0f;
            foreach (var input in inputs)
            {
                switch (input)
                {
                    case InputNeuron inputNeuron:
                        sum += inputNeuron.GetValue();
                        break;
                    case HiddenNeuron hiddenNeuron:
                        sum += hiddenNeuron.GetValue(this);
                        break;
                }
            }

            output = sum;
        }

        public override float GetValue([Optional] Neuron neuron)
        {
            return output;
        }
    }
}