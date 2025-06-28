using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Characters;
using Scripts.Components;
using Scripts.PlayerLogic;
using Unity.Netcode;
using UnityEngine;

namespace Scripts
{
    public class SpearAura : NetworkSmartComponent
    {
        [SerializeField] protected float _buffDelay;
        [SerializeField] protected float _buffAmount;
        [SerializeField] protected float _buffRadius;
        
        private Coroutine _buffCoroutine;
        private WaitForSeconds _buffWFS;

        private void Awake()
        {
            _buffWFS = new WaitForSeconds(_buffDelay);
        }

        private void OnEnable()
        {
            //if (!IsServer) return;
            _buffCoroutine = StartCoroutine(Buff());
        }

        private IEnumerator Buff()
        {
            while (true)
            {
                yield return _buffWFS;
                /*var hits = Physics.OverlapSphere(transform.position, _buffRadius);
                foreach (var hit in hits)
                {
                    if (hit.transform.TryGetComponent(out Player player))
                    {
                        foreach (var weapon in player.character.weapons.Values)
                        {
                            weapon.Power *= 1 + _buffAmount;
                        }
                    }
                }*/
            }
        }
        
        private void OnDisable()
        {
            //if (!IsServer) return;
            StopCoroutine(_buffCoroutine);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _buffRadius);
        }

        protected override bool shouldAddMissingComponents { get; }
    }
}
