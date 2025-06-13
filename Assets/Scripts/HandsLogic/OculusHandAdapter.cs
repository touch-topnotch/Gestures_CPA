using System;
using Scripts.Adapters;
using Sirenix.OdinInspector;
using UnityEngine;
using Util = Scripts.HandsLogic.HandBonesUtility;
namespace Scripts.HandsLogic
{
    [RequireComponent(typeof(OVRSkeleton))]
    public class OculusHandAdapter: HandAdapter
    {
        
        [SerializeField] private OVRSkeleton skeleton;

        [SerializeField] public int[] boneIds = new int[]
        {
            0, -1, 6, 7, 8, 20, 15, 16, 17, 18, 23, -1, 9, 10, 11, 21, 19, -1, 12, 13, 14, 22, 3, 4, 5, 19
        };
        
        #if UNITY_EDITOR
        [Button("Set indexes")]
        private void SetId()
        {
            int index = 0;
            for (int i = 0; i < Util.boneNames.Length; i++)
            {
              
                if (i == 0 || i == 4)
                {
                     if (Enum.TryParse<OVRSkeleton.BoneId>("Hand_WristRoot", out var id))
                     {
                         boneIds[index++] = (int)id;
                     }
                     else
                     {
                         Debug.Log("Wrong Name: Hand_WristRoot!");
                         boneIds[index++] = -1;
                     }
                     continue;
                    
                }

                for (int j = 0; j < Util.deepNames.Length; j++)
                {
                    if (index == 26)
                        return;
                    string utilName = Util.boneNames[i];
                    if (utilName == "Little")
                        utilName = "Pinky";
                    string boneName;
                    if (j == 4) 
                        boneName = "Hand_" + utilName + "Tip";
                    else
                        boneName = "Hand_" +  utilName + j;
                    if(Enum.TryParse<OVRSkeleton.BoneId>(boneName, out var id))
                    {
                        boneIds[index++] = (int)id;
                    }
                    else
                    {
                        Debug.Log("Wrong Name: " + boneName + "!");
                        boneIds[index++] = -1;
                    }
                }
            }
            
        }

        [Button("Check")]
        public void Check()
        {
            for (int i = 0; i < boneIds.Length; i++)
            {
                if(boneIds[i] != -1)
                    print(Enum.GetName(typeof(OVRSkeleton.BoneId), boneIds[i]));
            }
        }
        #endif
        public void Start()
        {
            for (int i = 0; i < skeleton.Bones.Count; i++)
            {
                if (boneIds[i] != -1)
                    points[i] = skeleton.Bones[boneIds[i]].Transform;
                else
                    points[i] = null;
            }
        }
        public void FixedUpdate()
        {
           
        }

        protected override bool shouldAddMissingComponents => false;

        public override Transform[] points { get; protected set; } = new Transform[26];
        public override bool isTracked => skeleton.IsDataHighConfidence;
    }
}