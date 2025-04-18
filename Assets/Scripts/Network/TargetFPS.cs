using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFPS : MonoBehaviour
{
    public int Target = 60;
    void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = Target;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
