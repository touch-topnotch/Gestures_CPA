using Scripts.Events;
using Scripts.PlayerLogic;
using TMPro;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine;
using Zenject;

namespace Scripts.Movements
{
    public class XRMovement : MonoBehaviour

    {
        protected BodyAnchors _rigAnchors;

        [SerializeField] protected CharacterController parentMoveController;
        [SerializeField] protected float gravity;

        [Range(0, 3f)] [SerializeField] protected float xzBoard;
        [Range(0, 3f)] [SerializeField] protected float yBoard;
        [Range(0, 10f)] [SerializeField] protected float jumpSpeed;
        [Range(0, 10f)] [SerializeField] protected float moveSpeed;

        [SerializeField] protected Transform pivot;


    public float XZBoard
    {
        get => xzBoard;
        set => xzBoard = math.clamp(value, 0, 5);
    }

    public float YBoard
    {
        get => yBoard;
        set => yBoard = math.clamp(value, 0, 5);
    }

    private Vector3 _velocity;

    private bool _isMoved;
    public bool isMoved() => _isMoved;

    public void StartMove()
    {
        _isMoved = true;
        Debug.Log("Movement started");
    }

    public void StopMove()
    {
        _isMoved = false;
        Debug.Log("Movement stopped");
    }

    protected void Update()
    {
        if (!_isMoved)
            return;

        _velocity = (HeadManipulations.HeadVelocity(pivot.position, _rigAnchors.Head.position, XZBoard, YBoard,
            moveSpeed,
            jumpSpeed) + Vector3.down * gravity) / 10;
        parentMoveController.Move(_velocity);
    }

    }
}