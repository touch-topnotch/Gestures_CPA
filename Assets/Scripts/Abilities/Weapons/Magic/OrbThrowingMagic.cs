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


        [Range(0.0f, 10f)] [SerializeField] protected float orbSpeed = 3f;

        [Range(0.0f, 100f)] [SerializeField] protected float orbBetweenHandsSpeed = 10f;
        
        public TriggerBehaviour orb;
        private bool isTriggered;
        private float lifeTimer;

        private void Start()
        {
            GestureCastedEvent.AddListener(() =>
            {
                isTriggered = false;
                lifeTimer = 0;
            });
            orb.DisableComponents();
        }


        protected override void ActivateSpell()
        {
            if (IsServer)
            {
                orb.EnableComponents();
                orb.TriggerEnterEvent.AddListener((affected)=>
                {
                    if (state != WeaponState.Activated)
                        return;
                    if (!isTriggered)
                    {
                        ImpactEvent.Invoke(affected.toString);
                        isTriggered = true;
                    }
                });
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
                    orb.transform.position =
                        (leftPalmPos + rightPalmPos) / 2; //Vector3.Lerp(orb.transform.position, (leftPalmPos + rightPalmPos) / 2,
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
                        AbilityDestroyedEvent.Invoke();
                    }
                }

                if (IsClient)
                {
                    SimpleOrbAim();
                    //DistanceBasedOrbAim();
                    //RayCastOrbAim();
                }
     
            }
        }

        private void SimpleOrbAim()
        {
            var fingerDir = playerData.hands.rightHand.points[3].transform.forward;
            orb.rigidBody.velocity = fingerDir * orbSpeed;
            //   orb.transform.position += fingerDir * (orbSpeed * Time.deltaTime);
        }

        private float _distanceMultiplier = 1.2f;
        private float _rotationSpeed = 360f;
        
        private void DistanceBasedOrbAim()
        {
            var fingerDir = playerData.hands.rightHand.points[3].transform.forward;
            var targetDistance = Vector3.Distance(orb.transform.position,  playerData.hands.rightHand.points[3].transform.position) * _distanceMultiplier;
            var targetPos = playerData.hands.rightHand.points[3].transform.position + fingerDir * targetDistance;
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - orb.transform.position);
            
            orb.rigidBody.MoveRotation(Quaternion.RotateTowards(orb.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime));
            orb.rigidBody.velocity = orb.transform.forward * orbSpeed;
        }

        [SerializeField] private LayerMask _aimTargetLayer;
        
        private void RayCastOrbAim()
        {
            var fingerDir = playerData.hands.rightHand.points[3].transform.forward;
            Ray ray = new Ray(playerData.hands.rightHand.points[3].transform.position, playerData.hands.rightHand.points[3].transform.forward);
            RaycastHit hit;
            
            Vector3 targetPos;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _aimTargetLayer))
            {
                targetPos = hit.point;
            }
            else
            {
                var targetDistance = Vector3.Distance(orb.transform.position,  playerData.hands.rightHand.points[3].transform.position) * _distanceMultiplier;
                targetPos = playerData.hands.rightHand.points[3].transform.position + fingerDir * targetDistance;
            }
            Quaternion targetRotation = Quaternion.LookRotation(targetPos - orb.transform.position);
            
            orb.rigidBody.MoveRotation(Quaternion.RotateTowards(orb.transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime));
            orb.rigidBody.velocity = orb.transform.forward * orbSpeed;
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