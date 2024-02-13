using System;
using Scripts.Events;
using Scripts.HandsLogic;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class BodyAnchors: MonoBehaviour
    {
        public Transform Body;
        public Transform Head;

        private void OnValidate()
        {
            if (Body == null)
                Body = transform.Find("Body");
            if (Head == null)
                Head = transform.Find("Head");
        }

        public static void EquateAnchors(in BodyAnchors master, BodyAnchors target)
        {
            if (!master || !target)
            {
                return;
            }
            
     
            target.Head.position = master.Head.position;
            target.Head.rotation = master.Head.rotation;
            
            target.Body.rotation = master.Body.rotation;
            target.Body.position = master.Body.position;
            
        }
        
    }
    
}