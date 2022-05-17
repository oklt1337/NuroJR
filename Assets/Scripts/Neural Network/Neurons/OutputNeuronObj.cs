namespace Neural_Network.Neurons
{
    public class OutputNeuronObj : NeuronObj
    {
        public override Neuron Clone()
        {
            var neuron = new OutputNeuron
            {
                Value = value
            };

            return neuron;
        }
    }

    public class OutputNeuron : Neuron
    {
        
    }
}