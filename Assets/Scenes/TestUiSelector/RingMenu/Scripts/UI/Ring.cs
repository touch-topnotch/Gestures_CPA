using System;
using System.Collections.Generic;

public class Ring
{
    private readonly Action<RingElement> _onClick;
    public string Name { get; set; }
    public List<RingElement> Elements { get; set; }

    public Ring(string name, List<RingElement> elements, Action<RingElement> onClick)
    {
        _onClick = onClick;
        Name = name;
        Elements = elements;
    }

    public static Ring CreateRing<T>
    (string ringName, Dictionary<string, T> data,
        Func<KeyValuePair<string, T>, string> getIdFromNodeAction, Action<RingElement> onClick)
    {
        var intNameRingElements = new List<RingElement>();
        foreach (var i in data)
        {
            intNameRingElements.Add(new RingElement(i.Key, getIdFromNodeAction(i)));
        }

        return new Ring(ringName, intNameRingElements, onClick);
    }
}

public class RingElement
{
    public string Name { get; set; }
    public string Key { get; set; }
    public Ring NextRing { get; set; }

    public RingElement(string name, string key)
    {
        Name = name;
        Key = key;
    }
}