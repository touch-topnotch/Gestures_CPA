using UnityEngine;
using UnityEngine.SceneManagement;

public class test : MonoBehaviour
{
    public Transform db_transform;
    public Transform hand_root;
    
    //когда пришел запрос измени сцену
    private void ChangeScene()
    {
        SceneManager.LoadScene("LobbyGroup");
    }
    
    private void Update()
    {

        var a = db_transform.localRotation;
        var b = hand_root.localRotation;
        var c = 0f;
        // c = distance(a, b);
        c = Quaternion.Dot(a, b);
        print(c);
        
        // Debug.Log(Recognizer.OptimizedDistance(db_transform.rotation, hand_root.rotation) + " rotation distance");
        // Debug.Log(Recognizer.OptimizedDistance(db_transform.position, hand_root.position) + " position distance");
    }
}
