using System;
using System.ComponentModel;
using Scripts.Abilities;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Static.Definitions;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using CharacterController = Scripts.Characters.CharacterController;

namespace Scripts.Players
{
    public class PlayerData
    {
        public PlayerMode playerMode => owner.playerMode;
        public PlayerProperties properties => owner.playerProperties;
        public Rig rig => owner.rig;
        
        public readonly PlayerHands hands;

        public readonly BodyAnchors anchors;

        public readonly AbilityController abilityController;
        public readonly CharacterController characterController;
        public readonly GesturesLibrary gesturesLibrary;

        public UnityEvent onPlayerInitialized => owner.onPlayerInitialized;
        public UnityEvent<PlayerMode> onPlayerModeChanged => owner.onPlayerModeChanged;
        public UnityEvent<RigType> onPlayerRigChanged => owner.onPlayerRigChanged;

        public UnityEvent<HeadInteractionType> onHeadInteraction => owner.onHeadInteraction;
        
        public static PlayerData local;
        
        private readonly Player owner;

        public PlayerData(Player owner, BodyAnchors anchors, PlayerHands playerHands, AbilityController abilityController, CharacterController characterController, GesturesLibrary gesturesLibrary)
        {
            this.owner = owner;
            this.anchors = anchors;
            this.hands = playerHands;
            this.gesturesLibrary = gesturesLibrary;
            this.abilityController = abilityController;
            this.characterController = characterController;
        }
    }
    [Serializable]
    public struct PlayerProperties
    {
        [EnumToggleButtons]
        public RigType rig;
        [EnumToggleButtons]
        public AvatarType avatar;
        public CharacterType character;
        [HideInInspector] public bool activateRigOnAwake;
        
        public PlayerProperties(RigType rig, AvatarType avatar, CharacterType character, bool activateRigOnAwake = true)
        {
            this.rig = rig;
            this.avatar = avatar;
            this.character = character;
            this.activateRigOnAwake = activateRigOnAwake;
        }

        public PlayerProperties(string fromString)
        {

            var words = fromString.Split(' ');

            if (words.Length < 3)
                throw new ArgumentException();

            this.rig = (RigType)words[0][0];
            this.avatar = (AvatarType)words[1][0];
            this.character = (CharacterType)words[2][0];
            this.activateRigOnAwake = Convert.ToBoolean(words[3]);
        }

        public string toString => $"{(char)rig} {(char)avatar} {(char)character} {Convert.ToInt16(activateRigOnAwake)}";
    }
}