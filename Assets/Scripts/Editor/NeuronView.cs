using System;
using Model.Neurons;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Editor
{
    public sealed class NeuronView : Node
    {
        public Action<NeuronView> OnNodeSelected;

        public readonly NeuronObj NeuronObj;
        public Port Input;
        public Port Output;

        #region Constructor

        public NeuronView(NeuronObj neuronObj)
        {
            NeuronObj = neuronObj;

            title = "Neuron";
            viewDataKey = NeuronObj.guid;
            capabilities = Capabilities.Selectable | Capabilities.Deletable;
            
            style.left = NeuronObj.neuronPosition.x;
            style.top = NeuronObj.neuronPosition.y;

            CreateInputPorts();
            CreateOutputPorts();
        }

        #endregion

        #region Position

        /// <summary>
        /// Set Position of Neuron Object
        /// </summary>
        /// <param name="newPos">Rect</param>
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            NeuronObj.neuronPosition.x = newPos.xMin;
            NeuronObj.neuronPosition.y = newPos.yMin;
        }
        
        /// <summary>
        /// Set Position of NeuronView
        /// </summary>
        public void RemapView()
        {
            style.left = NeuronObj.neuronPosition.x;
            style.top = NeuronObj.neuronPosition.y;
        }

        #endregion

        #region Ports

        /// <summary>
        /// Instantiate Input ports
        /// </summary>
        private void CreateInputPorts()
        {
            if (NeuronObj.GetType() == typeof(HiddenNeuronObj) || NeuronObj.GetType() == typeof(OutputNeuronObj))
            {
                Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            }

            if (Input == null) 
                return;
            
            Input.portName = "";
            inputContainer.Add(Input);
        }

        /// <summary>
        /// Instantiate Output ports
        /// </summary>
        private void CreateOutputPorts()
        {
            if (NeuronObj.GetType() == typeof(HiddenNeuronObj) || NeuronObj.GetType() == typeof(InputNeuronObj))
            {
                Output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }

            if (Output == null) 
                return;
            
            Output.portName = "";
            outputContainer.Add(Output);
        }

        #endregion
        
        /// <summary>
        /// Triggers OnNodeSelected event
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(this);
        }
    }
}
