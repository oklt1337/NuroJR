using System.Collections.Generic;
using Neural_Network.Layer;

namespace Neural_Network
{
    public class LayerComparer : IComparer<NetworkLayer>
    {
        public int Compare(NetworkLayer x, NetworkLayer y)
        {
            if (x != null && x.GetType() == typeof(InputLayer))
            {
                return -1;
            }

            if (y != null && y.GetType() == typeof(InputLayer))
            {
                return 1;
            }

            if (x != null && x.GetType() == typeof(OutputLayer))
            {
                return 1;
            }

            if (y != null && y.GetType() == typeof(OutputLayer))
            {
                return -1;
            }

            return 0;
        }
    }
}