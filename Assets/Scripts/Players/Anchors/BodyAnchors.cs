using Scripts.Components;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public class BodyAnchors : SmartComponent
    {

        public Vector3 up;
        public bool hasRoot = true;
        [ShowIf("hasRoot")]
        public Transform Root;

        public bool hasBody = true;
        [ShowIf("hasBody")]
        public Transform Body;

        public bool hasHead = true;
        [ShowIf("hasHead")]
        public Transform Head;

        public bool hasHands = false;
        [ShowIf("hasHands")]
        public Transform Hands;
        protected override bool shouldAddMissingComponents => !(Root && Body && Head);

        public const float k_bodySpeed = 5f;
        public override void AddMissingComponents()
        {
            if (Root == null)
                Root = transform;
            if (Body == null)
                Body = transform.Find("Body");
            if (Head == null)
                Head = transform.Find("Head");
        }

        public static void EquateAnchors(in BodyAnchors master, ref BodyAnchors target)
        {
            if (!master || !target)
            {
                return;
            }
            if(target.hasRoot && master.hasRoot)
                target.Root.position = master.Root.position;

            if (target.hasHead && master.hasHead)
            {
                target.Head.position = master.Head.position;
                target.Head.rotation = master.Head.rotation;
            }

            if (target.hasBody && master.hasBody)
            {
                target.Body.position = master.Body.position;
                target.Body.rotation = master.Body.rotation;
            }

            if (target.hasHands && master.hasHands)
            {
                target.Hands.position = master.Hands.position;
            }
        }

        public void TransformBody()
        {
            var h = Head.position;
            var r = Root.position;
            var A = new Vector3(h.x, -h.z, h.y);
            var B = new Vector3(r.x, -r.z, r.y);
            var xz = Quaternion.LookRotation(A - B).eulerAngles;
            Body.rotation = Quaternion.Euler(xz.x, Head.eulerAngles.y, xz.z);
        }
    }
}