using System.Collections.Generic;
using Scripts.Hands;
using Scripts.PlayerLogic;
using Scripts.Static;
using UnityEngine;
using Zenject;

namespace Scripts.Gestures
{
    public class GesturesLibrary
    {
        public List<GestureFrame> GestureFrames { get;} = new ();
        public List<DynamicGesture> DynamicGestures { get;} = new ();

        private GFramesCompiler _framesCompiler;
        
        public GesturesLibrary()
        {
            _framesCompiler = new GFramesCompiler(this);
            ReadFrames();
            Debug.Log("Library has initialized:\nDynamic gestures count: " + DynamicGestures.Count + "\nGesture frames count: " + GestureFrames.Count);
        }

        public void InitializeAllAssets(PlayerHands hands) //do it after initializing Player (it needs playerHands)
        {
            foreach (var gesture in DynamicGestures )
            {
                gesture.Graphics.Construct(hands);
            }
        }
        public void ReadFrames()
        {
            _framesCompiler.Read();
        }

        public void Record(HandsStruct hands, string name)
        {
            _framesCompiler.Record(hands, name);
        }
        
        public void SetGestureFrame(GestureFrame frame)
        {
            GestureFrames.Add(frame);
            
            if (frame.name != frame.baseName)
            {
                if (DynamicGestures.Count == 0)
                {
                    AddDGesturesToLibrary(frame);
                    return;
                }

                bool f = false;
                foreach (DynamicGesture dynamicGesture in DynamicGestures)
                {
                    if (dynamicGesture.Name == frame.baseName)
                    {
                        l.rl("Add " + frame.name + " to " + dynamicGesture.Name);
                        dynamicGesture.AddFrame(frame);
                        return;
                    }
                }

                if (!f)
                {
                  AddDGesturesToLibrary(frame);
                }
            }
        }

        private void AddDGesturesToLibrary(GestureFrame frame)
        {
            DynamicGestures.Add(GestureFactory.SetDynamicGesture(frame.baseName));
            DynamicGestures[^1].AddFrame(frame);
        }
        public DynamicGesture GetDynamicGesture(string name)
        {
            foreach (var frame in DynamicGestures)
            {
                if(frame.Name == name)
                    return frame;
            }
            throw new System.Exception("No gesture with this name");
        }
        
        public GestureFrame GetGestureFrame(string name)
        {
            foreach (var frame in GestureFrames)
            {
                if(frame.name == name)
                    return frame;
            }

            return null;
        }
    }
}