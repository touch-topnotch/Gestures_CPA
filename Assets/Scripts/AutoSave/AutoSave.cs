#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public class AutoSave
{
    private static DateTime nextSaveTime;
    
    // Static constructor that gets called when unity fires up.
    static AutoSave()
    {
        EditorApplication.playModeStateChanged += (PlayModeStateChange state) => {
            // If we're about to run the scene...
            if (!EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isPlaying) return;
        
            // Save the scene and the assets.
            EditorSceneManager.SaveOpenScenes();
            AssetDatabase.SaveAssets();
        };
        
        // Also, every five minutes.
        nextSaveTime = DateTime.Now.AddMinutes(0.5f);
        EditorApplication.update += Update;
    }

    private static void Update()
    {
        if (nextSaveTime > DateTime.Now || Application.isPlaying) return;
        
        nextSaveTime = nextSaveTime.AddMinutes(3);
        
        Debug.Log("AutoSave Scenes: "+DateTime.Now.ToShortTimeString());
        EditorSceneManager.SaveOpenScenes();
        AssetDatabase.SaveAssets();
    }
}
#endif