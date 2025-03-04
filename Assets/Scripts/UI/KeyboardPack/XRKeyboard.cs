using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.UI;

namespace UI.KeyboardPack
{
    public class XRKeyboard : MonoBehaviour
    {
        private static XRKeyboard _instance;

        public static XRKeyboard instance => _instance;
        
        [Range(0, 10)] [SerializeField] private float speed;
        [SerializeField] private Vector3 offset;
        [SerializeField] private Transform arcCenter;
        [SerializeField] private float arcRadius;
        [SerializeField] private float arcLength;
        [SerializeField] private RectTransform background;
        [SerializeField] private GameObject buttonPrefab;
        [SerializeField] private GameObject capsButtonPrefab;
        [SerializeField] private GameObject enterButtonPrefab;
        [SerializeField] private string[] sequences;
        [SerializeField] private Transform targetPoint;
        [SerializeField] private List<KeyboardButton> buttons;

        public event Action<string> OnButtonClick;
        public event Action OnBackSpaceClick;
        public event Action<bool> OnCapsToggle;
        public event Action OnEnterClick;

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
            for (int i = 0; i < transform.GetChild(0).childCount; i++)
            {
                var buttonObject = transform.GetChild(0).GetChild(i);
                if (buttonObject.TryGetComponent<KeyboardButton>(out var button))
                {
                    switch (button.type)
                    {
                        case ButtonType.simple:
                            button.OnClick += (e) =>
                            {
                                OnButtonClick?.Invoke(e);
                            };
                            break;
                        case ButtonType.backspace:
                            button.OnClick += (e) =>
                            {
                                OnBackSpaceClick?.Invoke();
                            };
                            break;
                        case ButtonType.capslock:
                            button.GetComponent<Toggle>().onValueChanged.AddListener((e)=>
                            {
                                CapsButtons(e);
                                OnCapsToggle?.Invoke(e);
                            });
                            break;
                        case ButtonType.enter:
                            button.OnClick += (e) =>
                            {
                                OnEnterClick?.Invoke();
                                this.gameObject.SetActive(false);
                            };
                            break;
                    }
                }
            }

          
        }

        public void Start()
        {
            transform.SetParent(null);
         //   gameObject.SetActive(false);
        }
#if UNITY_EDITOR

        public void OnDrawGizmos()
        {
            ArcDrawer.DrawWireArc(
                arcCenter.position, Vector3.forward, arcLength, arcRadius, 30);
        }

        [Button("Clean")]
        public void Clean()
        {
            buttons.Clear();
            foreach (var VARIABLE in background.GetComponentsInChildren<RectTransform>())
            {
                if (VARIABLE != background && VARIABLE&&VARIABLE.name != "ArcCenter")
                    DestroyImmediate(VARIABLE.gameObject);
            }
        }

        [Button("Build Keyboard")]
        public void BuildKeyboard()
        {
            Clean();
            var p = arcCenter.position;
            for (int i = 0; i < sequences.Length; i++)
            {
                float srcAngles = GetAnglesFromDir(arcCenter.position/ GetComponent<RectTransform>().localScale.x, Vector3.forward);
                var stepAngles = arcLength / sequences[i].Length;
                var angle = srcAngles + arcLength / 2 - 0.5f * stepAngles;
                // for each symbol
                for (int j = 0; j < sequences[i].Length; j++)
                {
                    KeyboardButton button;
                    if (sequences[i][j] == '>')
                    {
                        button = PrefabUtility.InstantiatePrefab(enterButtonPrefab, background).GetComponent<KeyboardButton>();
                        button.type = ButtonType.enter;
                    }
                    else if (sequences[i][j] == '^')
                    {
                        button = PrefabUtility.InstantiatePrefab(capsButtonPrefab, background).GetComponent<KeyboardButton>();
                        button.type = ButtonType.capslock;
                    }
                    else
                    {
                        button = PrefabUtility.InstantiatePrefab(buttonPrefab, background).GetComponent<KeyboardButton>();
                        button.type = sequences[i][j] == '<' ? ButtonType.backspace : ButtonType.simple;
                    }


                    switch (button.type)
                    {
                        case ButtonType.backspace:
                            button.type = ButtonType.backspace;
                            button.SetText("←");
                            break;
                        case ButtonType.capslock:
                            button.SetText("↑");
                            break;
                        case ButtonType.enter:
                            break;
                        default:
                            button.type = ButtonType.simple;
                            button.SetText(sequences[i][j].ToString());
                            buttons.Add(button);
                            break;
                    }

                
                    var rad = Mathf.Deg2Rad * angle;
                    angle -= stepAngles;
                    var localScale = GetComponent<RectTransform>().localScale.x;
                    Vector3 pos = new Vector3((p.x + arcRadius * Mathf.Cos(rad)/localScale),
                        p.y + (sequences.Length / 2 - i) * 50,
                        ( arcRadius * Mathf.Sin(rad) - arcRadius)/localScale);

                    button.GetComponent<RectTransform>().anchoredPosition3D = pos;
                    button.GetComponent<RectTransform>().rotation =
                        Quaternion.LookRotation(pos - arcCenter.position/localScale);
                }
            }
        }
        
    

        public static void DrawWireArc(Vector3 position, Vector3 dir, float anglesRange, float radius, float maxSteps = 20) 
        {
            var srcAngles = GetAnglesFromDir(position, dir);
            var initialPos = position;
            var posA = initialPos;
            var stepAngles = anglesRange / maxSteps;
            var angle = srcAngles - anglesRange / 2;
            for (var i = 0; i <= maxSteps; i++)
            {
                var rad = Mathf.Deg2Rad * angle;
                var posB = initialPos;
                posB += new Vector3(radius * Mathf.Cos(rad), 0, radius * Mathf.Sin(rad));

                Gizmos.DrawLine(posA, posB);

                angle += stepAngles;
                posA = posB;
            }
            Gizmos.DrawLine(posA, initialPos);
        }

        static float GetAnglesFromDir(Vector3 position, Vector3 dir)
        {
            var forwardLimitPos = position + dir;
            var srcAngles = Mathf.Rad2Deg * Mathf.Atan2(forwardLimitPos.z - position.z, forwardLimitPos.x - position.x);
            return srcAngles;
        }
            #endif
        

        private void CapsButtons(bool capsed)
        {
            foreach (var button in buttons)
            {
                button.SetText(capsed ? button.GetText().ToString().ToUpper() : button.GetText().ToString().ToLower());
            }
        }

        //privat
        private void FixedUpdate()
        {
            transform.position = Vector3.Lerp(transform.position, targetPoint.position + offset, speed * Time.fixedDeltaTime);
            
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(-targetPoint.position + transform.position- Vector3.up*offset.y)
                , speed * Time.fixedDeltaTime);
        }

        
    }
}