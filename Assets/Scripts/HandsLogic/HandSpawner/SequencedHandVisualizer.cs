using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Scripts.HandsLogic
{
    public class SequencedHandVisualizer : MonoBehaviour, ISequencedVisualizer<HandsStruct>
    {
        public delegate void HandMeshManipulation(HandMesh mesh);

        public delegate void VisualizerManipulation(SequencedVisualizer<BonesData> visualizer);

        [SerializeField] private int length;

        [SerializeField] private GameObject leftHandPrefab;
        [SerializeField] private GameObject rightHandPrefab;

        public SequencedVisualizer<BonesData> leftHandVisualizer;
        public SequencedVisualizer<BonesData> rightHandVisualizer;
        private Material _defaultMaterial;

        [SerializeField] private Transform parent;

        public void Awake()
        {
            leftHandVisualizer = new SequencedVisualizer<BonesData>(
                length, (int id) =>
                {
                    var o = Instantiate(leftHandPrefab, parent);
                    o.transform.name = "Sequenced_Hand_L (" + id + ")";
                    return o.GetComponent<HandMesh>();
                });
            rightHandVisualizer = new SequencedVisualizer<BonesData>(
                length, (id) =>
                {
                    var o = Instantiate(rightHandPrefab, parent);
                    o.transform.name = "Sequenced_Hand_R (" + id + ")";
                    return o.GetComponent<HandMesh>();
                });
            ManipulateAll(e => e.Hide());
        }

        public void SwitchManipulation(HandsStruct target,
            HandsStruct.HandManipulation<SequencedVisualizer<BonesData>> manipulate) =>
            HandsStruct.SwitchManipulation(target, manipulate, leftHandVisualizer, rightHandVisualizer);

        public void TwoHandsManipulation(VisualizerManipulation manipulation)
        {
            manipulation(leftHandVisualizer);
            manipulation(rightHandVisualizer);
        }

        public void ChangeMaxLength(int l) => TwoHandsManipulation((e) => { e.ChangeMaxLength(l); });

        public void Hide(int index) => TwoHandsManipulation((e) => { e.Hide(index); });

        public void HideAll() => TwoHandsManipulation((e) => { e.HideAll(); });

        public void Spawn(HandsStruct target) =>
            SwitchManipulation(target, (visualizer, data) => { visualizer.Spawn(data); });

        public void ShowHands()
        {
            leftHandVisualizer.Show();
            rightHandVisualizer.Show();
        }

        public void Override(HandsStruct target, int index = 0) =>
            SwitchManipulation(target, (v, d) => { v.Override(d, index); });

        public void SpawnAndMove(HandsStruct target, float speed, Action onPlaced) =>
            SwitchManipulation(target, (v, d) => { v.SpawnAndMove(d, speed, onPlaced); });

        public void Move(HandsStruct target, float speed, Action onPlaced, int index = 0)
            =>
                SwitchManipulation(target, (v, d) => { v.Move(d, speed, onPlaced, index); });

        public void ManipulateLasts(HandMeshManipulation manipulation) => TwoHandsManipulation((e) =>
        {
            manipulation((HandMesh)e.GetLast());
        });


        public void ManipulateAll(HandMeshManipulation manipulation) => TwoHandsManipulation((e) =>
        {
            foreach (var VARIABLE in e.GetAll())
            {
                manipulation((HandMesh)VARIABLE);
            }
        });
    }
}