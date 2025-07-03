using System;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.Weapons.Magic
{
    [Serializable]
    public abstract class Magic : Weapon
    {
        [Range(0, 1000)] protected float maxManaValue;
        protected NetworkVariable<float> Mana = new NetworkVariable<float>();
        protected override void OnInitialized()
        {
            if (IsServer)
            {
                StartUsingMagicSpell();
            }
        }
        protected virtual void StartUsingMagicSpell()
        {
            Mana.Value = maxManaValue;
            GestureCastedEvent.AddListener(()=>
            {
                ActivatedEvent.Invoke();
            });
            ActivatedEvent.AddListener(ActivateSpell);
        }

        protected abstract void ActivateSpell();
    }
}