using System;
using System.Runtime.Serialization;
using Scripts.Players;
using Scripts.Static.Definitions;
using Scripts.Weapons;
using UnityEngine;
using Random = System.Random;

namespace Scripts
{
    public class AudioWeaponObserver: WeaponObserver
    {
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private AudioClip[] onSwing;
        [SerializeField]
        private AudioClip[] onImpact_Stone;
        [SerializeField]
        private AudioClip[] onImpact_Wood;
        [SerializeField]
        private AudioClip[] onImpact_Metal;
        [SerializeField]
        private AudioClip[] onImpact_Body;
        [SerializeField]
        private AudioClip[] onImpact_Glass;

        private readonly Random rand = new Random();

        public override void Initialize(PlayerData playerData)
        {
           
        }

        public override void OnActivated()
        {
            
        }

        public override void OnDeactivated()
        {
          
        }

        public override void OnHitStarted()
        {
            
            if(onSwing is { Length: > 0 })
                audioSource.PlayOneShot(onSwing[rand.Next(0, onSwing.Length)]);
        }

        public override void OnHitStopped()
        {
          
        }

        public override void OnImpact(Affected affected)
        {
            switch (affected.surfaceType)
            {
                case SurfaceType.Body:
                    if(onImpact_Body is { Length: > 0 })
                        audioSource.PlayOneShot(onImpact_Body[rand.Next(0, onImpact_Body.Length)]);
                    break;
                case SurfaceType.Wood:
                    if(onImpact_Wood is { Length: > 0 })
                        audioSource.PlayOneShot(onImpact_Wood[rand.Next(0, onImpact_Wood.Length)]);
                    break;
                case SurfaceType.Stone:
                    if(onImpact_Stone is { Length: > 0 })
                        audioSource.PlayOneShot(onImpact_Stone[rand.Next(0, onImpact_Stone.Length)]);
                    break;
                case SurfaceType.Metal:
                    if(onImpact_Metal is { Length: > 0 })
                        audioSource.PlayOneShot(onImpact_Metal[rand.Next(0, onImpact_Metal.Length)]);
                    break;
                case SurfaceType.Glass:
                    if(onImpact_Glass is { Length: > 0 })
                        audioSource.PlayOneShot(onImpact_Glass[rand.Next(0, onImpact_Glass.Length)]);
                    break;
            }
        }

        public override void OnReleased()
        {
        
        }

   
    }
}
