using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Weapons;
using UnityEngine;

public class StabMelee : Melee
{
    [Header("Stab Settings")] [SerializeField]
    private Transform _weaponPoint;

    [SerializeField] private Transform _bladePoint;

    private Vector3 WeaponStabDirection => (_bladePoint.position - _weaponPoint.position).normalized;

    private float GetBladeSpeedAlongStabDirection(Vector3 bladeSpeedVec)
    {
        return Vector3.Dot(WeaponStabDirection, bladeSpeedVec);
    }

    protected override bool HitCondition() => GetBladeSpeedAlongStabDirection(_blade.speedVec) > _bladeMinSpeed;
}