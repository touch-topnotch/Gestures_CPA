#if UNITY_EDITOR
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;

using UnityEditor.Callbacks;

public class BuildManager: IPreprocessBuildWithReport
{

    private static string[] commands =
    {
        "/Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -client -n ",
        "/Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -client -n ",
        "/Users/dmitry057/Projects/UnityProjects/Gestures_CPA/Builds/NetworkTest.app/Contents/MacOS/Gesture -lobby-server -n "
    };
    
    public int callbackOrder { get; }
    public void OnPreprocessBuild(BuildReport report)
    {
        StopSCC();
    }
    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        Debug.Log("Running commands:");
      //  RunSCC();
    }
    [MenuItem("Testing/RunSCC")]
    static void RunSCC()
    {
        string command = "";
        for (int i = 0; i < commands.Length; i++)
        {
            if (i == 0)
            {
                command += commands[i];
            }
            else
            {
                command += " & " + commands[i];
            }
        }
        RunTerminalCommand(command);
    }
    
    
    static void RunTerminalCommand(string command)
    {
        Debug.Log(command);
        Process process = new Process();
        process.StartInfo.FileName = "/bin/bash";
        process.StartInfo.Arguments = $"-c \"{command}\"";
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;
        process.Start();
        process.Close();
    }
    [MenuItem("Testing/StopSCC")]
    static void StopSCC()
    {
        RunTerminalCommand("pgrep -f NetworkTest | xargs kill");
    }


   
}
#endif