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

        
        
        public GesturesLibrary()
        {
            //GestureMapper.ReplaceCharacters();
            ReadGestures();
            Debug.Log("Library has initialized:\nDynamic gestures count: " + DynamicGestures.Count);
        }

        
      
        public void ReadGestures()
        {
            _dynamicGestures = GestureMapper.ReadDynamicGestures();
        }

        public void RecordFrame(HandsStruct hands, string name)
        {
            SetGestureFrame(new GestureFrame(name, hands));
            GestureMapper.UpdateDynamicGesture(DynamicGestures[GestureMapper.PrefixOfName(name)]);
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