using Network;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class UserInitializator : MonoBehaviour
{
   private async void Start()
    {
      
        await UnityServices.InitializeAsync();
        
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Signed in {AuthenticationService.Instance.PlayerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }
}
