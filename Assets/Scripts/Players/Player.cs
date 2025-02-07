using System;
using Scripts.Characters;
using Scripts.Gestures;
using UnityEngine;

namespace Scripts.PlayerLogic
{
    public enum RigType
    {
        PCRig,
        XRRig,
        NoRig,
    }

 

    public class Player : MonoBehaviour
    {
        [Header("Runtime Settings")] 
        [SerializeField] private RigType _rigType;
        
        [SerializeField] private CharacterPool _characterPool;

        [SerializeField] private bool isLocal;
        
        private GestureCombiner _gestureCombiner = new();

        [Header("Rigs")] [SerializeField] private Rig _pcRig;
        [SerializeField] private Rig _xrRig;
        private Rig _curRig;

        [Header("Anchors")] [SerializeField] private BodyAnchors _anchors;
        public BodyAnchors Anchors => _anchors;
        public Character Character => _characterPool.GetCharacter();
        public CharacterPool CharacterPool => _characterPool;
        public RigType RigType
        {
            get => _rigType;
            set
            {
                _rigType = value;
                CurRig = GetRig();
                ActivateRig();
            }

        }

        private Rig CurRig
        {
            get => _curRig;
            set
            {
                _curRig = value;
                if(_curRig != null) ActivateRig();
            }
        }
        private Rig GetRig()
        {
            switch (_rigType)
            {
                case RigType.XRRig:
                    return _xrRig;
                case RigType.PCRig:
                    return _pcRig;
                case RigType.NoRig:
                    return null;
                default:
                    return _pcRig;
            }
        }

        private void ActivateRig()
        {
            _pcRig.gameObject.SetActive(_rigType == RigType.PCRig);
            _xrRig.gameObject.SetActive(_rigType == RigType.XRRig);
        }
        


        protected void OnValidate()
        {
            if (isAnyNull())
                return;
            CurRig = GetRig();
        }


        private void Start()
        {
            if(isLocal) 
                Initialize();
          
        }

        public void Initialize()
        {
            if (_rigType != RigType.NoRig)
            { 
              _gestureCombiner.Initialize(_curRig, true);
              _anchors.HandsInformation.OnFrameRecognized = _gestureCombiner.OnFrameRecognized;
              _curRig.StartMove();
            }
        }

        private bool isAnyNull()
        {
            if (_pcRig == null || _xrRig == null)
            {
                Debug.Log("Please, add all avatars and rigs to player " + name);
                return true;
            }

            return false;
        }

        private Transform leftRoot => CurRig.Hands.leftHand.points[0];
        private Transform rightRoot => CurRig.Hands.rightHand.points[0];
        protected void UpdateAnchors()
        {
            if (_rigType != RigType.NoRig)
            {
                _anchors.Head.position = CurRig.Anchors.Head.position;
                _anchors.Head.rotation = CurRig.Anchors.Head.rotation;
                _anchors.Body.position = CurRig.Anchors.Body.position;
                _anchors.Body.rotation = CurRig.Anchors.Body.rotation;
                _anchors.HandsInformation.left.rootPosition = leftRoot.position;
                _anchors.HandsInformation.left.rootRotation = leftRoot.rotation;
                _anchors.HandsInformation.right.rootPosition = rightRoot.position;
                _anchors.HandsInformation.right.rootRotation = rightRoot.rotation;
            }
        }
    }
}