using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

namespace Scripts.Network
{
    
    public static class SessionManager
    {

        private static ushort serverPort;
        public static void ReadCommandArgs(NetworkManager networkManager)
        {
            
            Application.targetFrameRate = 60;
            string[] args = System.Environment.GetCommandLineArgs();
            for (int i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    
                    case "-lobby-server":
                        // /Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -lobby-server -logfile -
                        networkManager.StartServer();
                        break;
                    
                    case "-game-server":
                        // /Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -lobby-client -logfile- & /Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -lobby-client -logfile -
                        
                      //  LoadScene
                        break;

                    case "-client":
                        // /Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -lobby-client -logfile- & /Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -lobby-client -logfile -
                        networkManager.StartClient();
                        break;
                    case "-port":
                        if (i + 1 != args.Length)
                            ushort.TryParse(args[i + 1], out serverPort);
                        break;
                }
            }
        }
        
    }
}