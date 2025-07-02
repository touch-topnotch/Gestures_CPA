using System.Collections.Generic;
using Scripts.PlayerLogic;
using Scripts.Static.Definitions;
using UnityEngine;

namespace Scripts.GameControllers
{
    public class GameProperties: MonoBehaviour
    {
        [SerializeField] public Player player;
        [InspectorName("Use next character abilities")]
        [SerializeField] public CharacterType[] debugCharacterAbilities;

        public Transform[] spawnPoints;
    }
}