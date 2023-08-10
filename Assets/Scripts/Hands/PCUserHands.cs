using Scripts.Events;
using Scripts.Gestures;
using UnityEngine;
using Zenject;

namespace Scripts.Hands
{
    public class PCUserHands:UserHands
    {
        [SerializeField] private SupportHandCreator DebugHandVisualizer;
        [Inject] private GesturesLibrary _library;
        [Inject] private UpdateEvent _onUpdate;
        [SerializeField]
        private string _targetGestureName = "Water_0";
        private Vector3 lastPos;
        private string lastName;

        [Inject]
        private void Construct(GesturesLibrary library, UpdateEvent onUpdate)
        {
            _library = library;
            _onUpdate = onUpdate;
            
            DebugHandVisualizer.CreateNewStack(new HandsStruct(){LeftPoints = new Vector3[26], RightPoints = new Vector3[26]});
            //создает вспомогательные руки. Позиции рук берутся от первого двуручного или специального жеста(для удобства визуализации)
            LeftSkeleton = new HandSkeleton(DebugHandVisualizer.ActiveHands[0].GetBonesTransforms());
            RightSkeleton = new HandSkeleton(DebugHandVisualizer.ActiveHands[1].GetBonesTransforms());
            _onUpdate.AddListener(UpdateLinesPosition);
            
        }

        private void ChangeHandPose()
        {
            var gest = _library.GetGestureFrame(_targetGestureName);
            if(gest != null)
                DebugHandVisualizer.CreateNewStack(_library.GetGestureFrame(_targetGestureName).Hands);
            else
            {
                print("Такого не существует.. или существует.. я не ебу короче");
            }
        }
        public void UpdateLinesPosition()
        {
            if (_targetGestureName != lastName)
            {
                lastName = _targetGestureName;
                ChangeHandPose();
            }
            if (transform.position != lastPos)
            {
                DebugHandVisualizer.RefreshLinesPosition();
            }

            lastPos = transform.position;
            lastName = _targetGestureName;
        }
    }
}