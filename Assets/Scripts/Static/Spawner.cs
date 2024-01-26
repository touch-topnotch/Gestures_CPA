using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
namespace Scripts.Static
{
    public class Spawner: MonoInstaller
    {
        public static GameObject SpawnInjectedPrefab(GameObject prefab, DiContainer container, Transform parent = null,
            bool hasParent = false)
        {
            var instPrefab = container.InstantiatePrefab(prefab, parent);
            if (!hasParent)
                instPrefab.transform.SetParent(null);
            return instPrefab;
        }

        public static GameObject SpawnPrefab(GameObject prefab, Transform parent = null,
            bool hasParent = false)
        {
            return Instantiate(prefab, parent, !hasParent);
        }
        
        public static GameObject SpawnPooledPrefab(GameObject prefab, Transform parent = null, bool enbaled = false)
        {
            var instPrefab = Instantiate(prefab, parent);
            instPrefab.SetActive(enbaled);
            return instPrefab;
        }
        

        public static T TryGetComponent<T>(in Transform prefab, out T component)
            where T : Component

        {
            Assert.IsNotNull(prefab.GetComponent<T>());
            component = prefab.GetComponent<T>();
            return component;
        }

    }
}