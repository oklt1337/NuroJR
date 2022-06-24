using Model;
using UnityEditor;

namespace Editor
{
    public class NeuralNetworkObjModificationProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string assetPath, RemoveAssetOptions options)
        {
            var network = AssetDatabase.LoadMainAssetAtPath(assetPath) as NeuralNetworkObj;
            if (network == null)
            {
                return AssetDeleteResult.DidNotDelete;
            }

            network.Delete();
            return AssetDeleteResult.DidNotDelete;
        }
    }
}