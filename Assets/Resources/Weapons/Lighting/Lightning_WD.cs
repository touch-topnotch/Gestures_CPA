using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Components;
using DG.Tweening;
using Scripts.Gestures;
using UnityEngine;
using UnityEngine.VFX;

namespace Scripts
{
    
    public class Lightning_WD : WeaponDesign
    {
        private GameObject _lightningObject => orb.gameObject;
        public float speed;
        public float rotationSpeed;
        
        [SerializeField] private GameObject _explosionObject;
        [SerializeField] private float _explosionDuration;

        [SerializeField] private VisualEffect arc;
        [SerializeField] private VisualEffect orb;
        [SerializeField] private float yCenter;
        [SerializeField] private float yWidth;
        [SerializeField] private float lifeTime;
        [SerializeField] [Range(0.001f, 0.5f)] private float boardingXZ;
        private int state;
        private Coroutine lightningMove;




        private List<Transform> left = new List<Transform>();
        private List<Transform> right = new List<Transform>();
        
        private void Start()
        {
            _lightningObject.SetActive(false);
            arc.gameObject.SetActive(false);
       
        }

        public override void OnFrameRecognized(string frameName)
        {
            state = GestureMapper.IndexOfName(frameName);
            
            switch (state)
            {
                case 0:
                    var l = playerData.hands.leftHand.points;
                    var r = playerData.hands.rightHand.points;
                    for (int i = 0; i < l.Length; i ++)
                    {
                        if(l[i].name.Contains("Tip"))
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
            }
        }
        
        private IEnumerator StartLightningBall()
        {
            var hand = playerData.hands.rightHand;
            var initialIndexDir = hand.points[3].transform.forward;
            var moveDir = initialIndexDir;
            while (true)

            {
       
                var fingerDir = hand.points[3].transform.forward;
                moveDir = Vector3.Slerp(moveDir, fingerDir, rotationSpeed * Time.deltaTime);
                _lightningObject.transform.position += fingerDir * (speed * Time.deltaTime);

                yield return null;
            }
            yield break;
        }
        
        private IEnumerator Explosion()
        {
            _explosionObject.transform.position = _lightningObject.transform.position;
            _explosionObject.SetActive(true);
            _lightningObject.SetActive(false);
            yield return new WaitForSeconds(_explosionDuration);
            _explosionObject.SetActive(false);
        }

        public override void OnGestureDetected()
        {
           
        }

        public override void OnHit()
        {
            
        }

        public override void OnImpact(string affected)
        {
           
        }

        public override void OnAbilityReleased()
        {
            _lightningObject.SetActive(false);
            StopCoroutine(lightningMove);
            StartCoroutine(Explosion());
        }

        public override void OnHitHolds()
        {
            
        }
        public void Update()
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
                var yCoef = Math.Clamp(((1 - Math.Abs(yCenter - distance) / yCenter)-0.2f)*4, 0, 1);
                var xzDistance = Vector2.Distance(new Vector2(leftPalmPos.x, leftPalmPos.z),
                    new Vector2(rightPalmPos.x, rightPalmPos.z));
                var xzCoef = Math.Clamp((boardingXZ - xzDistance) / boardingXZ*6, 0, 1);
                Debug.Log(yCoef + " " +xzCoef);
                var power = yCoef * xzCoef;
            
                arc.SetFloat("Power", power);
                orb.SetFloat("Power", power*0.9f);
                orb.transform.position = Vector3.Lerp(orb.transform.position, (leftPalmPos + rightPalmPos) / 2,
                    speed * Time.deltaTime);
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

            if (state >= 4 && lifeTime > 0)
            {
                lifeTime -= Time.deltaTime;
                var fingerDir =  playerData.hands.rightHand.points[3].transform.forward;
                _lightningObject.transform.position += fingerDir * (speed * Time.deltaTime);
            }

            if (lifeTime <= 0)
            {
                state = -1;
                StartCoroutine(Explosion());
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
    }
}
