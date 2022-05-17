using System;
using System.Collections.Generic;
using System.Linq;
using Neural_Network;
using Neural_Network.Layer;
using Neural_Network.Neurons;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor
{
    public class NeuralNetworkView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NeuralNetworkView, UxmlTraits>{}

        public NeuralNetwork Network;
        
        private readonly List<GraphElement> _elements = new();

        public Action<NeuronView> OnNodeSelected;
        public Action<LayerView> OnLayerSelected;
        public Action<EdgeView> OnEdgeSelected;

        private Action<NeuronView> onNeuronViewCreated;

        #region Constructor

        public NeuralNetworkView()
        {
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/NuroJR.uss");
            styleSheets.Add(styleSheet);
        }

        #endregion

        #region Graph

        public void PopulateView(NeuralNetwork neuralNetwork)
        {
            Network = neuralNetwork;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            neuralNetwork.OnLayerCreated += CreateLayerView;
            onNeuronViewCreated += AddElements;
            onNeuronViewCreated += CreateEdge;

            InitializeView(neuralNetwork);
        }

        public void Apply()
        {
            if (Network != null)
            {
                InitializeView(Network);
            }
        }

        public void UnPopulateView()
        {
            RemoveEdgeViews();
            RemoveNeuronViews();
            RemoveLayerViews();
        }

        private void RemoveEdgeViews()
        {
            var edgeViews = new List<EdgeView>();
            // get all edgeViews in elements
            foreach (var element in _elements)
            {
                if (element is not EdgeView edgeView) 
                    continue;
                
                edgeViews.Add(edgeView);
            }

            // remove edgeViews from graph and elements list.
            for (var i = edgeViews.Count - 1; i >= 0; i--)
            {
                _elements.Remove(edgeViews[i]);
                RemoveElement(edgeViews[i]);
            }
        }

        private void RemoveNeuronViews()
        {
            var neuronViews = new List<NeuronView>();
            // get all neuronViews in elements
            foreach (var element in _elements)
            {
                if (element is not NeuronView neuronView) 
                    continue;
                
                neuronViews.Add(neuronView);
            }

            // remove neuronViews from graph and elements list.
            for (var i = neuronViews.Count - 1; i >= 0; i--)
            {
                _elements.Remove(neuronViews[i]);
                RemoveElement(neuronViews[i]);
            }
        }

        private void RemoveLayerViews()
        {
            var layerViews = new List<LayerView>();
            // get all layerViews in elements
            foreach (var element in _elements)
            {
                if (element is not LayerView layerView) 
                    continue;
                
                layerViews.Add(layerView);
            }

            // remove layerViews from graph and elements list.
            for (var i = layerViews.Count - 1; i >= 0; i--)
            {
                _elements.Remove(layerViews[i]);
                RemoveElement(layerViews[i]);
            }
        }

        private void InitializeView(NeuralNetwork neuralNetwork)
        {
            neuralNetwork.GetLayer().ForEach(CreateLayerView);
            neuralNetwork.GetLayer().ForEach(RestoreNeuronView);
            neuralNetwork.GetConnections().ForEach(CreateEdgeView);
        }

        private void AddElements(GraphElement graphElement)
        {
            if (graphElement == null)
                return;
            if (_elements.Contains(graphElement))
                return;
            _elements.Add(graphElement);
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            graphviewchange.elementsToRemove?.ForEach(element =>
            {
                switch (element)
                {
                    case LayerView layerView:
                        if (layerView.NetworkLayer.GetType() == typeof(HiddenLayer))
                        {
                            Network.RemoveLayer(layerView.NetworkLayer);
                            SortLayer();
                        }
                        break;
                    case NeuronView nodeView:
                        var index = Network.layers.FindIndex(x => x.neurons.Contains(nodeView.Neuron));
                        if (index == -1)
                            return;
                        if(Network.layers[index].neurons.Count == 1)
                            return;
                        
                        Network.layers[index].RemoveNeuron(nodeView.Neuron);
                        SortNeurons();
                        break;
                    case Edge edge:
                    {
                        var parentView = edge.output.node as NeuronView;
                        var childView = edge.input.node as NeuronView;
                        var parentNeuron = parentView!.Neuron;
                        switch (parentNeuron)
                        {
                            case InputNeuron neuron:
                                neuron.RemoveChild(childView!.Neuron);
                                break;
                            case HiddenNeuron neuron:
                                neuron.RemoveChild(childView!.Neuron);
                                break;
                        }
                        break;
                    }
                }
            });

            graphviewchange.edgesToCreate?.ForEach(edge =>
            {
                var parentView = edge.output.node as NeuronView;
                var childView = edge.input.node as NeuronView;
                var parentNeuron = parentView!.Neuron;
                switch (parentNeuron)
                {
                    case InputNeuron neuron:
                        neuron.AddChild(childView!.Neuron);
                        break;
                    case HiddenNeuron neuron:
                        neuron.AddChild(childView!.Neuron);
                        break;
                }
            });

            return graphviewchange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            {
                var type = typeof(HiddenLayer);
                evt.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", _ => CreateHiddenLayer());
            }
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
        }

        #endregion

        #region Layer

        private void CreateHiddenLayer()
        {
            if (Network == null)
                return;
            Network.CreateLayer(typeof(HiddenLayer));
        }

        private void CreateLayerView(NetworkLayer networkLayer)
        {
            var layerView = new LayerView(networkLayer)
            {
                OnLayerSelected = OnLayerSelected,
                OnNodeSelected = OnNodeSelected,
                OnNeuronViewCreated = onNeuronViewCreated,
                OnDelete = DeleteLayer,
                expanded = false
            };

            if (networkLayer.GetType() == typeof(InputLayer))
            {
                layerView.style.backgroundColor = Color.green;
            }
            else if (networkLayer.GetType() == typeof(OutputLayer))
            {
                layerView.style.backgroundColor = Color.red;
            }

            AddElements(layerView);
            AddElement(layerView);
            SortLayer();
        }

        private LayerView FindLayerView(NetworkLayer layer)
        {
            return GetNodeByGuid(layer.guid) as LayerView;
        }

        private void SortLayer()
        {
            if (Network.GetLayer().Count < 2)
                return;

            var layers = new List<NetworkLayer>();
            foreach (var layer in Network.GetLayer().Where(layer => layer != null))
            {
                layers.Add(layer);

                var index = Network.GetLayer().FindIndex(l => l == layer);
                if (index != -1)
                {
                    switch (layer)
                    {
                        case InputLayer inputLayer:
                            inputLayer.position.x = index * 150;
                            break;
                        case OutputLayer outputLayer:
                            outputLayer.position.x = index * 150;
                            break;
                        case HiddenLayer hiddenLayer:
                            hiddenLayer.position.x = index * 150;
                            break;
                    }
                }
            }

            foreach (var layerView in layers.Select(FindLayerView))
            {
                layerView?.RemapView();
            }
        }

        private void DeleteLayer(NetworkLayer networkLayer)
        {
            var layerView =  FindLayerView(networkLayer);
            var neurons = networkLayer.neurons;
        }

        #endregion

        #region Node

        private void RestoreNeuronView(NetworkLayer layer)
        {
            var layerView = FindLayerView(layer);
            foreach (var neuron in layer.GetNeurons())
            {
                layerView.CreateNeuronView(neuron);
            }
        }

        private NeuronView FindNodeView(Neuron neuron)
        {
            return GetNodeByGuid(neuron.guid) as NeuronView;
        }

        private void SortNeurons()
        {
            var neurons = new List<Neuron>();

            foreach (var layer in Network.GetLayer())
            {
                foreach (var neuron in layer.GetNeurons())
                {
                    neurons.Add(neuron);

                    var index = layer.GetNeurons().FindIndex(n => n == neuron);
                    if (index != -1)
                    {
                        neuron.neuronPosition.y = (index + 1) * 105;
                    }
                }
            }

            foreach (var nodeView in neurons.Select(FindNodeView))
            {
                nodeView.RemapView();
            }
        }

        #endregion

        #region Edge

        private void CreateEdge(NeuronView neuronView)
        {
            var neuron = neuronView.Neuron;
            
            // Check if 1st Neuron in Layer
            var index = Network.layers.FindIndex(x => x.neurons.Contains(neuron));
            if (index == -1)
                return;
            if (Network.layers[index].neurons.Count == 1)
            {
                RemoveOldEdges();
            }
            
            var connectionsToCreate = Network.connections.Where(connection => connection.GetChild() == neuron || connection.GetParent() == neuron).ToList();

            foreach (var connection in connectionsToCreate.Where(connection => !CheckIfEdgeExists(connection)))
            {
                CreateEdgeView(connection);
            }
        }

        private bool CheckIfEdgeExists(Object connection)
        {
            var edgeViews = new List<EdgeView>();
            foreach (var element in _elements)
            {
                if (element is EdgeView edgeView)
                    edgeViews.Add(edgeView);
            }

            return edgeViews.Any(edgeView => edgeView.Connection == connection);
        }

        private void RemoveOldEdges()
        {
            var edgeViews = new List<EdgeView>();
            foreach (var element in _elements)
            {
                if (element is EdgeView edgeView)
                    edgeViews.Add(edgeView);
            }

            for (var i = edgeViews.Count - 1; i >= 0; i--)
            {
                if (Network.connections.Contains(edgeViews[i].Connection)) 
                    continue;
                
                _elements.Remove(edgeViews[i]);
                RemoveElement(edgeViews[i]);
            }
        }

        private void CreateEdgeView(Connection connection)
        {
            var parentView = FindNodeView(connection.GetParent());
            var childView = FindNodeView(connection.GetChild());

            if (parentView == null || childView == null)
                return;

            var edge = parentView.Output.ConnectTo<EdgeView>(childView.Input);
            edge.OnEdgeSelected = OnEdgeSelected;

            edge.SetConnection(connection);
            
            AddElements(edge);
            AddElement(edge);
        }

        #endregion
    }
}