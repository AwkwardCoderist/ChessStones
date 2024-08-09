using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class ModelRotater : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Transform _center;
    [SerializeField] private Transform _rotate;
    [SerializeField] private float _sensivity = 1;

    public bool Dragging;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Dragging = true;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Dragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //_rotate.rotation = Quaternion.AngleAxis(eventData.delta.y * _sensivity, Vector3.right) * _rotate.rotation;
        //_rotate.rotation = Quaternion.AngleAxis(-eventData.delta.x * _sensivity, Vector3.up) * _rotate.rotation;
        _rotate.rotation = Quaternion.Euler(eventData.delta.y * _sensivity, -eventData.delta.x * _sensivity, 0) * _rotate.rotation;
    }
}
