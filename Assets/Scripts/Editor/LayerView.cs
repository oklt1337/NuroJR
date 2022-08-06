using System;
using Model.Layer;
using Model.Neurons;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public sealed class LayerView : Node
    {
        public readonly NetworkLayerObj NetworkLayerObj;

        public Action<LayerView> OnLayerSelected;
        public Action<NeuronView> OnNodeSelected;
        public Action<NeuronView> OnNeuronViewCreated;

        #region Constructor

        public LayerView(NetworkLayerObj networkLayerObj)
        {
            NetworkLayerObj = networkLayerObj;
            
            title = networkLayerObj.GetType().Name;
            viewDataKey = NetworkLayerObj.guid;
            capabilities = Capabilities.Selectable | Capabilities.Deletable;
            
            var button = new Button
            {
                text = "Add new Neuron"
            };

            switch (NetworkLayerObj)
            {
                case InputLayerObj inputLayer:
                    style.left = inputLayer.position.x;
                    style.top = inputLayer.position.y;
                    
                    button.clicked += () => inputLayer.CreateNeuron();
                    break;
                case OutputLayerObj outputLayer:
                    style.left = outputLayer.position.x;
                    style.top = outputLayer.position.y;
                    
                    button.clicked += () => outputLayer.CreateNeuron();
                    break;
                case HiddenLayerObj hiddenLayer:
                    style.left = hiddenLayer.position.x;
                    style.top = hiddenLayer.position.y;
                    
                    button.clicked += () => hiddenLayer.CreateNeuron();
                    break;
            }
            Add(button);
        }

        #endregion

        #region Position

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            switch (NetworkLayerObj)
            {
                case InputLayerObj inputLayer:
                    inputLayer.position.x = newPos.xMin;
                    inputLayer.position.y = newPos.yMin;
                    break;
                case OutputLayerObj outputLayer:
                    outputLayer.position.x = newPos.xMin;
                    outputLayer.position.y = newPos.yMin;
                    break;
                case HiddenLayerObj hiddenLayer:
                    hiddenLayer.position.x = newPos.xMin;
                    hiddenLayer.position.y = newPos.yMin;
                    break;
            }
        }
        
        public void RemapView()
        {
            switch (NetworkLayerObj)
            {
                case InputLayerObj inputLayer:
                    style.left = inputLayer.position.x;
                    style.top = inputLayer.position.y;
                    break;
                case OutputLayerObj outputLayer:
                    style.left = outputLayer.position.x;
                    style.top = outputLayer.position.y;
                    break;
                case HiddenLayerObj hiddenLayer:
                    style.left = hiddenLayer.position.x;
                    style.top = hiddenLayer.position.y;
                    break;
            }
        }

        #endregion

        #region Creation

        /// <summary>
        /// Create a NeuronView
        /// </summary>
        /// <param name="neuronObj">NeuronObj</param>
        public void CreateNeuronView(NeuronObj neuronObj)
        {
            var index = NetworkLayerObj.GetNeurons().FindIndex(n => n == neuronObj);
            if (index == -1)
                return;

            neuronObj.neuronPosition = new Vector2(16f, (index + 1) * 105);
            var neuronView = new NeuronView(neuronObj)
            {
                OnNodeSelected = OnNodeSelected
            };
            
            if (NetworkLayerObj.GetType() == typeof(InputLayerObj))
            {
                neuronView.style.backgroundColor = Color.green;
            }
            else if (NetworkLayerObj.GetType() == typeof(OutputLayerObj))
            {
                neuronView.style.backgroundColor = Color.red;
            }
            else if (NetworkLayerObj.GetType() == typeof(HiddenLayerObj))
            {
                neuronView.style.backgroundColor = Color.grey;
            }
            
            Add(neuronView);
            OnNeuronViewCreated?.Invoke(neuronView);
        }

        #endregion

        /// <summary>
        /// Triggers OnLayerSelected event
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            OnLayerSelected?.Invoke(this);
        }
    }
}