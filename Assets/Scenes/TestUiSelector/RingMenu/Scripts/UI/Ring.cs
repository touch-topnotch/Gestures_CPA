using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Ring
{
    public string name { get; set; }
    public List<RingSector> sectors { get; set; }
    public Ring(
        string name,
        List<RingProps> elements)
    {
        this.name = name;
        if (elements == null)
            return;
        this.sectors = new List<RingSector>();
        foreach (var element in elements)
        {
            this.sectors.Add(new RingSector(element));
        }
    }
}
public class RingProps
{
    public string name;
    public UnityAction<string> onClick;

    public RingProps(string name, UnityAction<string> onClick)
    {
        this.name = name;
        this.onClick = onClick;
    }

    public static List<RingProps> GetFromDictionary<T>(in Dictionary<string, T> dictionary, in UnityAction<string> onClick)
    {
        if (dictionary == null || dictionary.Count == 0)
            return null;
        
        var list = new List<RingProps>();
        foreach (var VARIABLE in dictionary.Keys)
        {
            list.Add(new RingProps(VARIABLE, onClick));
        }
        
        return list;
    }
}

public class RingSector
{
    public RingProps props;
    public RingSector(RingProps ringProps)
    {
        props = ringProps;
    }
}