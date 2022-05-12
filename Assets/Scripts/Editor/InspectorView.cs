using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> {}

        private UnityEditor.Editor _editor;

        public void UpdateSelection(NeuronView neuronView)
        {
            Clear();

            Object.DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(neuronView.Neuron);

            var container = new IMGUIContainer(() =>
            {
                _editor.OnInspectorGUI();
            });
            Add(container);
        }
        
        public void UpdateSelection(LayerView layerView)
        {
            Clear();

            Object.DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(layerView.NetworkLayer);

            if (_editor.target == null)
            {
                Clear();
                return;
            }
            
            var container = new IMGUIContainer(() =>
            {
                _editor.OnInspectorGUI();
            });
            Add(container);
        }
        
        public void UpdateSelection(EdgeView edgeView)
        {
            Clear();
            
            Object.DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(edgeView.Connection);

            if (_editor.target == null)
            {
                Clear();
                return;
            }
            
            var container = new IMGUIContainer(() =>
            {
                _editor.OnInspectorGUI();
            });
            Add(container);
        }
        
    }
}