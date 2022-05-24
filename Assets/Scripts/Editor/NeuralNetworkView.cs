using System;
using System.Collections.Generic;
using System.Linq;
using Neural_Network;
using Neural_Network.Connection;
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
        public new class UxmlFactory : UxmlFactory<NeuralNetworkView, UxmlTraits> {}

        public NeuralNetworkObj NetworkObj;

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

        /// <summary>
        /// Display View of Neural Network Object
        /// </summary>
        /// <param name="neuralNetworkObj">NeuralNetworkObj</param>
        public void PopulateView(NeuralNetworkObj neuralNetworkObj)
        {
            NetworkObj = neuralNetworkObj;
            neuralNetworkObj.GenerateNewGuids();

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            neuralNetworkObj.OnLayerCreated += CreateLayerView;
            onNeuronViewCreated += AddElements;
            onNeuronViewCreated += CreateEdge;

            InitializeView(neuralNetworkObj);
            InitializeEvents(neuralNetworkObj);
        }

        /// <summary>
        /// Clears View
        /// </summary>
        public void UnPopulateView()
        {
            RemoveEdgeViews();
            RemoveNeuronViews();
            RemoveLayerViews();
        }
        
        /// <summary>
        /// Fill view with data
        /// </summary>
        /// <param name="neuralNetworkObj">NeuralNetworkObj</param>
        private void InitializeView(NeuralNetworkObj neuralNetworkObj)
        {
            neuralNetworkObj.GetLayer().ForEach(CreateLayerView);
            neuralNetworkObj.GetLayer().ForEach(RestoreNeuronView);
            neuralNetworkObj.GetConnections().ForEach(CreateEdgeView);
        }

        /// <summary>
        /// Reconnect events
        /// </summary>
        /// <param name="neuralNetworkObj">NeuralNetworkObj</param>
        private static void InitializeEvents(NeuralNetworkObj neuralNetworkObj)
        {
            // Reconnect layer Events
            foreach (var layerObj in neuralNetworkObj.layersObj)
            {
                neuralNetworkObj.ConnectEvents(layerObj);
            }
        }

        /// <summary>
        /// Remove all EdgeViews 
        /// </summary>
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

        /// <summary>
        /// Remove all NeuronViews
        /// </summary>
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

        /// <summary>
        /// Remove all LayerViews
        /// </summary>
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

        /// <summary>
        /// Add GraphElement to List
        /// </summary>
        /// <param name="graphElement">GraphElement</param>
        private void AddElements(GraphElement graphElement)
        {
            if (graphElement == null)
                return;
            if (_elements.Contains(graphElement))
                return;
            _elements.Add(graphElement);
        }

        /// <summary>
        /// Handles the Change of the GraphView
        /// </summary>
        /// <param name="graphviewchange">GraphViewChange</param>
        /// <returns>GraphViewChange</returns>
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphviewchange)
        {
            graphviewchange.elementsToRemove?.ForEach(element =>
            {
                switch (element)
                {
                    case LayerView layerView:
                        if (layerView.NetworkLayerObj.GetType() == typeof(HiddenLayerObj))
                        {
                            NetworkObj.RemoveLayer(layerView.NetworkLayerObj);
                            SortLayer();
                        }

                        break;
                    case NeuronView nodeView:
                        var index = NetworkObj.layersObj.FindIndex(x => x.neurons.Contains(nodeView.NeuronObj));
                        if (index == -1)
                            return;
                        if (NetworkObj.layersObj[index].neurons.Count == 1)
                            return;

                        NetworkObj.layersObj[index].RemoveNeuron(nodeView.NeuronObj);
                        SortNeurons();
                        break;
                    case Edge edge:
                    {
                        var parentView = edge.output.node as NeuronView;
                        var childView = edge.input.node as NeuronView;
                        var parentNeuron = parentView!.NeuronObj;
                        switch (parentNeuron)
                        {
                            case InputNeuronObj neuron:
                                neuron.RemoveChild(childView!.NeuronObj);
                                break;
                            case HiddenNeuronObj neuron:
                                neuron.RemoveChild(childView!.NeuronObj);
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
                var parentNeuron = parentView!.NeuronObj;
                switch (parentNeuron)
                {
                    case InputNeuronObj neuron:
                        neuron.AddChild(childView!.NeuronObj);
                        break;
                    case HiddenNeuronObj neuron:
                        neuron.AddChild(childView!.NeuronObj);
                        break;
                }
            });

            return graphviewchange;
        }

        /// <summary>
        /// Create Context Menu for Creating Hidden Layer
        /// </summary>
        /// <param name="evt">ContextualMenuPopulateEvent</param>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            {
                var type = typeof(HiddenLayerObj);
                evt.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", _ => CreateHiddenLayer());
            }
        }

        /// <summary>
        /// Get List of Compatible Ports
        /// </summary>
        /// <param name="startPort">Port</param>
        /// <param name="nodeAdapter">NodeAdapter</param>
        /// <returns>List Port</returns>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
        }

        #endregion

        #region Layer

        /// <summary>
        /// Create Hidden Layer
        /// </summary>
        private void CreateHiddenLayer()
        {
            if (NetworkObj == null)
                return;
            NetworkObj.CreateLayer(typeof(HiddenLayerObj));
        }

        /// <summary>
        /// Create Layer View
        /// </summary>
        /// <param name="networkLayerObj">NetworkLayerObj</param>
        private void CreateLayerView(NetworkLayerObj networkLayerObj)
        {
            var layerView = new LayerView(networkLayerObj)
            {
                OnLayerSelected = OnLayerSelected,
                OnNodeSelected = OnNodeSelected,
                OnNeuronViewCreated = onNeuronViewCreated,
                expanded = false
            };

            NetworkObj.OnConnectionCreated += layerView.CreateNeuronView;

            if (networkLayerObj.GetType() == typeof(InputLayerObj))
            {
                layerView.style.backgroundColor = Color.green;
            }
            else if (networkLayerObj.GetType() == typeof(OutputLayerObj))
            {
                layerView.style.backgroundColor = Color.red;
            }

            AddElements(layerView);
            AddElement(layerView);
            SortLayer();
        }

        /// <summary>
        /// Find Layer View by GUID
        /// </summary>
        /// <param name="layerObj">NetworkLayerObj</param>
        /// <returns>LayerView</returns>
        private LayerView FindLayerView(NetworkLayerObj layerObj)
        {
            return GetNodeByGuid(layerObj.guid) as LayerView;
        }

        /// <summary>
        /// Sort Layer
        /// </summary>
        private void SortLayer()
        {
            if (NetworkObj.GetLayer().Count < 2)
                return;

            var layers = new List<NetworkLayerObj>();
            foreach (var layer in NetworkObj.GetLayer().Where(layer => layer != null))
            {
                layers.Add(layer);

                var index = NetworkObj.GetLayer().FindIndex(l => l == layer);
                if (index != -1)
                {
                    switch (layer)
                    {
                        case InputLayerObj inputLayer:
                            inputLayer.position.x = index * 150;
                            break;
                        case OutputLayerObj outputLayer:
                            outputLayer.position.x = index * 150;
                            break;
                        case HiddenLayerObj hiddenLayer:
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

        /// <summary>
        /// Restores Neuron View
        /// </summary>
        /// <param name="layerObj">NetworkLayerObj</param>
        private void RestoreNeuronView(NetworkLayerObj layerObj)
        {
            var layerView = FindLayerView(layerObj);
            foreach (var neuron in layerObj.GetNeurons())
            {
                layerView.CreateNeuronView(neuron);
            }
        }

        /// <summary>
        /// Find Neuron View by GUID
        /// </summary>
        /// <param name="neuronObj">NeuronObj</param>
        /// <returns>NeuronView</returns>
        private NeuronView FindNeuronView(NeuronObj neuronObj)
        {
            return GetNodeByGuid(neuronObj.guid) as NeuronView;
        }

        /// <summary>
        /// Sort Neurons
        /// </summary>
        private void SortNeurons()
        {
            var neurons = new List<NeuronObj>();

            foreach (var layer in NetworkObj.GetLayer())
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

            foreach (var nodeView in neurons.Select(FindNeuronView))
            {
                nodeView.RemapView();
            }
        }

        #endregion

        #region Edge

        /// <summary>
        /// Create Edge
        /// </summary>
        /// <param name="neuronView">NeuronView</param>
        private void CreateEdge(NeuronView neuronView)
        {
            var neuron = neuronView.NeuronObj;

            // Check if 1st Neuron in Layer
            var index = NetworkObj.layersObj.FindIndex(x => x.neurons.Contains(neuron));
            if (index == -1)
                return;
            if (NetworkObj.layersObj[index].neurons.Count == 1)
            {
                RemoveOldEdges();
            }

            var connectionsToCreate = NetworkObj.connectionsObj
                .Where(connection => connection.GetChild() == neuron || connection.GetParent() == neuron).ToList();

            foreach (var connection in connectionsToCreate.Where(connection => !CheckIfEdgeExists(connection)))
            {
                CreateEdgeView(connection);
            }
        }

        /// <summary>
        /// Check if Edge already Exists
        /// </summary>
        /// <param name="connection">Object</param>
        /// <returns>true == exists</returns>
        private bool CheckIfEdgeExists(Object connection)
        {
            var edgeViews = new List<EdgeView>();
            foreach (var element in _elements)
            {
                if (element is EdgeView edgeView)
                    edgeViews.Add(edgeView);
            }

            return edgeViews.Any(edgeView => edgeView.ConnectionObj == connection);
        }

        /// <summary>
        /// Remove old Edges
        /// </summary>
        private void RemoveOldEdges()
        {
            var edgeViews = (from element in _elements
                where element.GetType() == typeof(EdgeView)
                select element as EdgeView).ToList();

            for (var i = edgeViews.Count - 1; i >= 0; i--)
            {
                if (NetworkObj.connectionsObj.Contains(edgeViews[i].ConnectionObj))
                    continue;

                _elements.Remove(edgeViews[i]);
                RemoveElement(edgeViews[i]);
            }
        }

        /// <summary>
        /// Create EdgeView
        /// </summary>
        /// <param name="connectionObj">ConnectionObj</param>
        private void CreateEdgeView(ConnectionObj connectionObj)
        {
            var parentView = FindNeuronView(connectionObj.GetParent());
            var childView = FindNeuronView(connectionObj.GetChild());

            if (parentView == null || childView == null)
                return;

            var edge = parentView.Output.ConnectTo<EdgeView>(childView.Input);
            edge.OnEdgeSelected = OnEdgeSelected;
            edge.SetConnection(connectionObj);
            edge.ConnectionObj.OnDeleted += RemoveEdge;

            AddElements(edge);
            AddElement(edge);
        }

        /// <summary>
        /// Remove Old Edges
        /// </summary>
        /// <param name="connection"></param>
        private void RemoveEdge(ConnectionObj connection)
        {
            RemoveOldEdges();
        }

        #endregion
    }
}