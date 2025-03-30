using System;
using System.Net;
using System.Text;
using Scripts.Static;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace Scripts.GameControllers
{
    public class ServerBrowser: MonoBehaviour
    {
        
        [SerializeField] private Transform serverContainer;
        [SerializeField] private Transform serverTemplate;
        private const string keyId = "1a49ee8c-66a4-4a3d-a208-e1e286f2cbd5";
        private const string keySecret = "8bGE6xx28AM1YBwGwSFc3U17GLxC8mDV";
    
        public static void ConnectToServer()
        {
            #if DEDICATED_SERVER
                return;
            #endif
            byte[] keyByteArray = Encoding.UTF8.GetBytes(keyId + " : " + keySecret);
            string keyBase64 = Convert.ToBase64String(keyByteArray);
            
            string url =
                $"https://services.api.unity.com/auth/v1/token-exchange?projectId={CustomPaths.projectId}&environmentId={CustomPaths.environmentId}";

            WebRequests.Get(url,
                (UnityWebRequest unityWebRequest) =>
                {
                    unityWebRequest.SetRequestHeader("Authorization", "Basic " + keyBase64);
                },
                (string error) => { Debug.Log("Error: " + error); },
                (string json) =>
                {
                    Debug.Log("Success: " + json);
                    ListServers listServers = JsonUtility.FromJson<ListServers>("{\"serverList\":" + json + "}");
                    foreach (Server server in listServers.serverList)
                    {
                        //Debug.Log(server.ip + " : " + server.port + " " + server.deleted + " " + server.status);
                        if (server.status == ServerStatus.ONLINE.ToString() ||
                            server.status == ServerStatus.ALLOCATED.ToString())
                        {
                            // Server is Online!
                            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(server.ip, (ushort)server.port);
                            NetworkManager.Singleton.StartClient();
                            // Transform serverTransform = Instantiate(serverTemplate, serverContainer);
                            // serverTransform.gameObject.SetActive(true);
                            // serverTransform.GetComponent<ServerBrowserSingleUI>().SetServer(
                            //     server.ip,
                            //     (ushort)server.port
                            // );
                        }
                    }
                }
            );
        }

        private enum ServerStatus
        {
            AVAILABLE,
            ONLINE,
            ALLOCATED
        }

        [Serializable]
        public class ListServers
        {
            public Server[] serverList;
        }

        [Serializable]
        public class Server
        {
            public int buildConfigurationID;
            public string buildConfigurationName;
            public string buildName;
            public bool deleted;
            public string fleetID;
            public string fleetName;
            public string hardwareType;
            public int id;
            public string ip;
            public int locationID;
            public string locationName;
            public int machineID;
            public int port;
            public string status;
        }
    }

}
