using System;
using Scripts.Abilities;
using Scripts.Characters;
using Scripts.Gestures;
using Scripts.HandsLogic;
using Scripts.PlayerLogic;
using Scripts.Static.Definitions;
using Scripts.Systems;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace Scripts.Players
{
    public class PlayerData
    {
        public PlayerMode playerMode => owner.playerMode;
        public PlayerProperties properties => owner.playerProperties;
        
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
        public PlayerProperties(RigType rig, AvatarType avatar, CharacterType character)
        {
            this.rig = rig;
            this.avatar = avatar;
            this.character = character;
        }

        public PlayerProperties(string fromString)
        {
            
            var words = fromString.Split(' ');
            
            if (words.Length < 3)
                throw new ArgumentException();
            
            this.rig = (RigType)words[0][0];
            this.avatar = (AvatarType)words[1][0];
            this.character = (CharacterType)words[2][0];
        }

        public string toString => $"{(char)rig} {(char)avatar} {(char)character}";
    }
}