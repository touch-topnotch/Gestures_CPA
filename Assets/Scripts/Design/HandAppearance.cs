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
    public class HandAppearance: MonoBehaviour
    {
       
        [SerializeField] private CustomDictionary<AvatarType, HandStageProps> appearancesDict = new CustomDictionary<AvatarType, HandStageProps>();
        private string default_stage;
        private string local_stage;
        private string enemy_stage;
        [Space]
        [SerializeField] private AvatarType debugStage;
        [SerializeField] private MaterialPair debugPair;
        
        public void ChangeMaterialPair(MaterialPair pair, AvatarType type)
        {
           RefreshProps(pair, type);
        }
        private void RefreshProps(MaterialPair pair, AvatarType type)
        {
            RefreshProps(pair.Left, type);
            RefreshProps(pair.Right, type);
            // var isL = stage.Contains("_L");
            // if (isL || stage.Contains("_R"))
            // {
            //     RefreshProps(isL ? pair.Left : pair.Right, stage);
            // }
            // else
            // {
            //     RefreshProps(pair.Left, stage);
            //     RefreshProps(pair.Right, stage);
            // }
        }

        private void RefreshProps(Material mat, AvatarType type)
        {
            if (appearancesDict.ContainsKey(type) && mat)
            {
//                Debug.Log("Set Props Material: "+  mat.name + " to stage " + stage);
                var props = appearancesDict[type];
                mat.SetColor(HandShaderProps.MainColor, props.MainColor);                             
                mat.SetColor(HandShaderProps.EdgeColor, props.EdgeColor);
                mat.SetFloat(HandShaderProps.EdgeHighlightPower, props.EdgeHighlightPower);
                mat.SetColor(HandShaderProps.ThumbColor, props.ThumbColor);
                mat.SetColor(HandShaderProps.FingerColor1, props.FingerColor1);
                mat.SetColor(HandShaderProps.FingerColor2, props.FingerColor2);
                mat.SetColor(HandShaderProps.FingerColor3, props.FingerColor3);
                mat.SetColor(HandShaderProps.FingerColor4, props.FingerColor4);
                mat.SetVector(HandShaderProps.FadeCenter, props.FadeCenter);
                mat.SetVector(HandShaderProps.FadeScale, props.FadeScale);
                mat.SetFloat(HandShaderProps.FadeStart, props.FadeStart);
                mat.SetFloat(HandShaderProps.NoiseScale, props.NoiseScale);
                mat.SetFloat(HandShaderProps.NoiseStrength, props.NoiseStrength);
            }
        }
        
        #if UNITY_EDITOR
        [Button("Check stage on material")]
        private void CheckStage()
        {
            ChangeMaterialPair(debugPair, debugStage);
        }
        [Button("Set default values to debugStage")]
        private void SetDefaultValuesToFirstStage()
        {
            var props = appearancesDict[debugStage];
            props.MainColor = Color.white;
            props.EdgeColor = Color.white;
            props.ThumbColor = Color.white;
            props.FingerColor1 = Color.white;
            props.FingerColor2 = Color.white;
            props.FingerColor3 = Color.white;
            props.FingerColor4 = Color.white;
            props.EdgeHighlightPower = 1;
            props.FadeCenter = new Vector3(0, 0, 0.15f);
            props.FadeScale = new Vector3(1, 4, 1);
            props.FadeStart = 0.12f;
            props.NoiseScale = 5000;
            props.NoiseStrength = 0.5f;
            appearancesDict[debugStage] = props;

        }
        
        [Button("Duplicate first settings to last")]
        private void DuplicateSettings()
        {   
            appearancesDict[appearancesDict.Keys.Last()] = appearancesDict.Values.First();
        }
    #endif
    }
}