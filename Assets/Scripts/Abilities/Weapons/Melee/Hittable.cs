using System;
using Scripts.Static.Definitions;
using UnityEngine.Events;

namespace Scripts.Weapons
{
    public interface Hittable
    {
        bool onHitImpact(out Affected affected);
    }
}