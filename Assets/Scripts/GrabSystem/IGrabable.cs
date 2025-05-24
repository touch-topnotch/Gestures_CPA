using System.Collections;
using System.Collections.Generic;
using Scripts.PlayerLogic;
using UnityEngine;

public interface IGrabable
{
    GrabSystem GrabSystem { get; set; }

    public void SetGrabSystem();

    public void OnGrabbed();

    public void OnUnGrabbed();
}