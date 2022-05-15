using System.Linq;
using Neural_Network;
using Neural_Network.Layer;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class CreateNeuralNetworks
    {
        public static void CreateNewNeuralNetwork()
        {
            if (AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/Resources/Neural_Networks"))
                    AssetDatabase.CreateFolder("Assets/Resources", "Neural_Networks");
            }
            else
            {
                AssetDatabase.CreateFolder("Assets/", "Resources");
                AssetDatabase.CreateFolder("Assets/Resources", "Neural_Networks");
            }
            
            var networkName = CheckName();
            var path = $"Assets/Resources/Neural_Networks/{networkName}.asset";
            Debug.Log(networkName);
            
            var network = ScriptableObject.CreateInstance<NeuralNetwork>();
            AssetDatabase.CreateAsset(network, path);
            AssetDatabase.SaveAssets();

            network.CreateLayer(typeof(InputLayer));
            network.CreateLayer(typeof(OutputLayer));
        }

        private static string CheckName()
        {
            var networks = Resources.FindObjectsOfTypeAll<NeuralNetwork>();
            const string networkName = "New Neural Network";
            var finalName = networkName;
            var count = 0;
            var free = false;
            
            while (!free)
            {
                var exists = false;
                if (networks.Any(network => network.name == finalName))
                {
                    count++;
                    finalName = $"{networkName}_{count}";
                    exists = true;
                }

                if (!exists)
                    free = true;
            }
            return finalName;
        }
    }
}