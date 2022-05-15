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

namespace Editor
{
    public class NeuralNetworkView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<NeuralNetworkView, UxmlTraits>
        {
        }

        public Action<NeuronView> OnNodeSelected;
        public Action<LayerView> OnLayerSelected;
        public Action<EdgeView> OnEdgeSelected;

        public NeuralNetwork Network;

        private readonly List<EdgeView> _edgeViews = new();
        private readonly List<GraphElement> _elements = new();

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

        public void PopulateView(NeuralNetwork network)
        {
            Network = network;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            InitializeEvents(network);
            InitializeView(network);
        }

        public void UnPopulateView()
        {
            _elements.ForEach(RemoveElement);
        }

        private void InitializeView(NeuralNetwork network)
        {
            CreateInputLayerAndOutputLayer();
            network.GetLayer().ForEach(CreateLayerView);
            network.GetLayer().ForEach(RestoreNeuronView);
            network.GetConnections().ForEach(CreateEdgeView);
        }

        private void InitializeEvents(NeuralNetwork network)
        {
            network.OnLayerCreated += CreateLayerView;
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
                        foreach (var networkLayer in Network.GetLayer().Where(networkLayer =>
                                     networkLayer.GetNeurons().Contains(nodeView.Neuron)))
                        {
                            networkLayer.RemoveNeuron(nodeView.Neuron);
                            SortNeurons();
                        }

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
                evt.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", _ => CreateLayer(type));
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

        private void CreateLayer(Type type)
        {
            if (Network == null)
                return;
            Network.CreateLayer(type);
        }

        private void OnLayerDelete(NetworkLayer networkLayer)
        {
            Debug.Log($"{networkLayer.name} has been deleted.");
            //Remove Connections and Reconnect 
        }

        private void CreateLayerView(NetworkLayer networkLayer)
        {
            Debug.Log($"Layer View Creation for Layer: {networkLayer.name}");

            var layerView = new LayerView(networkLayer)
            {
                OnLayerSelected = OnLayerSelected,
                OnNodeSelected = OnNodeSelected,
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

            networkLayer.OnDelete += OnLayerDelete;
            layerView.OnNeuronViewCreation += CheckEdges;
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

        private void CheckEdges()
        {
            var listOfConnectionsExisting = _edgeViews.Select(edgeView => edgeView.Connection).ToList();
            foreach (var connection in Network.connections.Where(connection => !listOfConnectionsExisting.Contains(connection)))
            {
                CreateEdgeView(connection);
            }
        }

        private void CreateEdgeView(Connection connection)
        {
            var parentView = FindNodeView(connection.GetParent());
            var childView = FindNodeView(connection.GetChild());


            var edge = parentView.Output.ConnectTo<EdgeView>(childView.Input);
            edge.OnEdgeSelected = OnEdgeSelected;

            edge.SetConnection(connection);
            _edgeViews.Add(edge);
            AddElement(edge);
        }

        #endregion

        #region Checks

        private void CreateInputLayerAndOutputLayer()
        {
            if (Network == null)
                return;

            var inputLayer = Network.GetLayer().OfType<InputLayer>();
            var outputLayer = Network.GetLayer().OfType<OutputLayer>();

            if (inputLayer.Any() && outputLayer.Any())
                return;

            Network.CreateLayer(typeof(InputLayer));
            Network.CreateLayer(typeof(OutputLayer));

            EditorUtility.SetDirty(Network);
            AssetDatabase.SaveAssets();
        }

        #endregion
    }
}