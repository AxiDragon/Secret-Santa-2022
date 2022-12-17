using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UIVirtualJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Rect References")] public RectTransform containerRect;

    public RectTransform handleRect;

    [Header("Settings")] public float joystickRange = 50f;

    public float magnitudeMultiplier = 1f;
    public bool invertXOutputValue;
    public bool invertYOutputValue;

    [Header("Output")] public Event joystickOutputEvent;

    private void Start()
    {
        SetupHandle();
    }

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, eventData.position,
            eventData.pressEventCamera, out var position);

        position = ApplySizeDelta(position);

        var clampedPosition = ClampValuesToMagnitude(position);

        var outputPosition = ApplyInversionFilter(position);

        OutputPointerEventValue(outputPosition * magnitudeMultiplier);

        if (handleRect) UpdateHandleRectPosition(clampedPosition * joystickRange);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OutputPointerEventValue(Vector2.zero);

        if (handleRect) UpdateHandleRectPosition(Vector2.zero);
    }

    private void SetupHandle()
    {
        if (handleRect) UpdateHandleRectPosition(Vector2.zero);
    }

    private void OutputPointerEventValue(Vector2 pointerPosition)
    {
        joystickOutputEvent.Invoke(pointerPosition);
    }

    private void UpdateHandleRectPosition(Vector2 newPosition)
    {
        handleRect.anchoredPosition = newPosition;
    }

    private Vector2 ApplySizeDelta(Vector2 position)
    {
        var x = position.x / containerRect.sizeDelta.x * 2.5f;
        var y = position.y / containerRect.sizeDelta.y * 2.5f;
        return new Vector2(x, y);
    }

    private Vector2 ClampValuesToMagnitude(Vector2 position)
    {
        return Vector2.ClampMagnitude(position, 1);
    }

    private Vector2 ApplyInversionFilter(Vector2 position)
    {
        if (invertXOutputValue) position.x = InvertValue(position.x);

        if (invertYOutputValue) position.y = InvertValue(position.y);

        return position;
    }

    private float InvertValue(float value)
    {
        return -value;
    }

    [Serializable]
    public class Event : UnityEvent<Vector2>
    {
    }
}