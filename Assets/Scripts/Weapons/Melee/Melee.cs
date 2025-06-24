using Scripts.Static.Definitions;
using UnityEngine;

namespace Scripts.Weapons
{
    // public class Melee : Weapon, IGrabable
    // {
    //     [field: SerializeField] public GrabSystem GrabSystem { get; set; }
    //
    //     [Header("Melee components")] [SerializeField]
    //     protected float _bladeMinSpeed;
    //
    //     [SerializeField] protected Blade _blade;
    //
    //     [SerializeField] private Rigidbody _rigidbody;
    //
    //     public int capacity
    //     {
    //         get => _power;
    //         protected set
    //         {
    //             _power = value;
    //             if (_power <= 0)
    //             {
    //                 AbilityReleased();
    //                 _power = 0;
    //             }
    //         }
    //     }
    //
    //     private Vector3 _previousBladePointPosition;
    //
    //     private bool _bladeTriggered;
    //
    //     public void Start()
    //     {
    //     }
    //
    //     public void SetGrabSystem()
    //     {
    //         GrabSystem.OnGrabStart +=
    //             OnGrabbed;
    //         GrabSystem.OnGrabEnd += OnUnGrabbed;
    //     }
    //
    //
    //     protected override bool ImpactCondition(out string affected) => _blade.onHitImpact(out affected);
    //
    //
    //     protected virtual void OnImpact(Affected affected)
    //     {
    //         switch (affected.physicLayer)
    //         {
    //             case PhysicLayer.Player:
    //                 Debug.Log("Melee weapon hit player!");
    //                 capacity -= 10;
    //                 break;
    //             case PhysicLayer.Map:
    //                 Debug.Log("Melee weapon hit solid object");
    //                 capacity -= 5;
    //                 break;
    //         }
    //     }
    //
    //     protected override void OnAbilityReleased()
    //     {
    //         throw new System.NotImplementedException();
    //     }
    //     // }
    // }
}