using System;
using Unity.Netcode;
using UnityEngine.Events;

namespace Scripts.Components
{
    public interface IInteractable
    {
        public UnityEvent StartInteractionEvent { get; set; }
        public UnityEvent StopInteractionEvent { get; set; }
        public void SubscribeEvents();
    }
    public abstract class NetworkInteractable : NetworkBehaviour, IInteractable
    {
        public UnityEvent StartInteractionEvent { get; set; } = new UnityEvent();
        public UnityEvent StopInteractionEvent { get; set; } = new UnityEvent();
        public abstract void SubscribeEvents();
    }
}