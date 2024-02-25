using System;
using UnityEngine.Events;

namespace Scripts.Weapons
{
    public interface Hittable
    {
        bool onHitImpact(out string tag);
    }
}