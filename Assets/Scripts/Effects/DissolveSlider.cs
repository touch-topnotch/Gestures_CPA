using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveSlider : MonoBehaviour
{
    [Range(0, 1)] public float dissolveValue = 0.7f;

    private bool _activated = true;
    private Material _material;

    void Start()
    {
        _material = GetComponent<Renderer>().material;
        _material.SetFloat("_DissolveVal", dissolveValue);
    }

    void Update()
    {
        if (dissolveValue <= 0)
        {
            _activated = false;
            return;
        }
        if (!_activated) return;
        
        _material.SetFloat("_DissolveVal", dissolveValue);
    }

    public void UpdateDisolveValue(float val)
    {
        if (val > 1) val = 1;
        if (val < 0) val = 0;
        dissolveValue = val;
    }

    public void Activate()
    {
        _activated = true;
    }
}