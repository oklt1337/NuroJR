using System.Collections.Generic;

namespace Neural_Network.Layer
{
    public class LayerComparer : IComparer<NetworkLayerObj>
    {
        /// <summary>
        /// Compare two NetworkLayerObj
        /// </summary>
        /// <param name="x">NetworkLayerObj</param>
        /// <param name="y">NetworkLayerObj</param>
        /// <returns>int</returns>
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