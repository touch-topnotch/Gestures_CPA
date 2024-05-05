using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using ModestTree;
using Scripts.Static.Extensions;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Debugger = Scripts.Systems.Debugger;

public class RingMenu : MonoBehaviour
{
    [SerializeField] private RingCakePiece RingElementPrefab;
    [SerializeField] private float _gapWidthDegree = 1f;
    private readonly Dictionary<string, Ring> _rings = new Dictionary<string, Ring>();
    private readonly List<RingCakePiece> _piecePool = new();
    private RingCakePiece[] _spawnedPieces;
    private string lastOpenedRing;
    public void SetRings(List<Ring> list, string first = null)
    {
        _rings.Clear();
        list.ForEach(e =>
        {
            if (e.sectors == null)
                Debug.Log("Ring " + e.name + " doesn't contain sectors!");
            else
            {
                first ??= e.name;
                _rings.Add(e.name, e);
                
            }
        });
        lastOpenedRing = first;
    }
    public void OpenRing(string key)
    {
        if (!_rings.ContainsKey(key))
        {
            Debug.Log("Ring " + key + " doesn't exist in dictionary!");
            return;
        }
        if(lastOpenedRing != key)
            Close();

        lastOpenedRing = key;
        var ring = _rings[key];
        var stepLength = 360f / ring.sectors.Count;
        var iconDist = Vector3.Distance(RingElementPrefab.Icon.transform.position,
            RingElementPrefab.CakePiece.transform.position);

        _spawnedPieces = new RingCakePiece[ring.sectors.Count];
        for (int i = 0; i < ring.sectors.Count; i++)
        {
            _spawnedPieces[i] = PoolSector();
            //set root element
            _spawnedPieces[i].transform.localPosition = Vector3.zero;
            _spawnedPieces[i].transform.localRotation = Quaternion.identity;

            //set cake piece
            _spawnedPieces[i].CakePiece.fillAmount = 1f / ring.sectors.Count - _gapWidthDegree / 360f;
            _spawnedPieces[i].CakePiece.transform.localPosition = Vector3.zero;
            _spawnedPieces[i].CakePiece.transform.localRotation =
                Quaternion.Euler(0, 0, -stepLength / 2f + _gapWidthDegree / 2f + i * stepLength);
            _spawnedPieces[i].CakePiece.color = new Color(1f, 1f, 1f, 0.5f);

            //set icon
            var transformLocalPosition = _spawnedPieces[i].CakePiece.transform.localPosition +
                                         Quaternion.AngleAxis(i * stepLength, Vector3.forward) *
                                         Vector3.up * iconDist;
            //_pieces[i].Icon.transform.localPosition = transformLocalPosition;
            //_pieces[i].Icon.sprite = _currentTabData.Elements[i].Icon;

            RectTransform iconRect = _spawnedPieces[i].Icon.GetComponent<RectTransform>(); // Получаем RectTransform иконки
            float halfIconHeight = iconRect.sizeDelta.y / 2f; // Половина высоты иконки

            _spawnedPieces[i].PieceLabel.transform.localPosition =
                transformLocalPosition - new Vector3(0, halfIconHeight, 0);

            _spawnedPieces[i].PieceLabel.text = ring.sectors[i].props.name;
        }
        
        // RectTransform cakePieceRenderer = pieces[0].Icon.GetComponent<RectTransform>();
        // float cakePieceTopY = cakePieceRenderer.position.y; // Верхняя точка CakePiece
        // float screenHeight = Screen.height;
        // float tabBarYPosition = cakePieceTopY + (screenHeight * 0.20f); // 10% от верхней точки CakePiece до края экрана
        //
        // var transform1 = ringTabBar.transform;
        // var position = transform1.position;
        // position = new Vector3(position.x, tabBarYPosition,
        //     position.z);
        // transform1.position = position;
        //
        // _ringTabBar.SetCurrentTabText("Current : " + ring.name);
    }
    
    private void Update()
    {
        if (!isActive)
            return;

        var sectorsCount = _rings[lastOpenedRing].sectors.Count;
        var stepLength = 360f / sectorsCount;
        var mouseAngle =
            NormalizeAngle(Vector3.SignedAngle(Vector3.up, Input.mousePosition - transform.position, Vector3.forward) +
                           stepLength / 2f);
        var activeElement = (int)(mouseAngle / stepLength);
        for (int i = 0; i < sectorsCount; i++)
        {
            if (i == activeElement)
                _spawnedPieces[i].CakePiece.color = new Color(0.5f, 0.5f, 0.5f, 0.8f);
            else
                _spawnedPieces[i].CakePiece.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        if (Input.GetMouseButtonDown(0))
        {
            var props = _rings[lastOpenedRing].sectors[activeElement].props;
            props.onClick?.Invoke(props.name);
        }
    }
    public void Close()
    {
        HideElements();
    }
    private void HideElements()
    {
        if (_piecePool.IsEmpty())
            return;
        foreach (var var in _piecePool)
        {
            var.gameObject.SetActive(false);
        }
    }
    private float NormalizeAngle(float a) => (a + 360f) % 360f;
    private bool _toggleAllowed = false;
    
    private bool _isActive = false;
    public bool isActive
    {
        get => _isActive;
        set
        {
         
            _isActive = value;
            if (_isActive && _rings.ContainsKey(lastOpenedRing))
            {
                OpenRing(lastOpenedRing);
            }
            else
            {
                Close();
            }
        }
    }
    private RingCakePiece PoolSector()
    {
        foreach (var piece in _piecePool)
        {
            if (!piece.gameObject.activeSelf)
            {
                piece.gameObject.SetActive(true);
                return piece;
            }
        }

        var newSector = Instantiate(RingElementPrefab, transform);
        _piecePool.Add(newSector);
        return newSector;
    }
}