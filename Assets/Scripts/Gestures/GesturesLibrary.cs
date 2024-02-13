using System.Collections.Generic;
using Scripts.PlayerLogic;
using Scripts.Tests;
using UnityEngine;

namespace Scripts.Gestures
{
    
    public class GesturesLibrary
    {
        private Dictionary<string, DynamicGesture> _dynamicGestures = new();
        public Dictionary<string, DynamicGesture> DynamicGestures => _dynamicGestures; // словарь, потому что поиск за
        private readonly PlayerData _playerData;                                                                        // O(1), а не O(n) как в листe
        public GesturesLibrary(PlayerData data)
        {
            _playerData = data;
            //GestureMapper.ReplaceCharacters();
            _dynamicGestures = GestureMapper.ReadDynamicGestures(data);

            var log = "Library has initialized for player: " + data.id +". Mapped gestures: ";
            foreach (var VARIABLE in DynamicGestures)
            {
                log += VARIABLE.Key + ", ";
            }
            Debug.Log(log);
        }

        public void RecordFrame(HandsStruct hands, string name)
        {
            SetGestureFrame(new GestureFrame(name, hands));
            GestureMapper.UpdateDynamicGesture(DynamicGestures[GestureMapper.PrefixOfName(name)], _playerData.id);
        }
        public bool TryGetDynamicGesture(string gesture, out DynamicGesture frame)
        {
            return DynamicGestures.TryGetValue(gesture, out frame) ||
                   DynamicGestures.TryGetValue(GestureMapper.PrefixOfName(gesture), out frame);
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

        public bool ContainsFrame(string frame) => (DynamicGestures.ContainsKey(GestureMapper.PrefixOfName(frame))) &&
                                                   DynamicGestures[GestureMapper.PrefixOfName(frame)].HasFrame(frame);

    }
}