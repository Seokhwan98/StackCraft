using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckCard : MonoBehaviour
{
    [SerializeField] private Transform _checkArea;
    private Vector2 _areaSize;

    private void Awake()
    {
        
    }

    private void Start()
    {
        var size = _checkArea.localScale;
        _areaSize = new Vector2(size.x, size.y);
        Debug.Log($"size: {_areaSize.x}, {_areaSize.y}");
    }

    private void Update()
    {
        //CheckTargetInArea();
    }

    private void CheckTargetInArea()
    {
        Vector2 center = _checkArea.position;
        Rect area = new Rect(center - _areaSize / 2f, _areaSize);
        
        
    }

    
}
