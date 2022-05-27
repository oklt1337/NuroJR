using Model;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> {}

        private UnityEditor.Editor _editor;

        /// <summary>
        /// Fill Editor with Neural Network Object Stats
        /// </summary>
        /// <param name="neuralNetworkObj">NeuralNetworkObj</param>
        public void ShowStats(NeuralNetworkObj neuralNetworkObj)
        {
            Clear();

            Object.DestroyImmediate(_editor);
            if (neuralNetworkObj == null)
                return;

            _editor = UnityEditor.Editor.CreateEditor(neuralNetworkObj);

            if (_editor == null)
                return;
            if (_editor.target == null)
            {
                Clear();
                return;
            }

            var container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            Add(container);
        }

        /// <summary>
        /// Fill Editor with Neuron Object
        /// </summary>
        /// <param name="neuronView">NeuronView</param>
        public void UpdateSelection(NeuronView neuronView)
        {
            Clear();

            Object.DestroyImmediate(_editor);
            if (neuronView == null)
                return;

            _editor = UnityEditor.Editor.CreateEditor(neuronView.NeuronObj);

            if (_editor == null)
                return;
            if (_editor.target == null)
            {
                Clear();
                return;
            }

            var container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            Add(container);
        }

        /// <summary>
        /// Fill Editor with Network Layer Object
        /// </summary>
        /// <param name="layerView">LayerView</param>
        public void UpdateSelection(LayerView layerView)
        {
            Clear();

            Object.DestroyImmediate(_editor);

            if (layerView == null)
                return;
            _editor = UnityEditor.Editor.CreateEditor(layerView.NetworkLayerObj);

            if (_editor == null)
                return;
            if (_editor.target == null)
            {
                Clear();
                return;
            }

            var container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            Add(container);
        }

        /// <summary>
        /// Fill Editor with Connection Object
        /// </summary>
        /// <param name="edgeView">EdgeView</param>
        public void UpdateSelection(EdgeView edgeView)
        {
            Clear();

            Object.DestroyImmediate(_editor);

            if (edgeView == null)
                return;
            _editor = UnityEditor.Editor.CreateEditor(edgeView.ConnectionObj);

            if (_editor == null)
                return;
            if (_editor.target == null)
            {
                Clear();
                return;
            }

            var container = new IMGUIContainer(() => { _editor.OnInspectorGUI(); });
            Add(container);
        }
    }
}