using System;
using System.Collections.Generic;
using Gesture_Editor_SDK.EditorAttributes.InspectorButtonAttribute;
using Gesture_Editor_SDK.ReadOnly;
using Scripts.PlayerLogic;
using Scripts.Static;
using Sirenix.OdinInspector;
using UnityEngine;
using Avatar = Scripts.PlayerLogic.Avatar;

namespace Scripts.Tests
{
    public class TestPlayerProperties : MonoBehaviour
    {
        
        //[SerializeField] private SerializedDictionary<AvatarType, Avatar> _avatars = new();
        [SerializeField] private CustomDictionary<AvatarType, Avatar> _avatars;
        
        //     [SerializeField] private Player _player;
        //     [SerializeField] private string changeCharacterTo;
        //
        //     [InspectorButton("Change character")]
        //     private void ChangeChar()
        //     {
        //         _player.ChangeCharacter(changeCharacterTo);
        //     }
        //
        //     [SerializeField] private ulong changeIDTo;
        //
        //     [InspectorButton("Change player_material id")]
        //     private void ChangeId()
        //     {
        //         _player.OnPlayerIdChanged(changeIDTo);
        //     }
        //     
        //     [SerializeField] private AvatarType changeAvatarTypeTo;
        //
        //     [InspectorButton("Change Avatar Type")]
        //     private void ChangeAvatar()
        //     {
        //         _player.ChangeAvatarType(changeAvatarTypeTo);
        //     }
        //     
    }
}