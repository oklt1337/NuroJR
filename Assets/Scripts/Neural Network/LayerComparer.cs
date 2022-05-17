using System.Collections.Generic;
using Neural_Network.Layer;

namespace Neural_Network
{
    public class LayerComparer : IComparer<NetworkLayerObj>
    {
        public int Compare(NetworkLayerObj x, NetworkLayerObj y)
        {
            if (x != null && x.GetType() == typeof(InputLayerObj))
            {
                return -1;
            }

            if (y != null && y.GetType() == typeof(InputLayerObj))
            {
                return 1;
            }

            if (x != null && x.GetType() == typeof(OutputLayerObj))
            {
                return 1;
            }

            if (y != null && y.GetType() == typeof(OutputLayerObj))
            {
                return -1;
            }

            return 0;
        }
    }
}