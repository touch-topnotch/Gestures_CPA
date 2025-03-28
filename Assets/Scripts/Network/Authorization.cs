using System;
using System.Threading.Tasks;
using Sirenix.Serialization;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Scripts.Network
{
    public class Authorization: MonoBehaviour
    {
        // singleton
        public string accessToken;
        public event Action OnSignedIn;
        public static Authorization Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            Debug.Log(UnityServices.State);
            SetupEvents();
            await SignInAnonymouslyAsync();
        }
        public void SetupEvents() {
            AuthenticationService.Instance.SignedIn += () => {
                // Shows how to get a playerID
                Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

                // Shows how to get an access token
                Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");
                accessToken = AuthenticationService.Instance.AccessToken;
                OnSignedIn?.Invoke();
                
            };

            AuthenticationService.Instance.SignInFailed += (err) => {
                Debug.LogError(err);
            };

            AuthenticationService.Instance.SignedOut += () => {
                Debug.Log("Player signed out.");
            };

            AuthenticationService.Instance.Expired += () =>
            {
                Debug.Log("Player session could not be refreshed and expired.");
            };
        }

        private async Task SignInAnonymouslyAsync()
        {
            try
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (AuthenticationException e)
            {
                Debug.LogException(e);
            }
            catch (RequestFailedException e)
            {
                Debug.LogException(e);
            }
        }
    }
}