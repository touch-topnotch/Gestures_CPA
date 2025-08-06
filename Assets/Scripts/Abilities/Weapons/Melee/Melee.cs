using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Components;
using Scripts.Gestures;
using Scripts.Network;
using Scripts.Players;
using Scripts.Static.Definitions;
using Scripts.Systems.Grab;
using Scripts.Weapons;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

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
          [SerializeField] protected float _bladeMinSpeed;
          [SerializeField] protected Blade _blade;
          [SerializeField] protected float _hitCooldown;
          private bool _isHitting;

          [SerializeField]
          protected GrabSystem _grabSystem;

          [FormerlySerializedAs("hammerObject")] [SerializeField]
          protected ClientTransform weaponObject;
          public GrabSystem GrabSystem
          {
               get => _grabSystem;
               set => _grabSystem = value;
          }

          protected  override void OnInitialized()
          {
               base.OnInitialized();
               SetGrabSystem();
               ActivatedEvent?.AddListener(()=>
               {
                    //StartCoroutine(dieDelay());
               });
               ActivatedEvent?.AddListener(ActivateWeapon);
          }

          private void ActivateWeapon()
          {
               if (IsServer)
               {
                    _blade.EnableComponents();
                    _blade.TriggerEnterEvent.AddListener((affected) =>
                    {
                         if (state != WeaponState.Activated)
                              return;
                         if (_isHitting && _blade.onHitImpact())
                         {
                              ImpactEvent.Invoke(affected.toString);
                         }
                    });
                    //ImpactEvent.AddListener(OnImpact);
               }
          }

          protected void Update()
          {
               if (IsOwner && weaponObject.enabled)
               {
                    weaponObject.transform.position = _grabSystem._grabObjectAnchor.position;
                    weaponObject.transform.rotation = _grabSystem._grabObjectAnchor.rotation;

                    
                    if (state == WeaponState.Activated && !_isHitting && HitCallCondition())
                    {
                         StartHitEvent.Invoke();
                         StartCoroutine(HitDelay());
                    }
               }
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

          public IEnumerator dieDelay()
          {
               yield return new WaitForSeconds(10f);
               AbilityReleasedEvent?.Invoke();
          }
          
          private IEnumerator HitDelay()
          {
               _isHitting = true;
               yield return new WaitForSeconds(_hitCooldown);
               _isHitting = false;
               StopHitEvent?.Invoke();
          }

          public void OnUnGrabbed()
          {
               DeactivatedEvent.Invoke();
          }
          protected virtual bool HitCallCondition() => _blade.speed > _bladeMinSpeed;
     }
}