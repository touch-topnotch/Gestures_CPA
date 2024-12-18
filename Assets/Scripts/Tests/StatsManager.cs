using System;
using Scripts.Events;
using Scripts.PlayerLogic;
using Scripts.Static;
using TMPro;
using UnityEngine;
using Zenject;

namespace Scripts.Tests
{
    public class StatsManager: CustomBehaviour
    {
        [SerializeField] private GameObject statsPanel;
        [SerializeField] private TMP_Text speedText;
        [Inject] private Rig rig;
        private bool isActive = true;
        private Vector3 lastPostion;
        private void Start()
        {
            onUpdate.AddListener(WaitCommand);
            ToggleStats();
        }
        
        private void ToggleStats()
        {
            isActive = !isActive;
            statsPanel.SetActive(isActive);
            if (isActive)
                onUpdate.AddListener(UpdateProps);
            else
                onUpdate.RemoveListener(UpdateProps);
            
        }

        private void UpdateProps()
        {
            speedText.text = "Speed: " + (Vector3.Distance(rig.GetBody.position, lastPostion) / Time.deltaTime) +
                             " units/sec";
            lastPostion = rig.GetBody.position;
        }

        private void WaitCommand()
        {
            if (KeyCombinations.ControlCommand(KeyCode.S))
            {
                ToggleStats();
            }
        }
    }
}