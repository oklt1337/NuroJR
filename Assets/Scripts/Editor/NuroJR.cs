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
        private List<NeuralNetworkObj> neuralNetworks = new();

        #region Editor Methods

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
            dropdownField.choices.Clear();
            neuralNetworks.Clear();
            dropdownField.RegisterCallback<ChangeEvent<string>>(OnChangeDropDropdownValue);
            
            // Refresh Button
            var refreshButton = dropdownField.Q<ToolbarButton>();
            refreshButton.clicked += RefreshDropdownValues;

            _neuralNetworkView = root.Q<NeuralNetworkView>();
            _inspectorView = root.Q<InspectorView>();
            
            // Create Stats Button and register event
            var statsButton = root.Q<ToolbarButton>("Stats");
            statsButton.clicked += () => _inspectorView.ShowStats(_neuralNetworkView.NetworkObj);

            _neuralNetworkView.OnNodeSelected = OnNodeSelectionChanged;
            _neuralNetworkView.OnLayerSelected = OnLayerSelectionChanged;
            _neuralNetworkView.OnEdgeSelected = OnEdgeSelectionChanged;

            // New Button
            var newButton = root.Q<ToolbarButton>("new");
            newButton.clicked += () =>
            {
                CreateNeuralNetworks.CreateNewNeuralNetwork();
                RefreshDropdownValues();
            };
        }

        #endregion

        #region Private Methods

        private void OnChangeDropDropdownValue(ChangeEvent<string> changeEvent)
        {
            var index = neuralNetworks.FindIndex(x => x.name == changeEvent.newValue);
            if (index == -1)
                return;

            if (neuralNetworks[index] == null)
                RefreshDropdownValues();
            else
            {
                _neuralNetworkView.UnPopulateView();
                _inspectorView.Clear();
                _neuralNetworkView.PopulateView(neuralNetworks[index]);
            }
        }

        private void RefreshDropdownValues()
        {
            dropdownField.choices.Clear();
            var networks = Resources.FindObjectsOfTypeAll<NeuralNetworkObj>().ToList();
            neuralNetworks.Clear();
            if (networks.Count != 0)
            {
                networks.ForEach(x => dropdownField.choices.Add(x.name));
                neuralNetworks = networks;

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
            _inspectorView.Clear();
            _neuralNetworkView.PopulateView(neuralNetworks[index]);
        }

        #region OnSeletionChnaged

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

        #endregion
        
        #endregion
    }
}