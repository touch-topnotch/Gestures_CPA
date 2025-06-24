using IngameDebugConsole;
using Scripts.Systems;
using UnityEngine;

[RequireComponent(typeof(DebugLogManager))]
public class VRDebugConsoleAdapter : MonoBehaviour
{
    [SerializeField] private HeadInteractionType toggleConsole = HeadInteractionType.DoubleNod;
    [SerializeField] private HeadInteraction headInteraction;


    private DebugLogManager _debugLogManager;

    private void OnValidate()
    {
        if (headInteraction == null)
        {
            var v = FindObjectOfType<HeadInteraction>();
            if (v)
            {
                headInteraction = v;
            }
        }
    }

    private void Awake()
    {
        _debugLogManager = GetComponent<DebugLogManager>();
        headInteraction.onHeadInteraction += e =>
        {
            if (e != toggleConsole)
                return;
            if (_debugLogManager.IsLogWindowVisible)
                _debugLogManager.HideLogWindow();
            else
            {
                _debugLogManager.ShowLogWindow();
            }
        };
    }
}