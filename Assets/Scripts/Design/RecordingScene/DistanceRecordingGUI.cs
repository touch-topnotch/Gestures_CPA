using System.Collections;
using Scripts.Databases;
using Scripts.Events;
using Scripts.Gestures;
using TMPro;
using UnityEngine;
using Zenject;

namespace Scripts.Design.RecordingScene
{
    public class DistanceRecordingGUI : MonoBehaviour

    {
        public TMP_Text timerText;
        public Trigger leftAnchor;

        public Trigger rightAnchor;

        //private NetworkPlayer _networkPlayer;
        private UpdateEvent _onUpdate;

        //private bool _isStarted = false;
        private bool _leftPrepared = false;
        private bool _rightPrepared = false;

        private UserData testUserData;

        public void Construct()
        {
            // if (!UserAdmin.HasIncluded(player.ownUser.userData.id))
            // {
            //     UserAdmin.Add(player.ownUser.userData);
            // }
            timerText.text = "Положите руки на стол";
            UserAdmin.Add(new UserData()
            {
                id = 0,
                name = "debugger",
                bonesData = new float[26, 2],
            });
            //_networkPlayer = networkPlayer;
            // leftAnchor.OnEnter += (tag)=> {if(tag == "LeftHandTrigger")
            // {
            //     _networkPlayer.CurAvatar.hands.leftHand.SetFingersColor(Color.yellow);
            //     
            //     _leftPrepared = true;
            //     CheckOnEntering();
            // } };
            // leftAnchor.OnExit += (tag)=> {if(tag == "LeftHandTrigger") {_networkPlayer.CurAvatar.hands.leftHand.SetFingersColor(Color.clear);
            //     _leftPrepared = false;
            // } };
            // rightAnchor.OnEnter += (tag)=> {if(tag == "RightHandTrigger") {_networkPlayer.CurAvatar.hands.rightHand.SetFingersColor(Color.yellow);
            //     _rightPrepared = true;
            //     CheckOnEntering();
            // } };
            // rightAnchor.OnExit += (tag)=> {if(tag == "RightHandTrigger") {_networkPlayer.CurAvatar.hands.rightHand.SetFingersColor(Color.clear);
            //     _rightPrepared = false;
            // } };
        }

        private void CheckOnEntering()
        {
            if (_leftPrepared && _rightPrepared)
            {
                //_isStarted = true;
                leftAnchor.OnEnter = null;
                leftAnchor.OnExit = null;
                rightAnchor.OnEnter = null;
                rightAnchor.OnExit = null;
                StartCoroutine(RecordingCoroutine());
            }
        }

        public IEnumerator RecordingCoroutine()
        {
            var wait1 = new WaitForSeconds(1);
            for (int i = 5; i > 0; i--)
            {
                timerText.text = "BIODATA RECORDING AFTER: " + i.ToString();
                yield return wait1;
            }
            // _networkPlayer.CurAvatar.hands.SetSameColor("_EdgeColor",  Color.red);
            // _networkPlayer.CurAvatar.hands.leftHand.SetFingersColor(Color.red, false);
            // _networkPlayer.CurAvatar.hands.rightHand.SetFingersColor(Color.red, false);
            // testUserData.bonesData = BonesDistancesRecorder.RecordDistances(_networkPlayer.CurAvatar.hands.leftHand.points,
            //     _networkPlayer.CurAvatar.hands.rightHand.points);
            // UserAdmin.Override(testUserData);
            // timerText.text = "RECORDING";
            // yield return wait1;
            // yield return wait1;
            // _networkPlayer.CurAvatar.hands.leftHand.SetFingersColor(Color.clear, false);
            // _networkPlayer.CurAvatar.hands.rightHand.SetFingersColor(Color.clear, false);
            // _networkPlayer.CurAvatar.hands.SetSameColor("_EdgeColor",  Color.green);
            // timerText.text = "COMPLETE!";
            // yield return wait1;
            // yield return wait1;
            // yield return wait1;
            // _networkPlayer.CurAvatar.hands.SetSameColor("_EdgeColor",  Color.clear);
        }
    }
}