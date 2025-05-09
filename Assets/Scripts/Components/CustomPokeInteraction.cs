using System;
using Scripts.PlayerLogic;
using Scripts.Static;
using UnityEngine;

namespace Components
{
    public class CustomPokeInteraction : MonoBehaviour
    {
        public Player player;
        public Transform leftPokePoint;
        public Transform rightPokePoint;

        private void OnValidate()
        {
            if (!player)
                player = FindObjectOfType<Player>();
            if (player)
            {
                leftPokePoint = player.data.hands.leftHand.points[5];
                rightPokePoint = player.data.hands.rightHand.points[5];
            }
        }

        private void Update()
        {
        }
    }
}