using System;
using UnityEngine;
using UnityEngine.XR.Hands;

namespace Scripts.Hands
{
    public enum HandType
    {
        left,
        right
    }
    public class HandMesh : MonoBehaviour
    {
        public Material material;
        public Transform[] points;
        public HandType handType = HandType.left;
        [SerializeField] private Material _defaultMaterial;
        

        private void Start()
        {
            if(_defaultMaterial != null)
                ResetMaterial();
        }

        public void ResetMaterial()
        {
            material.SetColor("_MainColor", _defaultMaterial.GetColor("_MainColor"));
            material.SetColor("_EdgeColor", _defaultMaterial.GetColor("_EdgeColor"));
            material.SetFloat("_EdgeHighlightPower", _defaultMaterial.GetFloat("_EdgeHighlightPower"));
            material.SetColor("_ThumbColor", _defaultMaterial.GetColor("_ThumbColor"));
            material.SetColor("_FingerColor_1", _defaultMaterial.GetColor("_FingerColor_1"));
            material.SetColor("_FingerColor_2", _defaultMaterial.GetColor("_FingerColor_2"));
            material.SetColor("_FingerColor_3", _defaultMaterial.GetColor("_FingerColor_3"));
            material.SetColor("_FingerColor_4", _defaultMaterial.GetColor("_FingerColor_4"));
            material.SetVector("_FadeCenter", _defaultMaterial.GetVector("_FadeCenter"));
            material.SetVector("_FadeScale", _defaultMaterial.GetVector("_FadeScale"));
            material.SetFloat("_FadeStart", _defaultMaterial.GetFloat("_FadeStart"));
            material.SetFloat("_NoiseScale", _defaultMaterial.GetFloat("_NoiseScale"));
            material.SetFloat("_NoiseStrength", _defaultMaterial.GetFloat("_NoiseStrength"));

        }
        public void SetFingersColor(in Color color)
        {
            material.SetColor("_ThumbColor", color);
            material.SetColor("_FingerColor_1", color);
            material.SetColor("_FingerColor_2", color);
            material.SetColor("_FingerColor_3", color);
            material.SetColor("_FingerColor_4", color);
        }
        
        public void SetRotations(in Vector3[] rotations)
        {
            if (rotations == null)
            {
                return;
            }
            for (int i = 0; i < points.Length; i++)
            {
                points[i].rotation = Quaternion.Euler(rotations[i]);
            }
        }
        public void SetRotations(in Quaternion[] rotations)
        {
            if (rotations == null)
            {
                return;
            }
            for (int i = 0; i < points.Length; i++)
            {
                points[i].rotation = rotations[i];
            }
        }

        public void SetRootPosition(in Vector3 position)
        {
            points[0].position = position;
        }

        public void SetBonesData(in BonesData data)
        {
            SetRootPosition(data.rootPos);
            SetRotations(data.rotations);
        }   

        public Vector3[] GetRotations()
        {
            Vector3[] rots = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                rots[i] = Quaternion.ToEulerAngles(points[i].rotation);
            }

            return rots;
        }
    }
}