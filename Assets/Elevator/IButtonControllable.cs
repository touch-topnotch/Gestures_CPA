using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public interface IButtonControllable
    {
        public bool IsInteractable { get; set; }
        public void Interact();
        public event Action OnCompleted; 
    }
}
