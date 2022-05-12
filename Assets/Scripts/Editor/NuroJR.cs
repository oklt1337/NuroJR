using Neural_Network;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class NuroJR : EditorWindow
    {
        private NeuralNetworkView _neuralNetworkView;
        private InspectorView _inspectorView;
        
        [MenuItem("NuroJR/Editor ...")]
        public static void OpenWindow()
        {
            var wnd = GetWindow<NuroJR>();
            wnd.titleContent = new GUIContent("NuroJR");
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

            _neuralNetworkView = root.Q<NeuralNetworkView>();
            _inspectorView = root.Q<InspectorView>();
            
            _neuralNetworkView.OnNodeSelected = OnNodeSelectionChanged;
            _neuralNetworkView.OnLayerSelected = OnLayerSelectionChanged;
            _neuralNetworkView.OnEdgeSelected = OnEdgeSelectionChanged;

            OnSelectionChange();
        }

        private void OnSelectionChange()
        {
            var network = Selection.activeObject as NeuralNetwork;

            if (network && AssetDatabase.CanOpenAssetInEditor(network.GetInstanceID()))
            {
                _neuralNetworkView.PopulateView(network);
            }
            else
            {
                _neuralNetworkView.UnPopulateView();
                _inspectorView.Clear();
            }
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