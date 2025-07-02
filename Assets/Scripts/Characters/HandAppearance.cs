using System;
using System.Collections.Generic;
using Scripts.Static.Definitions;
using Scripts.Tests;
using Sirenix.OdinInspector;
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

        public static readonly int[] AllColors = new[]
            { MainColor, EdgeColor, ThumbColor, FingerColor1, FingerColor2, FingerColor3, FingerColor4 };
    }
    

    [InlineEditor()]
    [CreateAssetMenu(fileName = "HandAppearance_", menuName = "Character/HandAppearance")]
    public class HandAppearance : SerializedScriptableObject
    {
        public Color mainColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
        public Color accentColor = new Color(0.8f, 0.8f, 0.8f, 0.8f);
        public Color fingerColor = new Color(0.4f, 0.4f, 0.4f, 0.6f);

        [ShowInInspector]
        public Dictionary<AvatarType, HandStageProps> handStages = new Dictionary<AvatarType, HandStageProps>();

        [BoxGroup("Create Automatically")] [SerializeField]
        private AvatarType tempType;

        [Button("Create")]
        [BoxGroup("Create Automatically")]
        private void GenerateOneAutomatically()
        {
            if (!handStages.ContainsKey(tempType))
            {
                handStages.Add(tempType, defaultProps);
            }

            handStages[tempType] = defaultProps;
        }

        public void GenerateAllAutomatically()
        {
            var avatarTypes = Enum.GetValues(typeof(AvatarType));
            foreach (AvatarType type in avatarTypes)
            {
                tempType = type;
                GenerateOneAutomatically();
            }
        }

        public HandStageProps defaultProps => new HandStageProps()
        {
            MainColor = mainColor,
            EdgeColor = tempType == AvatarType.Enemy ? accentColor * new Color(1.3f, 1, 1, 1) : accentColor,
            ThumbColor = fingerColor,
            FingerColor1 = fingerColor,
            FingerColor2 = fingerColor,
            FingerColor3 = fingerColor,
            FingerColor4 = fingerColor,
            EdgeHighlightPower = 1,
            FadeCenter = new Vector3(0, 0, 0.15f),
            FadeScale = new Vector3(1, 4, 1),
            FadeStart = 0.12f,
            NoiseScale = 5000,
            NoiseStrength = 0.5f,
        };
    }
}