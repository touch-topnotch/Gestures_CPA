using System.Collections;
using System.Collections.Generic;
using Components;
using Scripts.Gestures;
using UnityEngine;

public class Spear : WeaponDesign
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnFrameRecognized(string frameName)
    {
        var frameId = GestureMapper.IndexOfName(frameName);
        Debug.Log("FrameRecognized" + frameId);

    }

    public override void OnGestureDetected()
    {
        Debug.Log("GestureDetected");
    }

    public override void OnHitHolding()
    {
        Debug.Log("HitHolding");

    }

    public override void OnHitCalled()
    {
        Debug.Log("HitCalled");

    }

    public override void OnHitImpact(string affected)
    {
        Debug.Log("HitImpact " + affected);

    }

    public override void OnAbilityReleased()
    {
        Debug.Log("AbilityReleased");

    }
}
