using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Network
{
    
    public class ConnectToSession: MonoBehaviour
    {
        public void Awake()
        {
            Application.targetFrameRate = 60;
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "-lobby-server": 
                                          // /Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -lobby-server -logfile -
                        
                        StartLobbyServerSession();
                        break;
                    case "-client": 
                                        // /Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -lobby-client -logfile- & /Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -lobby-client -logfile -
                        
                        StartClientSession();
                        break;
                }
            }
        }
        public void StartLobbyServerSession()
        {
            SceneManager.LoadScene("DedicatedServers/LobbyServer/LobbyServerScene/LobbyServer", LoadSceneMode.Single);
        }

        public void StartClientSession()
        {
            SceneManager.LoadScene("Assets/Scenes/GameScenes/LobbyScene/LobbyClient.unity", LoadSceneMode.Single);
        }
    }
}