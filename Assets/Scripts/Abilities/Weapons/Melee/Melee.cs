using System;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Players;
using Scripts.Static.Definitions;
using UnityEngine;
using UnityEngine.Rendering;
using Sirenix.OdinInspector;
using Unity.Netcode;

namespace Scripts.Weapons
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
          private GrabSystem _grabSystem;
          
          public GrabSystem GrabSystem
          {
               get => _grabSystem;
               set => _grabSystem = value;
          }

          public void SetGrabSystem()
          {
               if (_grabSystem != null)
               {
                    Debug.Log("Вы конченные");
               }
          }
          
          public void OnGrabbed()
          {
               ActivatedEvent.Invoke();
          }

          public void OnUnGrabbed()
          {
               DeactivatedEvent.Invoke();
          }
     }
}