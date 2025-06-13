#if UNITY_EDITOR
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Scripts.Systems
{
    public class ScriptableObjectsUpdater: MonoBehaviour
    {
        [SerializeField] private string defaultNetworkPrefabsPath = "Configs/ChangableNetworkPrefabs.asset";
        [SerializeField] private bool updateOnAwake;
        private NetworkPrefabsList networkPrefabsList;
        public void Awake()
        {
            if(updateOnAwake)
                UpdateScriptableObjects();
        }

        #if UNITY_EDITOR
        [Sirenix.OdinInspector.Button("Update")]
        public void UpdateInInspector()
        {
            UpdateScriptableObjects();
        }
        #endif
        public void UpdateScriptableObjects()
        {
        
        }

        public void UpdateNetworkPrefabList()
        {
            // find ScriptableObject - ChangableNetworkPrefabs in Resources
            networkPrefabsList ??= Resources.Load(defaultNetworkPrefabsPath) as NetworkPrefabsList;
            if (!networkPrefabsList)
                return;
            
            // find all NetworkPrefabs in the Resources folder

            string[] guids = AssetDatabase.FindAssets("t:GameObject");
            foreach (var guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);

                if (go.TryGetComponent(out NetworkObject _))
                {
                    networkPrefabsList.Add(new NetworkPrefab { Prefab = go });
                }
            }
            //  networkPrefabsList.Add(test);


        }
    }
}
#endif