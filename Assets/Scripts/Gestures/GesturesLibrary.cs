using System.Collections.Generic;
using Scripts.Tests;
using UnityEngine;

namespace Scripts.Gestures
{
    public class GesturesLibrary
    {

        private Dictionary<string, DynamicGesture> _dynamicGestures = new();

        public Dictionary<string, DynamicGesture> DynamicGestures => _dynamicGestures; // словарь, потому что поиск за
                                                                                       // O(1), а не O(n) как в листе
        private static GesturesLibrary _instance;
        public static GesturesLibrary Instance => _instance ??= new GesturesLibrary();
        private GesturesLibrary()
        {
            //GestureMapper.ReplaceCharacters();
            ReadGestures();
            Debug.Log("Library has initialized.");
            var log = "Mapped gestures: ";
            foreach (var VARIABLE in DynamicGestures)
            {
                log += VARIABLE.Key + ", ";
            }
            Debug.Log(log);
        }

        private void ReadGestures()
        {
            _dynamicGestures = GestureMapper.ReadDynamicGestures();
        }

        public void RecordFrame(HandsStruct hands, string name)
        {
            SetGestureFrame(new GestureFrame(name, hands));
            GestureMapper.UpdateDynamicGesture(DynamicGestures[GestureMapper.PrefixOfName(name)]);
        }
        public bool TryGetDynamicGesture(string gesture, out DynamicGesture frame)
        {
            return DynamicGestures.TryGetValue(gesture, out frame);
        }
        public bool TryGetGestureFrame(string name, out GestureFrame frame)
        {
            if (DynamicGestures.ContainsKey(GestureMapper.PrefixOfName(name)))
            {
                frame = DynamicGestures[GestureMapper.PrefixOfName(name)].frames[GestureMapper.IndexOfName(name)];
                return true;
            }
            frame = null;
            return false;
        }

        private void SetGestureFrame(GestureFrame frame)
        {
            if (DynamicGestures.ContainsKey(frame.baseName))
            {
                if (DynamicGestures[frame.baseName].frames.Count <= GestureMapper.IndexOfName(frame.name))
                {
                    for(int i = DynamicGestures[frame.baseName].frames.Count; i <= GestureMapper.IndexOfName(frame.name); i++)
                    {
                        DynamicGestures[frame.baseName].frames.Add(null);
                    }
                }
                DynamicGestures[frame.baseName].frames[GestureMapper.IndexOfName(frame.name)] = frame;
            }
            else
            {
                throw new System.Exception("No dynamic gesture with this name: " + frame.baseName);
            }
        }
       


    }
}