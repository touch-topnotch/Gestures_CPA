
using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Scripts.Events
{
    public class EventInitializer: MonoBehaviour
    {
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
        public static EventInitializer Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;
            onServicesInitilalised += () => { isInitialized = true; };
            CheckServicesInitialization();
        }

        private void Update()
        {
            UpdateEvent.Instance?.Invoke();
        }
    }
}