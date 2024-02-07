using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute.Editor
{
      public class InspectorButtonsDrawer
    {
        public readonly List<IGrouping<string, InspectorButton>> ButtonGroups;

        public InspectorButtonsDrawer(object target)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            var methods = target.GetType().GetMethods(flags);
            var buttons = new List<InspectorButton>();
            var rowNumber = 0;

            foreach (MethodInfo method in methods)
            {
                var buttonAttribute = method.GetCustomAttribute<InspectorButtonAttribute>();

                if (buttonAttribute == null)
                    continue;

                buttons.Add(new InspectorButton(method, buttonAttribute));
            }

            ButtonGroups = buttons.GroupBy(button =>
            {
                var attribute = button.ButtonAttribute;
                var hasRow = attribute.HasRow;
                return hasRow ? attribute.Row : $"__{rowNumber++}";
            }).ToList();
        }

        public void DrawButtons(IEnumerable<object> targets)
        {
            foreach (var buttonGroup in ButtonGroups)
            {
                if(buttonGroup.Count() > 0)
                {
                    var space = buttonGroup.First().ButtonAttribute.Space;
                    if(space != 0) EditorGUILayout.Space(space);
                }
                using (new EditorGUILayout.HorizontalScope())
                {
                    foreach (var button in buttonGroup)
                    {
                        button.Draw(targets);
                    }
                }
            }
        }
    }
    
    public class InspectorButton
    {
        public readonly string DisplayName;
        public readonly MethodInfo Method;
        public readonly InspectorButtonAttribute ButtonAttribute;

        public InspectorButton(MethodInfo method, InspectorButtonAttribute buttonAttribute)
        {
            ButtonAttribute = buttonAttribute;
            DisplayName = string.IsNullOrEmpty(buttonAttribute.Name)
                ? ObjectNames.NicifyVariableName(method.Name)
                : buttonAttribute.Name;

            Method = method;
        }

        internal void Draw(IEnumerable<object> targets)
        {
            if (!GUILayout.Button(DisplayName)) return;

            foreach (object target in targets)
            {
                Method.Invoke(target, null);
            }
        }
    }

    [CustomEditor(typeof(UnityEngine.Object), true), CanEditMultipleObjects]
    internal class ObjectEditor : UnityEditor.Editor
    {
        private InspectorButtonsDrawer _buttonsDrawer;
     //   private ReadOnlyListDrawer _readOnlyListDrawer;
        private void OnEnable()
        {
            _buttonsDrawer = new InspectorButtonsDrawer(target);
        //    _readOnlyListDrawer = new ReadOnlyListDrawer(target);
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            _buttonsDrawer.DrawButtons(targets);
        //    _readOnlyListDrawer.DrawButtons(targets);
        }
    }
}