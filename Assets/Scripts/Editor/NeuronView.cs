using System;
using Neural_Network.Neurons;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor
{
    public class NeuronView : Node
    {
        public Action<NeuronView> OnNodeSelected;
        
        public readonly Neuron Neuron;
        public Port Input;
        public Port Output;

        #region Constructor

        public NeuronView(Neuron neuron)
        {
            Neuron = neuron;
            
            title = "Neuron";
            viewDataKey = Neuron.guid;

            style.left = Neuron.neuronPosition.x;
            style.top = Neuron.neuronPosition.y;

            CreateInputPorts();
            CreateOutputPorts();
            
            capabilities = Capabilities.Selectable | Capabilities.Deletable | Capabilities.Ascendable;
        }

        #endregion

        #region Position

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            Neuron.neuronPosition.x = newPos.xMin;
            Neuron.neuronPosition.y = newPos.yMin;
        }
        
        public void RemapView()
        {
            style.left = Neuron.neuronPosition.x;
            style.top = Neuron.neuronPosition.y;
        }

        #endregion

        #region Ports

        private void CreateInputPorts()
        {
            if (Neuron.GetType() == typeof(HiddenNeuron) || Neuron.GetType() == typeof(OutputNeuron))
                Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));

            if (Input == null) 
                return;
            
            Input.portName = "";
            inputContainer.Add(Input);
        }

        private void CreateOutputPorts()
        {
            if (Neuron.GetType() == typeof(HiddenNeuron) || Neuron.GetType() == typeof(InputNeuron))
                Output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));

            if (Output == null) 
                return;
            
            Output.portName = "";
            outputContainer.Add(Output);
        }

        #endregion
        
        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }
    }
}
