using System;
using System.Collections.Generic;
using UnityEngine;

public class RingMenu : MonoBehaviour
{
    [SerializeField] private RingCakePiece RingElementPrefab;
    [SerializeField] private RingTabBar RingTabBarPrefab;

    private List<Ring> _tabs;
    private Ring _currentTabData;
    private Stack<Ring> ringStack = new Stack<Ring>();
    private float _gapWidthDegree = 1f;
    private Action<string> _callback;
    private RingCakePiece[] _pieces;
    private RingTabBar _ringTabBar;
    private RingMenu _parent;
    private string _path = "";

    private bool _isActive = false;

    private void Update()
    {
        if (!_isActive)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (_tabs.Count > 1)
            {
                var indexOf = _tabs.IndexOf(_currentTabData);
                if (indexOf + 1 == _tabs.Count)
                {
                    //нужно переключить на первый элемент
                    indexOf = 0;
                }
                else
                {
                    //тыкаем следующий табл
                    indexOf += 1;
                }


                //var newSubRing = Instantiate(gameObject, transform.parent).GetComponent<RingMenu>();
                //for (var j = 0; j < newSubRing.transform.childCount; j++)
                //    Destroy(newSubRing.transform.GetChild(j).gameObject);
//
                //newSubRing.SetCallback(_callback);
                //newSubRing.Open(_tabs[indexOf], _tabs);

                DestroyElements();
                var path = "/" + _tabs[indexOf].Name;
                ringStack.Clear();
                ringStack.Push(_tabs[indexOf]);
                SetPath(path);
                OpenNext(_tabs[indexOf], _tabs);

                //Destroy(gameObject);
            }
            else
            {
                return;
            }
        }

        var stepLength = 360f / _currentTabData.Elements.Count;
        var mouseAngle =
            NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - transform.position, Vector3.forward) +
                           stepLength / 2f);
        var activeElement = (int)(mouseAngle / stepLength);
        for (int i = 0; i < _currentTabData.Elements.Count; i++)
        {
            if (i == activeElement)
                _pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.75f);
            else
                _pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);
        }


        if (Input.GetMouseButtonDown(0))
        {
            var path = _path + "/" + _currentTabData.Elements[activeElement].Name;
            if (_currentTabData.Elements[activeElement].NextRing != null)
            {
                DestroyElements();
                //var newSubRing = Instantiate(gameObject, transform.parent).GetComponent<RingMenu>();
                //for (var j = 0; j < newSubRing.transform.childCount; j++)
                //    Destroy(newSubRing.transform.GetChild(j).gameObject);
                //newSubRing.SetParent(this);
                //newSubRing.SetPath(path);
                //newSubRing.SetCallback(_callback);
                //newSubRing.Open(_currentTabData.Elements[activeElement].NextRing, _tabs);

                ringStack.Push(_currentTabData.Elements[activeElement].NextRing);
                SetPath(path);
                OpenNext(_currentTabData.Elements[activeElement].NextRing, _tabs);
            }
            else
            {
                Close();
                _callback?.Invoke(path);
                //Destroy(gameObject);
            }
        }
    }

    public void Open(Ring tab, List<Ring> tabs)
    {
        if (_isActive)
        {
            return;
        }

        _currentTabData = tab;
        SetTabs(tabs);

        Init();
        _isActive = true;
    }

    public void OpenNext(Ring tab, List<Ring> tabs)
    {
        _currentTabData = tab;
        SetTabs(tabs);

        Init();
        _isActive = true;
    }

    public void Close()
    {
        if (!_isActive)
        {
            return;
        }

        _isActive = false;
        if (_currentTabData == null || _currentTabData.Elements.Count == 0)
        {
            return;
        }

        DestroyElements();

        _pieces = null;

        if (_parent != null)
        {
            _parent.Close();
        }

        Destroy(gameObject);
    }

    private void DestroyElements()
    {
        for (int i = 0; i < _currentTabData.Elements.Count; i++)
        {
            if (_pieces != null && _pieces.Length > 0)
            {
                if (_pieces[i] != null)
                {
                    foreach (Transform t in _pieces[i].transform)
                    {
                        Destroy(t.gameObject);
                    }

                    Destroy(_pieces[i].gameObject);
                }
            }
        }
    }

    public void SetActive(bool flag) => _isActive = flag;

    private void Init()
    {
        var stepLength = 360f / _currentTabData.Elements.Count;
        var iconDist = Vector3.Distance(RingElementPrefab.Icon.transform.position,
            RingElementPrefab.CakePiece.transform.position);

        //Position it
        _pieces = new RingCakePiece[_currentTabData.Elements.Count];

        for (int i = 0; i < _currentTabData.Elements.Count; i++)
        {
            _pieces[i] = Instantiate(RingElementPrefab, transform);
            //set root element
            _pieces[i].transform.localPosition = Vector3.zero;
            _pieces[i].transform.localRotation = Quaternion.identity;

            //set cake piece
            _pieces[i].CakePiece.fillAmount = 1f / _currentTabData.Elements.Count - _gapWidthDegree / 360f;
            _pieces[i].CakePiece.transform.localPosition = Vector3.zero;
            _pieces[i].CakePiece.transform.localRotation =
                Quaternion.Euler(0, 0, -stepLength / 2f + _gapWidthDegree / 2f + i * stepLength);
            _pieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);

            //set icon
            var transformLocalPosition = _pieces[i].CakePiece.transform.localPosition +
                                         Quaternion.AngleAxis(i * stepLength, Vector3.forward) *
                                         Vector3.up * iconDist;
            //_pieces[i].Icon.transform.localPosition = transformLocalPosition;
            //_pieces[i].Icon.sprite = _currentTabData.Elements[i].Icon;

            RectTransform iconRect = _pieces[i].Icon.GetComponent<RectTransform>(); // Получаем RectTransform иконки
            float halfIconHeight = iconRect.sizeDelta.y / 2f; // Половина высоты иконки

            _pieces[i].PieceLabel.transform.localPosition =
                transformLocalPosition - new Vector3(0, halfIconHeight, 0);

            _pieces[i].PieceLabel.text = _currentTabData.Elements[i].Name;
        }

        if (_ringTabBar == null)
        {
            _ringTabBar = Instantiate(RingTabBarPrefab, transform);
        }

        RectTransform cakePieceRenderer = _pieces[0].Icon.GetComponent<RectTransform>();
        float cakePieceTopY = cakePieceRenderer.position.y; // Верхняя точка CakePiece
        float screenHeight = Screen.height;
        float tabBarYPosition = cakePieceTopY + (screenHeight * 0.20f); // 10% от верхней точки CakePiece до края экрана

        _ringTabBar.transform.position = new Vector3(_ringTabBar.transform.position.x, tabBarYPosition,
            _ringTabBar.transform.position.z);

        _ringTabBar.SetCurrentTabText("Current : " + _currentTabData.Name);
        _ringTabBar.SetTabsCountText("Tabs count: " + _tabs.Count.ToString());
        if (_tabs.Count > 1)
        {
            _ringTabBar.SetNextTabText("Next : " + _currentTabData.Name);
        }
        else
        {
            _ringTabBar.SetNextTabText(null);
        }
    }

    private float NormalizeAngle(float a) => (a + 360f) % 360f;

    public void SetPath(string path)
    {
        _path = path;
    }

    public void SetParent(RingMenu parent)
    {
        _parent = parent;
    }

    public bool IsActive() => _isActive;

    public void SetCallback(Action<string> action)
    {
        _callback = action;
    }

    private void SetTabs(List<Ring> tabs)
    {
        _tabs = tabs;
    }
}