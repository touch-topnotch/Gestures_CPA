using System;
using UnityEngine;

namespace Scripts.Adapter
{
    public class A_OVRCameraRig : OVRCameraRig
    {
        protected GameObject plug;

        protected override void FixedUpdate()
        {
            if (useFixedUpdateForTracking)
                UpdateAnchors(true, false);
        }

        protected override void Update()
        {
            _skipUpdate = false;

            if (!useFixedUpdateForTracking)
                UpdateAnchors(true, false);

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            CheckForAnchorsInParent();
#endif
        }

        protected override Transform ConfigureAnchor(Transform root, string name)
        {
            Transform anchor = (root != null) ? root.Find(name) : null;

            if (anchor == null)
            {
                anchor = transform.Find(name);
            }

            if (anchor == null)
            {
                if (!plug)
                {
                    var p = this.transform.Find("PlugAnchor")?.gameObject;
                    if (!p)
                    {
                        Debug.Log("Plug anchor is not found!");
                        
                    }

                
                    plug = p;
                }

                anchor = plug.transform;
            }
;
            anchor.localScale = Vector3.one;
            anchor.localPosition = Vector3.zero;
            anchor.localRotation = Quaternion.identity;

            return anchor;
        }
    }
}