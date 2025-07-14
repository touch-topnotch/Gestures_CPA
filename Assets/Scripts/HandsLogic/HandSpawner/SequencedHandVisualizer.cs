using System;
using Scripts.Gestures;
using Scripts.Systems;
using UnityEngine;

namespace Scripts.HandsLogic
{
    public class SequencedHandVisualizer : MonoBehaviour, ISequencedVisualizer<FrameData>
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
            ManipulateAll(e => e.Hide(true));
        }

        public void SwitchManipulation(FrameData target,
            FrameData.HandManipulation<SequencedVisualizer<BonesData>> manipulate) =>
            FrameData.SwitchManipulation(target, manipulate, leftHandVisualizer, rightHandVisualizer);

        public void TwoHandsManipulation(VisualizerManipulation manipulation)
        {
            manipulation(leftHandVisualizer);
            manipulation(rightHandVisualizer);
        }

        public void ChangeMaxLength(int l) => TwoHandsManipulation((e) => { e.ChangeMaxLength(l); });
      
        public void Hide(bool immediately, int index) => TwoHandsManipulation((e) => { e.Hide(immediately, index); });

        public void HideAll() => TwoHandsManipulation((e) => { e.HideAll(); });

        public void Spawn(FrameData target) =>
            SwitchManipulation(target, (visualizer, data) => { visualizer.Spawn(data); });


        public void ShowHands()
        {          
            Debug.Log("  public void ShowHands()");
            leftHandVisualizer.Show();
            rightHandVisualizer.Show();

        }

        public void Override(FrameData target, int index = 0) =>
            SwitchManipulation(target, (v, d) => { v.Override(d, index); });

        public void SpawnAndMove(FrameData target, HandMoveProps props) =>
            SwitchManipulation(target, (v, d) => { v.SpawnAndMove(d, props); });

        public void Move(FrameData target, HandMoveProps props, int index = 0)
            =>
                SwitchManipulation(target, (v, d) => { v.Move(d, props, index); });

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