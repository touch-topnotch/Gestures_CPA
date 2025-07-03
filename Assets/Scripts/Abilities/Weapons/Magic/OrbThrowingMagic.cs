using System;
using Scripts.Components;
using Scripts.Static.Definitions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Weapons.Magic
{
    public class OrbThrowingMagic : Magic
    {
        
        [Range(0, 1000)] [SerializeField]
        protected float damage;

        [Range(0.0f, 100f)] [SerializeField]
        protected float lifeTime;


        [Range(0.0f, 100f)] [SerializeField] protected float orbSpeed = 3f;

        [Range(0.0f, 100f)] [SerializeField] protected float orbBetweenHandsSpeed = 10f;
        
        public TriggerBehaviour orb;
        private bool isTriggered;
        private float lifeTimer;

        private void Start()
        {
            orb.DisableComponents();
        }


        protected override void ActivateSpell()
        {
            if (IsServer)
            {
                orb.EnableComponents();
                orb.TriggerEnterEvent.AddListener((affected)=> ImpactEvent.Invoke(affected.toString));
                ImpactEvent.AddListener(OnImpact);
            }

        }

        protected void OnImpact(string affected)
        {
            
            var affectedStruct = new Affected(affected);
            if (IsServer)
            {
                if (affectedStruct.physicLayer == PhysicLayer.Player && affectedStruct.surfaceType == SurfaceType.Body)
                {
                    Debug.Log("take damage - " +  damage * 0.2 * Mana.Value * Mana.Value);
                }
            }
        }

        private void Update()
        {
            if (state == WeaponState.Casting)
            {
                if (IsClient)
                {
                    var leftPalmPos = playerData.hands.leftHand.palmCenter.position;
                    var rightPalmPos = playerData.hands.rightHand.palmCenter.position;
                    orb.transform.position = (leftPalmPos + rightPalmPos) / 2; //Vector3.Lerp(orb.transform.position, (leftPalmPos + rightPalmPos) / 2,
                       // orbBetweenHandsSpeed * Time.deltaTime);
                }

           
            }
            if (state == WeaponState.Activated)
            {
                if (IsServer)
                {
                    lifeTimer += Time.deltaTime;
                    if (lifeTimer >= lifeTime)
                    {
                        lifeTimer = 0;
                        ImpactEvent.Invoke(new Affected(PhysicLayer.NONE, SurfaceType.NONE).toString);
                    }
                }

                if (IsClient)
                {
                    var fingerDir = playerData.hands.rightHand.points[3].transform.forward;
                    orb.transform.position += fingerDir * (orbSpeed * Time.deltaTime);
                }
     
            }
        }

        private void HideOrbAfterDelay()
        {
            orb.gameObject.SetActive(false);
            if(IsServer) 
                AbilityDestroyedEvent.Invoke();
        }
        protected override bool shouldAddMissingComponents => !weaponDesign;
    }
}