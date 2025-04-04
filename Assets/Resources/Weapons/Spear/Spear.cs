using System.Collections;
using System.Collections.Generic;
using Components;
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
        throw new System.NotImplementedException();
    }

    public override void OnGestureDetected()
    {
        throw new System.NotImplementedException();
    }

    public override void OnHitHolding()
    {
        throw new System.NotImplementedException();
    }

    public override void OnHitCalled()
    {
        throw new System.NotImplementedException();
    }

    public override void OnHitImpact(string affected)
    {
        throw new System.NotImplementedException();
    }

    public override void OnAbilityReleased()
    {
        throw new System.NotImplementedException();
    }
}
