using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Scripts.XR
{
    public class CustomXRPokeInteractor: XRPokeInteractor
    {
        [Obsolete("Obsolete")]
        public void Initialize()
        {
            Debug.Log("Registering interactor");
            this.interactionManager.RegisterInteractor(this);
            var interactors = new List<IXRInteractor>();
            interactionManager.GetRegisteredInteractors(interactors);
            Debug.Log("Registered interactors: " + interactors.Count);
        }
    }
}