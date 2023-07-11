using CrossPlatform.PlayerLogic;
using CrossPlatform.Gestures;
using UnityEngine;

namespace CrossPlatform.Tests
{
    public class GestureFramesRecorder

    {
        private Player _player;
        private GFramesCompiler compiler = new GFramesCompiler();
        private string current_name = "";
        private Vector3[] left = null;
        private Vector3[] right = null;
        
        public void Initialize(Player player)
        {
            _player = player;
            compiler.Initialize();
        }

        public virtual void NextHand()
        {
            if (current_name != "")
            {
                compiler.Record(left, right, current_name);
                current_name = "";
                left = null;
                right = null;
            }
            else
            {
                Debug.LogWarning("Haven't name of gesture!");
            }
        }

        public virtual void RecordName(string name)
        {
            current_name = name;
        }

        public virtual void RecordLeft()
        {
            left = _player.GetLeftHandPoints();
        }
        public virtual void RecordRight()
        {
            right = _player.GetRightHandPoints();
        }

    }
}