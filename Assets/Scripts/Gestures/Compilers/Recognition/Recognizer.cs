using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Design;
using Scripts.Events;
using Scripts.HandsLogic;
using Scripts.Players;
using Scripts.Systems;
using UnityEngine;

namespace Scripts.Gestures
{
    public class Recognizer
    {
        public static readonly FrameRecognized onSharedFrameBetweenDevices = new FrameRecognized();

        private readonly RecognitionPropertiesConfig _config;

        private static readonly Color _colorActive = new Color(1, 1, 1, 0.0f);
        private static readonly Color _colorPassive = new Color(0.6f, 1, 1, 0.8f);
        private static readonly WaitForUpdate v_waitForUpdate = new WaitForUpdate();
        private readonly PlayerHands _hands;
        public Recognizer(RecognitionPropertiesConfig config, PlayerHands hands)
        {
            _config = config;
            _hands = hands;
        }

        
        public IEnumerator RecognizeDynamicGesture(Dictionary<string, DynamicGesture> possibleGestures,
            Action<string> onGestureRecognized, Action<string> onFrameRecognized)
        {
            // Initialize possible gestures
            var v_possibleGestures = new List<DynamicGesture>(possibleGestures.Values);

            // Initialize possible frames. For first time we will take a 1st frame of each Dynamic Gesture 
            var v_possibleFrames = new List<FrameData>();

            for (int i = 0; i < v_possibleGestures.Count; i++)
            {
                v_possibleFrames.Add(v_possibleGestures[i].frames[0].AttachedToPlayer());
            }

            LogPossibleFrames();

            //when some gesture of the list has been recognized, the curGesture becomes to the curFrameId of the list (index of Dynamic Gesture is equal to the index of possible frame)

            int v_curGesture;
            int drawnSuppLast = -1;
            while (!TryRecognizeFrameInAnyPossibles(_config.PlayerProperties, v_possibleFrames, out v_curGesture, true))
            {
                // and draw supportive hands at this time

                if (TryRecognizeFrameInAnyPossibles(_config.SupportiveProperties, v_possibleFrames,
                        out var curSuppRec, false) && drawnSuppLast != curSuppRec)
                {
                     _hands.handVisualiser.ShowHands();
                      _hands.handVisualiser.Move(v_possibleFrames[curSuppRec], 4, null);
                      _hands.handVisualiser.ManipulateLasts((m) => m.ChangeColorPinPong(_colorActive, _colorPassive,
                         new ColorParams(HandShaderProps.EdgeColor, 1, false)));
                    drawnSuppLast = curSuppRec;
                }

                yield return v_waitForUpdate;
            }

            // call the Frame Recognized Event after it
            onFrameRecognized?.Invoke(v_possibleFrames[v_curGesture].name);


            int v_curFrameId = 1;
            v_possibleFrames = v_possibleGestures[v_curGesture].frames;

            while (v_curFrameId < v_possibleGestures[v_curGesture].frames.Count)
            {
                var possibleFrame = v_possibleFrames[v_curFrameId].AttachedToPlayer();
                var wasDrawn = false;
                while (!RecognizeFrame(_config.PlayerProperties, possibleFrame, true))
                {
                    if (!wasDrawn && RecognizeFrame(_config.SupportiveProperties, possibleFrame, _hands, false))
                    {
                        _hands.handVisualiser.Move(possibleFrame, 4, null);
                        _hands.handVisualiser.ManipulateLasts((m) => m.ChangeColorPinPong(_colorActive, _colorPassive, new ColorParams(HandShaderProps.EdgeColor, 1, false)));
                        wasDrawn = true;
                    }

                    yield return v_waitForUpdate;
                }

                //  v_possibleGestures[v_curGesture].FrameRecognized(v_possibleFrames[v_curFrameId].name);
                onFrameRecognized?.Invoke(v_possibleFrames[v_curFrameId].name);
                v_curFrameId++;
            }

            //  v_possibleGestures[v_curGesture].AllFramesDetected();
            onGestureRecognized?.Invoke(v_possibleGestures[v_curGesture].name);
            _hands.handVisualiser.ManipulateAll(e => e.Hide(true));

            void LogPossibleFrames()
            {
                string log = "Try to detect: ";
                foreach (var frame in v_possibleFrames)
                {
                    log += frame.name + ", ";
                }

                Debug.Log(log);
            }
        }

        public bool TryRecognizeFrameInAnyPossibles(in RecognitionProperties props,
            in List<FrameData> possibleFrames, out int frameId, in bool shareFrameBetweenDevices = false)
        {
            for (int i = 0; i < possibleFrames.Count; i++)
            {
                if (RecognizeFrame(props, possibleFrames[i], shareFrameBetweenDevices))
                {
                    frameId = i;
                    return true;
                }
            }

            frameId = -1;
            return false;
        }

        public bool RecognizeFrame(in RecognitionProperties properties, FrameData frameData,
            in bool shareFrameBetweenDevices = false)
        {
            return RecognizeFrame(properties, frameData, _hands, shareFrameBetweenDevices);
        }

        public static bool RecognizeFrame(RecognitionProperties properties, FrameData frameData,
            PlayerHands hands, in bool shareFrameBetweenDevices = false)
        {
            if (RecognizeHand(frameData.LeftBones, hands.leftHand.points, properties)
                && RecognizeHand(frameData.RightBones, hands.rightHand.points, properties))
            {
                if (shareFrameBetweenDevices)
                    onSharedFrameBetweenDevices?.Invoke(frameData.name);
                return true;
            }

            return false;
        }

        public static bool RecognizeHand(in BonesData bonesData, in Transform[] handSkeleton,
            in RecognitionProperties props)
        {
            if (bonesData == null || bonesData.rotations?.Length != handSkeleton.Length)
                return true;

            var dist = OptimizedDistance(bonesData.rootPos, handSkeleton[0].localPosition);
            if (1 / props.positionQuality - dist < props.positionQuality)
            {
                //     l.rl("Canceled, because position: " + dist + " < " + props.positionQuality);
                return false;
            }

            float distance = OptimizedDistance(bonesData.rotations[0], handSkeleton[0].localRotation);

            if (distance < props.rootRotationQuality)
            {
                //    l.rl("Canceled, because root rotation: " + distance + " > " + props.rootRotationQuality);
                return false;
            }


            for (int i = 1; i < bonesData.rotations.Length; i++)
            {
                distance = OptimizedDistance(bonesData.rotations[i], handSkeleton[i].localRotation);
                var quality = props.rotationQuality;
                if (distance < quality) // 0 - bad, 1 - good, 0.9 - ok
                {
                    //    l.rl("Canceled, because rotation: " + distance + " > " + props.rotationQuality);
                    return false;
                }
            }

            return true;
        }

        public static float OptimizedDistance(in Vector3 a, in Vector3 b) =>
            (float)Math.Sqrt((a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z));

        public static float OptimizedDistance(in Vector4 a, in Vector4 b) =>
            (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y) + (a.z - b.z) * (a.z - b.z) +
            (a.w - b.w) * (a.w - b.w);

        public static float OptimizedDistance(in Quaternion a, in Quaternion b) =>
            Math.Abs(Quaternion.Dot(a, b));

        public static void FrameLog(string name)
        {
            Debug.Log("Frame " + name + " recognized");
        }

        public static void GestureLog(string name)
        {
            Debug.Log("Dynamic Gesture " + name + " recognized");
        }
    }
}