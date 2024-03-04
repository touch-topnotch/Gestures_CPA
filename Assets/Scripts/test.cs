using Scripts.Gestures;
using Scripts.Network;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{
    public Transform db_transform;
    public Transform hand_root;

    [Button("Try get message")]
    public void TestSend()
    { 
        TelegramBotProcessor.StartReceiving();
    }
    
    [Button("Try stop get message")]
    public void TestStop()
    { 
        TelegramBotProcessor.StopReceiving();
    }
    //когда пришел запрос измени сцену
    private void ChangeScene()
    {
        SceneManager.LoadScene("LobbyGroup");
    }
}
