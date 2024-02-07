using Scripts.PlayerLogic;
using Unity.Netcode;
using UnityEngine;

namespace Scripts.Characters
{
    [RequireComponent(typeof(CharacterPool))]
    public class NetworkCharacterHandler : NetworkBehaviour
    {
        private CharacterPool _characterPool;
        
        private void OnValidate()
        {
            _characterPool = GetComponent<CharacterPool>();
            _characterPool.OnCharacterChanged.AddListener((charName)=>
            {
                //     HandleReactivationServerRpc(charName, OwnerClientId);
            });
        }
        [ServerRpc]
        private void HandleReactivationServerRpc(string charName, ulong playerId)
        {
            _characterPool.CurrentName = charName; // Reactivate the character on the server

            // Call an RPC on all clients to update the character
            ReactivateCharacterClientRpc(charName, playerId);
        }

        [ClientRpc]
        private void ReactivateCharacterClientRpc(string name, ulong playerId)
        {
            // Call the Reactivate method on the character on all clients
            Debug.Log("Trying to ReactivateCharacterClientRpc of" + NetworkManager.ConnectedClients[playerId].PlayerObject.transform.name);
            NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<Player>().CharacterPool.name = name;
        }

    }
}
