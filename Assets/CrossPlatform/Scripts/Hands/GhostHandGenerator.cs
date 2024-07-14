using System;
using System.Collections;
using System.Runtime.InteropServices;
using CrossPlatform.Scripts;
using UnityEngine;

namespace CrossPlatform.Gestures
{
    public class GhostHandGenerator: MonoBehaviour
    {
        public GameObject LeftHand;
        public GameObject RightHand;
        public Color LeftHandColor;
        public Color RightHandColor;
        private WaitForEndOfFrame _wait = new WaitForEndOfFrame();
        public void Initialize()
        {
        }

        public void SpawnHand(HandUsedType handType, Transform[] bones, float time = 1f)
        {
            if ((handType != HandUsedType.LEFT) || (handType != HandUsedType.RIGHT)) 
                throw new ArgumentException("HandType must be LEFT or RIGHT");
            var hand = new GameObject();
            if (handType == HandUsedType.LEFT)
            {
                hand = GameObject.Instantiate(LeftHand, bones[0]); 
            }
            if (handType == HandUsedType.RIGHT)
            {
                hand = GameObject.Instantiate(RightHand, bones[0]); 
            }
            
           for(int i = 0; i < bones.Length; i++)
           {
               hand.transform.GetChild(i).position = bones[i + 1].position;
               hand.transform.GetChild(i).rotation = bones[i + 1].rotation;
           }
           
           StartCoroutine(Timer(hand, time));
        }

        private IEnumerator Timer(GameObject hand, float time)
        {
            var startTime = time;
            
            while (time > 0)
            {   
                hand.transform.Find("HandMesh").GetComponent<MeshRenderer>().material.color = Color.Lerp(LeftHandColor, Color.clear, startTime - time);
                time -= Time.deltaTime;
                yield return _wait;
            }
            Destroy(hand);
        }
    }
}