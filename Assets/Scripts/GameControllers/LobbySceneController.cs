using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.GameControllers
{
    public class LobbySceneController: MonoBehaviour
    {
        public void GoToDistanceRecordingScene()
        {
            SwitchSceneServerRpc("Scenes/MeshGeneration");
        }

        [ServerRpc]
        private void SwitchSceneServerRpc(string path)
        { 
            if (!NetworkManager.Singleton.IsServer)
            {
                return;
            }
            var status = NetworkManager.Singleton.SceneManager.LoadScene(path, LoadSceneMode.Single);
            if (status != SceneEventProgressStatus.Started)
            {
                Debug.LogWarning($"Failed to load {path} " +
                                 $"with a {nameof(SceneEventProgressStatus)}: {status}");
            }
        }

        public void GoToGestureRecordingScene()
        {
            
        }
    }
}