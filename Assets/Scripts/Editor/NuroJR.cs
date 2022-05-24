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

            // Get Dropdown Field and Subscribe to event
            dropdownField = root.Q<DropdownField>();
            dropdownField.RegisterCallback<ChangeEvent<string>>(OnChangeDropDropdownValue);

            // Clear Choices and Neural Network List
            dropdownField.choices.Clear();
            neuralNetworks.Clear();

            // Refresh Button
            var refreshButton = dropdownField.Q<ToolbarButton>();
            refreshButton.clicked += RefreshDropdownChoices;

            // get neural Network View
            _neuralNetworkView = root.Q<NeuralNetworkView>();
            // subscribe to Selection events
            _neuralNetworkView.OnNodeSelected = OnNodeSelectionChanged;
            _neuralNetworkView.OnLayerSelected = OnLayerSelectionChanged;
            _neuralNetworkView.OnEdgeSelected = OnEdgeSelectionChanged;

            // Get Inspector View
            _inspectorView = root.Q<InspectorView>();

            // Create Stats Button and register event
            var statsButton = root.Q<ToolbarButton>("Stats");
            statsButton.clicked += () => _inspectorView.ShowStats(_neuralNetworkView.NetworkObj);

            // Create New button and subscribe to clicked event
            var newButton = root.Q<ToolbarButton>("new");
            newButton.clicked += () =>
            {
                CreateNeuralNetworks.CreateNewNeuralNetwork();
                RefreshDropdownChoices();
            };
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles new value of DropDown
        /// </summary>
        /// <param name="changeEvent">ChangeEvent string</param>
        private void OnChangeDropDropdownValue(ChangeEvent<string> changeEvent)
        {
            SetView(changeEvent.newValue);
        }

        /// <summary>
        /// Refreshes Dropdown Choices
        /// </summary>
        private void RefreshDropdownChoices()
        {
            dropdownField.choices.Clear();
            neuralNetworks.Clear();
            
            var networks = Resources.FindObjectsOfTypeAll<NeuralNetworkObj>().ToList();
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

        /// <summary>
        /// Set View by String
        /// </summary>
        /// <param name="value"></param>
        private void SetView(string value)
        {
            if (neuralNetworks.Count == 0 || neuralNetworks == null)
                return;
            var index = neuralNetworks.FindIndex(x => x.name == value);
            if (index == -1)
                return;

            if (neuralNetworks[index] == null)
                RefreshDropdownChoices();
            else
            {
                _neuralNetworkView.UnPopulateView();
                _inspectorView.Clear();
                _neuralNetworkView.PopulateView(neuralNetworks[index]);
            }
        }

        #region OnSeletionChnaged

        /// <summary>
        /// Updates InspectorView
        /// </summary>
        /// <param name="neuronView">NeuronView</param>
        private void OnNodeSelectionChanged(NeuronView neuronView)
        {
            _inspectorView.UpdateSelection(neuronView);
        }

        /// <summary>
        /// Updates InspectorView
        /// </summary>
        /// <param name="layerView">LayerView</param>
        private void OnLayerSelectionChanged(LayerView layerView)
        {
            _inspectorView.UpdateSelection(layerView);
        }

        /// <summary>
        /// Updates InspectorView
        /// </summary>
        /// <param name="edgeView">EdgeView</param>
        private void OnEdgeSelectionChanged(EdgeView edgeView)
        {
            _inspectorView.UpdateSelection(edgeView);
        }

        #endregion

        #endregion
    }
}