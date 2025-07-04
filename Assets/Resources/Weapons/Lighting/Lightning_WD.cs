using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components;
using DG.Tweening;
using Scripts.Gestures;
using Scripts.Static.Definitions;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.VFX;

namespace Scripts
{
    public class Lightning_WD : WeaponDesign
    {

        [SerializeField] private GameObject _explosionObject;

        [SerializeField] private VisualEffect arc;
        [SerializeField] private VisualEffect orb;

        [Header("Properties")]
        [SerializeField] private float yCenter;
        [SerializeField] private float yWidth;
        [SerializeField] [Range(0.001f, 0.5f)]
        private float boardingXZ;
        [SerializeField]
        private float orbBetweenHandsSpeed;
        [SerializeField]
        private float rotationSpeed;
        [SerializeField] 
        private float _explosionDuration;

        private int state;
        private Coroutine lightningMove;
        private GameObject _lightningObject => orb.gameObject;
        private List<Transform> left = new();
        private List<Transform> right = new();
        
        private void Start()
        {
            orb.gameObject.SetActive(false);
            arc.gameObject.SetActive(false);
        }
        
        public override void OnFrameRecognized(string frameName)
        {
            Debug.Log(" public override void OnFrameRecognized(string "+ frameName+")");
            state = GestureMapper.IndexOfName(frameName);
        
            switch (state)
            {
                case 0:
                    var l = playerData.hands.leftHand.points;
                    var r = playerData.hands.rightHand.points;
                    for (int i = 0; i < l.Length; i++)
                    {
                        if (l[i].name.Contains("Tip"))
                        {
                            left.Add(l[i]);
                            right.Add(r[i]);
                        }
                    }
        
                    orb.gameObject.SetActive(true);
                    arc.gameObject.SetActive(true);
                    orb.SetFloat("Power", 0);
                    arc.SetFloat("Power", 0);
                    break;
                case 1:
                    audioProcessor.PlaySequencedSound("Frames", state);
                    break;
            }
        }

        private IEnumerator Explosion()
        {
            _explosionObject.transform.position = _lightningObject.transform.position;
            _explosionObject.SetActive(true);
            _lightningObject.SetActive(false);
            yield return new WaitForSeconds(_explosionDuration);
            _explosionObject.SetActive(false);
        }
        public override void OnAbilityDestroyed()
        {
            _lightningObject.SetActive(false);
            StopCoroutine(lightningMove);
            StartCoroutine(Explosion());
        }
        
        
        public override void Update()
        {
            if (state <= 0)
                return;
        
            if (state <= 2)
            {
                
                var leftPalmPos = playerData.hands.leftHand.palmCenter.position;
                var rightPalmPos = playerData.hands.rightHand.palmCenter.position;
                var distance = Vector3.Distance(leftPalmPos, rightPalmPos);
                // i have min and max boardings. I need to get a coefficient from 0 to 1, where 0 is the distance less than minimum or more than maximum, and 1 is the distance between minimum and maximum
                // it should be linear function
                var yCoef = Math.Clamp(((1 - Math.Abs(yCenter - distance) / yCenter) - 0.2f) * 4, 0, 1);
                var xzDistance = Vector2.Distance(new Vector2(leftPalmPos.x, leftPalmPos.z),
                    new Vector2(rightPalmPos.x, rightPalmPos.z));
                var xzCoef = Math.Clamp((boardingXZ - xzDistance) / boardingXZ * 6, 0, 1);
        
                var power = yCoef * xzCoef;
                arc.transform.position = (leftPalmPos + rightPalmPos) / 2;

                To(orb, "Power", 0.4f);
                To(arc, "Power", 1);
            }    
        
            if (state > 2)
            {
                To(arc, "Power", 0);
                To(orb, "Power", 1);
            }
        
            if (state < 4)
            {
                for (int i = 0; i < left.Count; i++)
                {
                    arc.SetVector3(left[i].name, left[i].position);
                    arc.SetVector3(right[i].name, right[i].position);
                }
        
                arc.SetVector3("EnergyOrbPosition", orb.transform.position);
            }
            
        }
        
        private static void To(VisualEffect effect, string name, float value)
        {
            if (effect.GetFloat(name) < value)
            {
                effect.SetFloat(name, effect.GetFloat(name) + Time.deltaTime);
            }
        
            if (effect.GetFloat(name) > value)
            {
                effect.SetFloat(name, effect.GetFloat(name) - Time.deltaTime);
            }
        }
        public override void OnReadyToBeCasted()
        {
            Debug.Log(" public override void OnReadyToBeCasted()");
        }

        public override void OnCastCancelled()
        {
            Debug.Log(" public override void OnCastCancelled()");
        }

        public override void OnGestureCasted()
        {
            Debug.Log(" public override void OnGestureCasted()");
        }

        public override void OnActivated()
        {
            Debug.Log(" public override void OnActivated()");
        }

        public override void OnHitStarted()
        {
            Debug.Log(" public override void OnHitStarted()");
        }

        public override void OnHitStopped()
        {
            Debug.Log(" public override void OnHitStopped()");
        }

        public override void OnDeactivated()
        {
            Debug.Log(" public override void OnDeactivated()");
            orb.gameObject.SetActive(false);
        }

        public override void OnImpact(string aff)
        {
            Debug.Log(" public override void OnImpact(string aff)");
            var affected = new Affected(aff);
            Debug.Log($"I should play {affected.surfaceType} sound");
            StartCoroutine(Explosion());
        }

        protected override bool shouldAddMissingComponents =>
            !(vfxProcessor && audioProcessor && arc && orb && _explosionObject);

        public override void AddMissingComponents()
        {
            base.AddMissingComponents();
            vfxProcessor = GetComponent<VFXProcessor>();
            audioProcessor = GetComponent<AudioProcessor>();
            arc = transform.Find("Electric Arc").GetComponent<VisualEffect>();
            orb = transform.Find("Electric Orb").GetComponent<VisualEffect>();
            
            _explosionObject = transform.Find("EnergyExplosion").gameObject;
        }
        
        
    }
}