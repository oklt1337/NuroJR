using System;
using System.Collections.Generic;
using Neural_Network.Layer;
using Neural_Network.Neurons;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public sealed class LayerView : Node
    {
        public readonly NetworkLayer NetworkLayer;

        public Action<LayerView> OnLayerSelected;
        public Action<NeuronView> OnNodeSelected;
        public Action OnNeuronViewCreation;

        #region Constructor

        public LayerView(NetworkLayer networkLayer)
        {
            NetworkLayer = networkLayer;

            networkLayer.OnNeuronCreated += CreateNeuronView;
            
            title = networkLayer.GetType().Name;
            viewDataKey = NetworkLayer.guid;
            
            var button = new Button
            {
                text = "Add new Neuron"
            };

            switch (NetworkLayer)
            {
                case InputLayer inputLayer:
                    style.left = inputLayer.position.x;
                    style.top = inputLayer.position.y;
                    
                    button.clicked += () => inputLayer.CreateNeuron();
                    break;
                case OutputLayer outputLayer:
                    style.left = outputLayer.position.x;
                    style.top = outputLayer.position.y;
                    
                    button.clicked += () => outputLayer.CreateNeuron();
                    break;
                case HiddenLayer hiddenLayer:
                    style.left = hiddenLayer.position.x;
                    style.top = hiddenLayer.position.y;
                    
                    button.clicked += () => hiddenLayer.CreateNeuron();
                    break;
            }
            Add(button);

            capabilities = Capabilities.Selectable | Capabilities.Deletable;
        }

        #endregion

        #region Position

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            switch (NetworkLayer)
            {
                case InputLayer inputLayer:
                    inputLayer.position.x = newPos.xMin;
                    inputLayer.position.y = newPos.yMin;
                    break;
                case OutputLayer outputLayer:
                    outputLayer.position.x = newPos.xMin;
                    outputLayer.position.y = newPos.yMin;
                    break;
                case HiddenLayer hiddenLayer:
                    hiddenLayer.position.x = newPos.xMin;
                    hiddenLayer.position.y = newPos.yMin;
                    break;
            }
        }
        
        public void RemapView()
        {
            switch (NetworkLayer)
            {
                case InputLayer inputLayer:
                    style.left = inputLayer.position.x;
                    style.top = inputLayer.position.y;
                    break;
                case OutputLayer outputLayer:
                    style.left = outputLayer.position.x;
                    style.top = outputLayer.position.y;
                    break;
                case HiddenLayer hiddenLayer:
                    style.left = hiddenLayer.position.x;
                    style.top = hiddenLayer.position.y;
                    break;
            }
        }

        #endregion

        #region Creation

        public void CreateNeuronView(Neuron neuron)
        {
            var index = NetworkLayer.GetNeurons().FindIndex(n => n == neuron);
            if (index == -1)
                return;

            neuron.neuronPosition = new Vector2(16f, (index + 1) * 105);
            var nodeView = new NeuronView(neuron)
            {
                OnNodeSelected = OnNodeSelected
            };
            
            if (NetworkLayer.GetType() == typeof(InputLayer))
            {
                nodeView.style.backgroundColor = Color.green;
            }
            else if (NetworkLayer.GetType() == typeof(OutputLayer))
            {
                nodeView.style.backgroundColor = Color.red;
            }
            else if (NetworkLayer.GetType() == typeof(HiddenLayer))
            {
            }
            
            Add(nodeView);
            OnNeuronViewCreation?.Invoke();
        }

        #endregion

        public override void OnSelected()
        {
            base.OnSelected();
            OnLayerSelected?.Invoke(this);
        }
    }
}