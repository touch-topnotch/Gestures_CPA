using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Scripts.Events
{
    public class EventInitializer : MonoBehaviour
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
        // write same method, but with courutine
        private IEnumerator CheckServicesInitializationCoroutine()
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                InitializationOptions options = new InitializationOptions();
                
                yield return UnityServices.InitializeAsync(options);
                yield return AuthenticationService.Instance.SignInAnonymouslyAsync();
                onServicesInitilalised?.Invoke();
            }
            else
            {
                onServicesInitilalised?.Invoke();
            }
        }
        

        public static EventInitializer Instance;

        private void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
            {
                Instance = this;
                DontDestroyOnLoad(this);
            }

            Debug.Log("Event system initialized");

            onServicesInitilalised += () => { isInitialized = true; };
            StartCoroutine(CheckServicesInitializationCoroutine());
        }

        private void Update()
        {
            UpdateEvent.Instance?.Invoke();
        }
    }
}