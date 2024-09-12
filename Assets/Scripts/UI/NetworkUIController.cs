using Scripts.Network;
using UnityEngine;
using UnityEngine.UI;
namespace Scripts.UI
{
    public class NetworkUIController : MonoBehaviour
    {
        public ConnectToSession networkManager;
        public Button startLobbyServerButton;
        public Button startClientButton;

        private void Start()
        {
            startLobbyServerButton.onClick.AddListener(networkManager.StartLobbyServerSession);
            startClientButton.onClick.AddListener(networkManager.StartClientSession);
        }
    }

}