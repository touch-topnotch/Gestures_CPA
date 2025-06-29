using System;
using Scripts.Static;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Scripts.Events
{
    
 
    public class EventManager : MonoBehaviour
    {
        public UpdateEvent updateEvent = new UpdateEvent();
        public bool isInitialized;
        public event Action onServicesInitilalised;
        private async void CheckServicesInitialization()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions options = new InitializationOptions();
                await UnityServices.InitializeAsync(options);
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                onServicesInitilalised?.Invoke();
            }
            else
            {
                onServicesInitilalised?.Invoke();
            }
        }
        private void Awake()
        {
            Debug.Log("Event system initialized");

            onServicesInitilalised += () => { isInitialized = true; };
            CheckServicesInitialization();
        }

        private void Update()
        {
            updateEvent?.Invoke();
        }
    }
}