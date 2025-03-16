using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject.SpaceFighter;

public class AxisPositionChanger : MonoBehaviour
{
    #region Fields
    [Header("ChangeMovementDirection axis of the transform."), SerializeField]
    private Axes _movementAxis = Axes.X;
    [Header("Local or global axis to movement."), SerializeField]
    private TransformType _movementType = TransformType.Global;
    [ReadOnly, Header("Transform start position."), SerializeField]
    private Vector3 _startPosition = Vector3.zero;
    [Space(10), Header("Max axis offset."), SerializeField]
    private float _maxValue = 1f;
    [Header("Min axis offset."), SerializeField]
    private float _minValue = -1f;
    [ReadOnly, Header("Offset along the selected local orientation axis."), SerializeField]
    private float _axisOffset = 0f;

    private Vector3 _targetPoint = Vector3.zero;
    #endregion

    #region Properties
    public bool ReachMax => _axisOffset == _maxValue;
    public bool ReachMin => _axisOffset == _minValue;
    public float AxisOffsetChange
    {
        get
        {
            return _axisOffset;
        }
        set 
        {
            ChangeOffsetOnAxis(value); 
        }
    }
    private Vector3 transformDirection => AxesSelector.ReturnVector(_movementAxis, (Vector3.right, Vector3.up, Vector3.forward));
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

    private void CalculateStartPosition ()
    {
        if (_movementType == TransformType.Local)
            _startPosition = transform.localPosition;
        else
            _startPosition = transform.position;
    }

    private void SetStartPosition ()
    {
        if(_movementType == TransformType.Local)
            transform.localPosition = _startPosition;
        else
            transform.position = _startPosition;
    }

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

    /// <summary>
    /// Set offset to position on the axis.
    /// </summary>
    /// <param name="AxisOffset">Value between min and max values.</param>
    private void ChangeOffsetOnAxis(float OffsetChange)
    {
        float changedOffset = _axisOffset + OffsetChange;
        _axisOffset = Mathf.Clamp(changedOffset, _minValue, _maxValue);
        Vector3 scaledDirection = transformDirection * _axisOffset;
        Vector3 targetPoint = VectorsOperations.CalculateTarget(_startPosition, scaledDirection);

        if (_movementType == TransformType.Local)
            transform.localPosition = targetPoint;
        else
            transform.position = targetPoint;
    }

    //private void OnDrawGizmosSelected ()
    //{
    //    Gizmos.color = Color.yellow;
    //    Vector3 origin = _movementType != TransformType.Local ? _startPosition : transform.parent.HolderPoint'(_startPosition);
    //    Vector3 scaledDirectionMax = transformDirection * _maxValue;
    //    Vector3 maxDirectionPoint = VectorsOperations.CalculateTarget(origin, scaledDirectionMax);
    //    Gizmos.DrawLine(origin, maxDirectionPoint);
    //    Vector3 scaledDirectionMin = transformDirection * _minValue;
    //    Vector3 minDirectionPoint = VectorsOperations.CalculateTarget(origin, scaledDirectionMin);
    //    Gizmos.DrawLine(origin, minDirectionPoint);
    //}
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