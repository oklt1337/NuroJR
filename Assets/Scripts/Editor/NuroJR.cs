using System.Collections.Generic;
using System.Linq;
using Neural_Network;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class NuroJR : EditorWindow
    {
        private static NuroJR _wnd;

        private NeuralNetworkView _neuralNetworkView;
        private InspectorView _inspectorView;

        private DropdownField dropdownField;
        private List<NeuralNetwork> neuralNetworks = new();


        [MenuItem("NuroJR/Editor ...")]
        public static void OpenWindow()
        {
            if (_wnd == null)
            {
                _wnd = GetWindow<NuroJR>();
                _wnd.titleContent = new GUIContent("NuroJR");
            }

            _wnd.Show();
        }

        private void OnInspectorUpdate()
        {
            if (dropdownField.value == null)
                return;

            var index = neuralNetworks.FindIndex(x => x.name == dropdownField.value);
            if (index == -1)
                return;

            if (_neuralNetworkView.Network == null)
            {
                _neuralNetworkView.UnPopulateView();
                _neuralNetworkView.PopulateView(neuralNetworks[index]);
            }

            if (_neuralNetworkView.Network.name == dropdownField.value)
                return;

            _neuralNetworkView.UnPopulateView();
            _neuralNetworkView.PopulateView(neuralNetworks[index]);
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            var root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Scripts/Editor/NuroJR.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Scripts/Editor/NuroJR.uss");
            root.styleSheets.Add(styleSheet);
            
            // DropDownField
            dropdownField = root.Q<DropdownField>();
            var refreshButton = dropdownField.Q<ToolbarButton>();
            refreshButton.clicked += RefreshDropdownValues;
            dropdownField.choices.Clear();
            neuralNetworks.Clear();

            _neuralNetworkView = root.Q<NeuralNetworkView>();
            _inspectorView = root.Q<InspectorView>();

            _neuralNetworkView.OnNodeSelected = OnNodeSelectionChanged;
            _neuralNetworkView.OnLayerSelected = OnLayerSelectionChanged;
            _neuralNetworkView.OnEdgeSelected = OnEdgeSelectionChanged;

            // Apply Button
            var apply = root.Q<ToolbarButton>("apply");
            apply.clicked += _neuralNetworkView.Apply;
            
            // New Button
            var newButton = root.Q<ToolbarButton>("new");
            newButton.clicked += CreateNeuralNetworks.CreateNewNeuralNetwork;
            
            OnSelectionChange();
        }

        private void RefreshDropdownValues()
        {
            dropdownField.choices.Clear();
            var networks = Resources.FindObjectsOfTypeAll<NeuralNetwork>().ToList();
            neuralNetworks.Clear();
            if (networks.Count != 0)
            {
                networks.ForEach(x => dropdownField.choices.Add(x.name));
                neuralNetworks = networks.ToList();

                if (dropdownField.value is not ("NULL" or "" or null))
                    return;

                dropdownField.value = dropdownField.choices[0];
                SetView(dropdownField.value);
            }
            else
            {
                dropdownField.value = "NULL";
            }
        }

        private void SetView(string value)
        {
            if (neuralNetworks.Count == 0 || neuralNetworks == null)
                return;
            var index = neuralNetworks.FindIndex(x => x.name == value);
            if (index == -1)
                return;

            _neuralNetworkView.UnPopulateView();
            _neuralNetworkView.PopulateView(neuralNetworks[index]);
        }

        private void OnSelectionChange()
        {
            /*var network = Selection.activeObject as NeuralNetwork;
            if (network && AssetDatabase.CanOpenAssetInEditor(network!.GetInstanceID()))
            {
                _neuralNetworkView.PopulateView(network);
            }
            else
            {
                _neuralNetworkView.UnPopulateView();
                _inspectorView.Clear();
            }*/
        }

        private void OnNodeSelectionChanged(NeuronView neuronView)
        {
            _inspectorView.UpdateSelection(neuronView);
        }

        private void OnLayerSelectionChanged(LayerView layerView)
        {
            _inspectorView.UpdateSelection(layerView);
        }

        private void OnEdgeSelectionChanged(EdgeView edgeView)
        {
            _inspectorView.UpdateSelection(edgeView);
        }
    }
}