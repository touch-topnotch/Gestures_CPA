
using Scripts.PlayerLogic;
using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Installers
{
    public class RigInstaller: MonoInstaller
    {
        [SerializeField] private bool instantiate;
        
        [SerializeField] private RigType type;

        [SerializeField] private Rig spawnedRig;
        
        
        // lets spawn player at first and after connection add him to NetworkUser
        public override void InstallBindings()
        {
            Rig rig;
            if (!instantiate)
            {
                rig = spawnedRig;
            }
            else
            {
                if (type == RigType.NoRig)
                {
                    Debug.Log("Rig is not initialized in SceneContext");
                    return;
                }

                rig = SpawnRig(type == RigType.PCRig
                    ? Resources.Load("Players/PC Rig") as GameObject
                    : SetPlayerByPlatform(), Container);
            }

            Container.Bind<Rig>().FromInstance(rig).AsSingle();
            DontDestroyOnLoad(rig.gameObject);
            //_xrPlayer.Initialize();
        }

        private Rig SpawnRig(in GameObject player, in DiContainer container)
        {
            return Spawner.SpawnInjectedPrefab(player, container).GetComponent<Rig>();
        }

        private GameObject SetPlayerByPlatform()
        {
            Debug.Log("LOADING XR RIG PREFAB");
            return Resources.Load("Players/XR Rig") as GameObject;
            
            // if (XRGeneralSettings.Instance.Manager.activeLoader)
            // {
            //     return Resources.Load("Players/XR Rig") as GameObject;
            // }
            // else
            // {
            //     print("Open XR is not supported on your platform. Initializing PC rig..");
            //     return Resources.Load("Players/PC Rig") as GameObject;
            // }
        }
    }
}
// we have a SpawnerNetUser script, which create any users (our or not, doesn't matter)
// in playerInstaller we should give our NetworkSpawner component