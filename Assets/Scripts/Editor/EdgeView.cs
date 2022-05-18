using System;
using Neural_Network;
using UnityEditor.Experimental.GraphView;

namespace Editor
{
    public class EdgeView : Edge
    {
        public Action<EdgeView> OnEdgeSelected;
        public ConnectionObj ConnectionObj;

        public EdgeView()
        {
            capabilities = Capabilities.Selectable;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnEdgeSelected?.Invoke(this);
        }

        public void SetConnection(ConnectionObj connectionObj)
        {
            ConnectionObj = connectionObj;
        }
    }
}