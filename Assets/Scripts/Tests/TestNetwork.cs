using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace Scripts.Tests
{
    public class NetworkUIController : MonoBehaviour
    {
        public NetworkManager networkManager;
        public Button startServerButton;
        public Button startClientButton;
        public GameObject TestObj;

        private void Start()

        {
            startServerButton.onClick.AddListener(() =>
            {
                networkManager.StartServer();
                networkManager.ConnectionApprovalCallback += (request, response) =>
                {
                    OnServer();
                    SpawnObj();
                };
            });
            startClientButton.onClick.AddListener(() =>
            {
                networkManager.StartClient();
                networkManager.ConnectionApprovalCallback += (request, response) => { OnClient(); };
            });
        }

        public void OnServer()
        {
            print("server found client");
        }

        public void SpawnObj()
        {
            var inst = Instantiate(TestObj);
            inst.GetComponent<NetworkObject>().Spawn();
        }

        public void OnClient()
        {
            print("client found server");
        }
    }
}