using System.Collections;
using Scripts.Databases;
using Scripts.Events;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Design
{
    public class DistanceRecordingGUI : MonoBehaviour

    {
        public TMP_Text timerText;
        private Player _player;
        private UpdateEvent _onUpdate;
        private bool _isStarted = false;
        [Inject]
        public void Construct(Player player, UpdateEvent onUpdate)
        {
            if (!UserAdmin.HasIncluded(player.ownUser.userData.id))
            {
                UserAdmin.Add(player.ownUser.userData);
            }
            _player = player;
            _onUpdate = onUpdate;
            _onUpdate.AddListener(WaitToClickSpace);
        }

        public void WaitToClickSpace()
        {
            if (Input.GetKeyUp(KeyCode.Space))
            {
                _isStarted = true;
                StartCoroutine(RecordingCoroutine());
                _onUpdate.RemoveListener(WaitToClickSpace);
            }
        }

        public IEnumerator RecordingCoroutine()
        {
            var wait1 = new WaitForSeconds(1);
            _player.bodyAnchors.Hands.leftHand.material.SetColor("Edge Color", Color.yellow);
            _player.bodyAnchors.Hands.rightHand.material.SetColor("Edge Color", Color.yellow);
            for (int i = 5; i > 0; i--)
            {
                timerText.text = i.ToString();
                yield return wait1;
            }
            _player.bodyAnchors.Hands.leftHand.material.SetColor("Edge Color", Color.red);
            _player.bodyAnchors.Hands.rightHand.material.SetColor("Edge Color", Color.red);
            _player.ownUser.userData = BonesDistancesRecorder.RecordDistances(_player.bodyAnchors.Hands.leftHand.points,
                _player.bodyAnchors.Hands.rightHand.points, _player.ownUser.userData);
            UserAdmin.Override(_player.ownUser.userData);
            timerText.text = "RECORDING";
            yield return wait1;
            yield return wait1;
            _player.bodyAnchors.Hands.leftHand.material.SetColor("Edge Color", Color.green);
            _player.bodyAnchors.Hands.rightHand.material.SetColor("Edge Color", Color.green);
            timerText.text = "COMPLETE!";

        }
         
    }

}