namespace Neural_Network.Neurons
{
    public class OutputNeuronObj : NeuronObj
    {
        public override Neuron Clone()
        {
            var neuron = new OutputNeuron
            {
                NeuronObj = this
            };
            connectionObjs.ForEach(x => neuron.Connections.Add(x.Clone()));
            return neuron;
        }
    }

    public class OutputNeuron : Neuron
    {
        private float output;

        public override float GetValue(Neuron neuron)
        {
            return output;
        }
    }
}