using System;
using Neural_Network;
using UnityEditor.Experimental.GraphView;

namespace Editor
{
    public class EdgeView : Edge
    {
        public Action<EdgeView> OnEdgeSelected;
        public Connection Connection;

        public EdgeView()
        {
            capabilities = Capabilities.Selectable;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnEdgeSelected?.Invoke(this);
        }

        public void SetConnection(Connection connection)
        {
            Connection = connection;
        }
    }
}