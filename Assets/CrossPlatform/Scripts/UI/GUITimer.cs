using System.Collections;
using JetBrains.Annotations;
using UnityEngine;

namespace CrossPlatform.UI
{
    public static class GUITimer
    {
        private static readonly WaitForSeconds WaitSecond = new WaitForSeconds(1);
        public static IEnumerator SetTimeForAction(int timeInSeconds, System.Action action, UIText uiText)
        {
            uiText.Show();
            for(int i =  timeInSeconds; i > 0; i--)
            {
                uiText.SetText(i.ToString());
                yield return WaitSecond;
            }

            uiText.Hide();
            action();
        }
    }
}