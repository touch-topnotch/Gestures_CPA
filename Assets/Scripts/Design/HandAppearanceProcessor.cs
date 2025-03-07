using System.Linq;
using Scripts.PlayerLogic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Scripts.Design
{
    public class HandAppearanceProcessor: MonoBehaviour
    {
        // This class is used to process the appearance of the hand
        [ShowInInspector] [InlineEditor()] public HandAppearance handAppearanceConfig;
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
        }

        private void RefreshProps(Material mat, AvatarType type)
        {
            if (handAppearanceConfig.appearancesDict.ContainsKey(type) && mat)
            {
//                Debug.Log("Set Props Material: "+  mat.name + " to stage " + stage);
                var props = handAppearanceConfig.appearancesDict[type];
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
            SetDefaultValues(ref handAppearanceConfig, AvatarType.Local);
        }

        public static void SetDefaultValues(ref HandAppearance appearance, AvatarType type)
        {
            if(!appearance.appearancesDict.ContainsKey(type))
                appearance.appearancesDict.Add(type, new HandStageProps());
            var props = appearance.appearancesDict[type];
            props.MainColor = new Color(0.1f, 0, 0.2f, 0.55f);
            props.EdgeColor = new Color(0.53f, 0, 0.8f, 0.8f);
            props.ThumbColor = new Color(0.1f, 0, 0.2f, 0.55f);
            props.FingerColor1 = new Color(0.1f, 0, 0.2f, 0.55f);
            props.FingerColor2 = new Color(0.1f, 0, 0.2f, 0.55f);
            props.FingerColor3 = new Color(0.1f, 0, 0.2f, 0.55f);
            props.FingerColor4 = new Color(0.1f, 0, 0.2f, 0.55f);
            props.EdgeHighlightPower = 1;
            props.FadeCenter = new Vector3(0, 0, 0.15f);
            props.FadeScale = new Vector3(1, 4, 1);
            props.FadeStart = 0.12f;
            props.NoiseScale = 5000;
            props.NoiseStrength = 0.5f;
            appearance.appearancesDict[type] = props;
        }

        [Button("Duplicate first settings to last")]
        private void DuplicateSettings()
        {   
            handAppearanceConfig.appearancesDict[handAppearanceConfig.appearancesDict.Keys.Last()] = handAppearanceConfig.appearancesDict.Values.First();
        }
    #endif
    }
}