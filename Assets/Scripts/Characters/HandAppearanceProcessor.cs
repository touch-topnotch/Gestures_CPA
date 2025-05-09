using System.Linq;
using Scripts.PlayerLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Design
{
    public class HandAppearanceProcessor
    {
        // This class is used to process the appearance of the hand 
        private readonly HandAppearance handAppearanceConfig;

        public HandAppearanceProcessor(HandAppearance handAppearance)
        {
            handAppearanceConfig = handAppearance;
        }

        public void ChangeMaterialPair(MaterialPair pair, AvatarType type)
        {
            RefreshProps(pair, type);
        }

        private void RefreshProps(MaterialPair pair, AvatarType type)
        {
            RefreshProps(pair.Left, type);
            RefreshProps(pair.Right, type);
        }

        private void RefreshProps(Material mat, AvatarType type)
        {
            if (handAppearanceConfig.handStages.ContainsKey(type) && mat)
            {
//                Debug.Log("Set Props Material: "+  mat.name + " to stage " + stage);
                var props = handAppearanceConfig.handStages[type];
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
    }
}