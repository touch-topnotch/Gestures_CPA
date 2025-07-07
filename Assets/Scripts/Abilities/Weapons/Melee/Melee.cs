using System;
using Scripts.Components;
using Scripts.Gestures;
using Scripts.Players;
using Scripts.Systems.Grab;
using Scripts.Weapons;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.Abilities.Weapons.Melee
{
     public class Melee : Weapon, IGrabable
     {
          [Range(0, 1000)] protected float maxCapacityValue;
          protected NetworkVariable<float> Capacity = new NetworkVariable<float>();
         
          public override void Initialize(PlayerData data, DynamicGesture gesture)
          {
               base.Initialize(data, gesture);
               //ActivatedEvent
               //StartHitEvent
          }

          [SerializeField]
          protected GrabSystem _grabSystem;
          
          public GrabSystem GrabSystem
          {
               get => _grabSystem;
               set => _grabSystem = value;
          }

          protected  override void OnInitialized()
          {
               base.OnInitialized();
               SetGrabSystem();
          }

          public void SetGrabSystem()
          {
               if (_grabSystem != null)
               {
                    _grabSystem.OnGrabStart += OnGrabbed;
                    _grabSystem.OnGrabEnd += OnUnGrabbed;
               }
          }
          
          public void OnGrabbed()
          {
               Debug.Log("OnGrabbed()");
               ActivatedEvent.Invoke();
          }

          public void OnUnGrabbed()
          {
               DeactivatedEvent.Invoke();
          }
     }
}