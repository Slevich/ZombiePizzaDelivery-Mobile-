using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Zenject.SpaceFighter;

public class AxisPositionChanger : MonoBehaviour
{
    #region Fields
    [Header("Local axis of the transform."), SerializeField]
    private Axes _movementAxis = Axes.X;
    [ReadOnly, Header("Transform start position."), SerializeField]
    private Vector3 _startPosition = Vector3.zero;
    [Space(10), Header("Max axis offset."), SerializeField]
    private float _maxValue = 1f;
    [Header("Min axis offset."), SerializeField]
    private float _minValue = -1f;
    [ReadOnly, Header("Offset along the selected local orientation axis."), SerializeField]
    private float _axisOffset = 0f;

    private Vector3 _direction = Vector3.zero;
    #endregion

    #region Properties
    public float MaxAxisValue => _maxValue;
    public float MinAxisValue => _minValue;
    private Vector3 transformDirection => AxesSelector.ReturnVector(_movementAxis, (transform.right, transform.up, transform.forward));
    private bool _hasParent => transform.parent != null;
    #endregion

    #region Methods
    private void Awake ()
    {
        _axisOffset = 0f;
        SetStartPosition();
        CalculateMovementBorders();
    }

    private void OnValidate ()
    {
        CalculateMovementBorders();  
    }

    private void CalculateStartPosition () => _startPosition = transform.position;
    private void SetStartPosition () => transform.position = _startPosition;

    private void CalculateMovementBorders()
    {
        if (_minValue >= 0)
            _minValue = -1;
        else if (_maxValue <= 0)
            _maxValue = 1;

        if (_minValue != -_maxValue)
            _minValue = -_maxValue;
        else if (_maxValue != -_minValue)
            _maxValue = -_minValue;
    }

    private Vector3 CalculateDirection(Vector3 target, Vector3 origin)
    {
        Vector3 heading = target - origin;
        float distance = heading.magnitude;
        Vector3 direction = heading / distance;
        return direction;
    }

    private Vector3 CalculateTransformDirection ()
    {
        Vector3 localStartPoint = _hasParent ? transform.parent.InverseTransformPoint(_startPosition) : _startPosition;
        Vector3 targetPoint = _startPosition + transformDirection;
        Vector3 localDirection = CalculateDirection(targetPoint, _startPosition);
        Vector3 globalDirection = _hasParent ? transform.parent.TransformPoint(localDirection) : localDirection;

        return globalDirection * _axisOffset;
    }

    /// <summary>
    /// Set offset to position on the axis.
    /// </summary>
    /// <param name="AxisOffset">Value between min and max values.</param>
    public void SetPositionOffsetOnAxis(float AxisOffset)
    {
        if (AxisOffset > _maxValue || _axisOffset > _maxValue)
            _axisOffset = _maxValue;
        else if (AxisOffset < _minValue || _axisOffset < _minValue)
            _axisOffset = _minValue;
        else
            _axisOffset = AxisOffset;

        _direction = CalculateTransformDirection();
        transform.position = _startPosition + _direction;
    }
    #endregion
}

[CustomEditor(typeof(AxisPositionChanger))]
public class AxisPositionChangerEditor: Editor
{
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();
        serializedObject.Update();
        AxisPositionChanger changer = target as AxisPositionChanger;

        bool calculatePositionButtonPressed = GUILayout.Button("Calculate position!");
        if (calculatePositionButtonPressed)
        {
            MethodInfo calculateStartPositionMethod = changer.GetType().GetMethod("CalculateStartPosition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            calculateStartPositionMethod.Invoke(changer, null);
        }

        bool setPositionButtonPressed = GUILayout.Button("Set position!");
        if (setPositionButtonPressed)
        {
            MethodInfo setMethod = changer.GetType().GetMethod("SetStartPosition", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            setMethod.Invoke(changer, null);
        }

        serializedObject.ApplyModifiedProperties();
    }
}