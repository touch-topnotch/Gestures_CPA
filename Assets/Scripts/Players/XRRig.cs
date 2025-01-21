using Scripts.Movements;
using Scripts.PlayerLogic;
using UnityEngine;

public class XRRig :Rig
{
    [SerializeField] private XRMovement _movement;
    public override bool isMoved() => _movement.isMoved();

    public override void StartMove() => _movement.StartMove();

    public override void StopMove() => _movement.StopMove();
}