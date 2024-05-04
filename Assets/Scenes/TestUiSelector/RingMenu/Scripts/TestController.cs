using System.Collections.Generic;
using System.Linq;
using Scripts.Gestures;
using Scripts.PlayerLogic;
using Scripts.Systems;
using UnityEngine;

public class TestController : MonoBehaviour
{
    [SerializeField] private RingMenu RingMenuPrefab;
    [SerializeField] private Player _player;

    private Dictionary<string, DynamicGesture> _characterGestures;
    private Dictionary<string, GestureFrame> _supportiveGestures;
    private Dictionary<string, GestureFrame> _systemGestures;


    private void Start()
    {
        _player.gestureCombiner.library.onLibraryInitialized += Library_OnLibraryInitialized;
    }

    private void Library_OnLibraryInitialized()
    {
        Debug.Log("I get this gestures");
        Debug.Log("character: " +
                  Debugger.dictionaryToString(_player.gestureCombiner.library.characterGestures, false, true));
        _characterGestures = _player.gestureCombiner.library.characterGestures;

        Debug.Log("supportive: " +
                  Debugger.dictionaryToString(_player.gestureCombiner.library.supportiveGestures, false, true));
        _supportiveGestures = _player.gestureCombiner.library.supportiveGestures;

        Debug.Log("system: " +
                  Debugger.dictionaryToString(_player.gestureCombiner.library.systemGestures, false, true));
        _systemGestures = _player.gestureCombiner.library.systemGestures;
    }

    [SerializeField] private RingMenu ringMenu;
    private List<Ring> _rings;

    private void PrepareRingData()
    {
        var dynamicGesturesRing = DynamicGesturesRing(_characterGestures);

        var supportiveGesturesRing = GestureFramesRing(_supportiveGestures);
        var systemGesturesRing = GestureFramesRing(_systemGestures);

        _rings = new List<Ring>();
        // _rings.Add(Ring.CreateRing(
        //     "types", new Dictionary<string, Object>(), pair => pair.Value.name,
        //     i => { Debug.Log($"Ring invoked onClick Action; Name: {i.Name} Key:{i.Key}"); }));
        if (dynamicGesturesRing.Elements.Count > 0)
        {
            _rings.Add(dynamicGesturesRing);
        }

        if (supportiveGesturesRing.Elements.Count > 0)
        {
            _rings.Add(supportiveGesturesRing);
        }

        if (systemGesturesRing.Elements.Count > 0)
        {
            _rings.Add(systemGesturesRing);
        }
    }

    private static Ring GestureFramesRing(Dictionary<string, GestureFrame> gestureFrames)
    {
        var supportiveGesturesRing = Ring.CreateRing(
            nameof(gestureFrames),
            gestureFrames,
            pair => pair.Value.name,
            i => { Debug.Log($"Ring invoked onClick Action; Name: {i.Name} Key:{i.Key}"); }
        );

        return supportiveGesturesRing;
    }

    private static Ring DynamicGesturesRing(Dictionary<string, DynamicGesture> DynamicGestures)
    {
        var dynamicGesturesRing = Ring.CreateRing(
            nameof(DynamicGestures),
            DynamicGestures,
            pair => pair.Value.Name,
            i => { Debug.Log($"Ring invoked onClick Action; Name: {i.Name} Key:{i.Key}"); }
        );

        foreach (var element in dynamicGesturesRing.Elements)
        {
            var dictionary = DynamicGestures[element.Key].frames.ToDictionary(frame => frame.name, frame => frame);
            element.NextRing = Ring.CreateRing(
                $"{element.Name} frames",
                dictionary,
                pair => pair.Value.name,
                i => { Debug.Log($"Ring invoked onClick Action; Name: {i.Name} Key:{i.Key}"); }
            );
        }

        return dynamicGesturesRing;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            //SetMode(ControllerMode.Menu);

            if (ringMenu.IsActive())
            {
                return;
            }

            PrepareRingData();

            ringMenu = Instantiate(RingMenuPrefab, FindObjectOfType<Canvas>().transform);

            ringMenu.SetCallback(s => { Debug.Log($"Callback on ring menu button press: {s}"); });
            ringMenu.Open(_rings[0], _rings);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!ringMenu.IsActive())
            {
                return;
            }

            ringMenu.Close();
            //Destroy(ringMenu.gameObject);
        }
    }
}