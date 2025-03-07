using System;
using System.Collections.Generic;
using System.Linq;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Tests;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.Design
{
    [Serializable]
    public struct MaterialPair
    {
        public Material Left;
        public Material Right;
    }
    public static class HandShaderProps
    { 
        public static readonly int MainColor = Shader.PropertyToID("_MainColor");
        public static readonly int EdgeColor = Shader.PropertyToID("_EdgeColor");
        public static readonly int EdgeHighlightPower = Shader.PropertyToID("_EdgeHighlightPower");
        public static readonly int ThumbColor = Shader.PropertyToID("_ThumbColor");
        public static readonly int FingerColor1 = Shader.PropertyToID("_FingerColor_1");
        public static readonly int FingerColor2 = Shader.PropertyToID("_FingerColor_2");
        public static readonly int FingerColor3 = Shader.PropertyToID("_FingerColor_3");
        public static readonly int FingerColor4 = Shader.PropertyToID("_FingerColor_4");
        public static readonly int FadeCenter = Shader.PropertyToID("_FadeCenter");
        public static readonly int FadeScale = Shader.PropertyToID("_FadeScale");
        public static readonly int FadeStart = Shader.PropertyToID("_FadeStart");
        public static readonly int NoiseScale = Shader.PropertyToID("_NoiseScale");
        public static readonly int NoiseStrength = Shader.PropertyToID("_NoiseStrength");
        public static readonly int[] FingerNames = new[] { FingerColor1, FingerColor2, FingerColor3, FingerColor4 };
        public static readonly int[] AllColors = new[] { MainColor, EdgeColor, ThumbColor, FingerColor1, FingerColor2, FingerColor3, FingerColor4 };
    }

   
    [Serializable]
    public struct HandStageProps
    {
        public Color MainColor;
        public Color EdgeColor;
        public float EdgeHighlightPower;
        public Color ThumbColor;
        public Color FingerColor1;
        public Color FingerColor2;
        public Color FingerColor3;
        public Color FingerColor4;
        public Vector3 FadeCenter;
        public Vector3 FadeScale;
        public float FadeStart;
        public float NoiseScale;
        public float NoiseStrength;
    }
   

    [CreateAssetMenu(fileName = "Hand_Appearance_", menuName = "Character/HandAppearance")]
    public class HandAppearance : ScriptableObject
    {
        [ShowInInspector]
        public CustomDictionary<AvatarType, HandStageProps> appearancesDict { get; private set; } =
            new CustomDictionary<AvatarType, HandStageProps>();
    }
}