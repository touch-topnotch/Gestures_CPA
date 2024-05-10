using System;
using Scrips.Components;
using Scripts.Components;
using Scripts.Events;
using Scripts.HandsLogic;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class BodyAnchors : SmartComponent
    {
        public Transform Root;
        public Transform Body;
        public Transform Head;

        protected override bool shouldAddMissingComponents => !(Root && Body && Head);
        public override void AddMissingComponents()
        {
            if (Root == null)
                Root = transform;
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

            target.Root.position = master.Root.position;

            target.Head.position = master.Head.position;
            target.Head.rotation = master.Head.rotation;

            target.Body.position = master.Body.position;
            target.Body.rotation = master.Body.rotation;
        }
    }
}