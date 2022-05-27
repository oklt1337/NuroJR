using System;
using Model.Connection;
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

        /// <summary>
        /// Triggers OnEdgeSelected event
        /// </summary>
        public override void OnSelected()
        {
            base.OnSelected();
            OnEdgeSelected?.Invoke(this);
        }

        /// <summary>
        /// Sets the Connection Object
        /// </summary>
        /// <param name="connectionObj">ConnectionObj</param>
        public void SetConnection(ConnectionObj connectionObj)
        {
            ConnectionObj = connectionObj;
        }
    }
}