using System;
using UnityEngine;

namespace Scripts.Static.Definitions
{
   public enum CharacterType
   {
      Grief,
      Anger,
      Bravery,
      Delight,
      None
   }
   public enum AvatarType
   {
      Local,
      Enemy,
      None
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
}