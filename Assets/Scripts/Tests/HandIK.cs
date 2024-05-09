using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace Scripts.Tests
{
    public class HandIK : MonoBehaviour
    {
        public Transform JointRoot;
        public Transform RigRoot;

        public void Start()
        {
            GoByChildren(RigRoot);
        }

        public void GoByChildren(in Transform parent)
        {
            if (parent.GetComponent<TwoBoneIKConstraint>())
            {
                //
                parent.GetComponent<TwoBoneIKConstraint>().data.target =
                    JointRoot.GetChildByName("Joint" + parent.name.Substring(1));
            }

            if (parent.childCount == 0)
                return;
            for (int i = 0; i < parent.childCount; i++)
            {
                GoByChildren(parent.GetChild(i));
            }
        }
    }

    public static class TransformExtensions
    {
        public static Transform GetChildByName(this Transform parent, string childName)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name == childName)
                {
                    return child;
                }
            }

            return null;
        }
    }
}