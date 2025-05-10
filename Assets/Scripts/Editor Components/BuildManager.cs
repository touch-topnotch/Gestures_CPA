#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEditor.Callbacks;

public class BuildManager : IPreprocessBuildWithReport
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
        // if (target == BuildTarget.Android)
        // {
        //     try
        //     {
        //         AdbInstaller adbInstaller = new AdbInstaller();
        //
        //         adbInstaller.InstallApk(
        //             "/Users/dmitry057/Projects/platform-tools",
        //             pathToBuiltProject
        //         );
        //     }
        //     catch(Exception e)
        //     {
        //         Debug.LogWarning(e);
        //     }
        // }
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

public class AdbInstaller
{
    public void InstallApk(string sdkPath, string apkPath)
    {
        Debug.Log("Importing apk to the oculus quest ... ");

        string apkFileName = Path.GetFileName(apkPath);
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "/bin/bash";
        startInfo.Arguments = $"-c \"{sdkPath}/adb devices\"";
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;

        using (Process process = Process.Start(startInfo))
        {
            string output = process?.StandardOutput.ReadToEnd();
            if (output != null && output.Contains("device"))
            {
                Debug.Log(output);
                // Copy the APK file to the SDK folder
                startInfo.Arguments = $"-c \"cp {apkPath} {sdkPath}\"";
                Process.Start("/bin/bash", startInfo.Arguments)?.WaitForExit();

                // Install the APK on the connected device
                startInfo.Arguments = $"-c \"{sdkPath}/adb install {apkFileName}\"";
                Process.Start("/bin/bash", startInfo.Arguments)?.WaitForExit();
                output = process?.StandardOutput.ReadToEnd();
                if (!output.Contains("Success"))
                {
                    throw new Exception("Installation is not completed. Check device permissions. " + output);
                }
            }
            else
            {
                throw new Exception("No device connected. Please connect your device to install apk.");
            }
        }
    }
}


#endif