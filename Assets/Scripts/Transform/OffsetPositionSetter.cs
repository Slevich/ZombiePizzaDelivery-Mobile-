using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OffsetPositionSetter : MonoBehaviour
{
    #region Fields
    [Header("Transform for the offset reliance.")]
    [SerializeField] private Transform _anchor;
    [Header("Offset rely on anchor position")]
    [SerializeField] private Vector3 _offset;
    #endregion

    #region Properties
    public Transform Anchor => _anchor;
    #endregion

    #region Methods
    public void SetPositionWithOffset() => transform.position = CalculateOffset();

    private Vector3 CalculateOffset()
    {
        if (_anchor == null)
            return Vector3.zero;

        if (_anchor.parent != null)
            return _anchor.parent.TransformPoint(_anchor.localPosition + _offset);
        else
            return _anchor.position + _offset;
    }
    #endregion
}

[CustomEditor(typeof(OffsetPositionSetter))]
public class OffsetSetterEditor : Editor
{
    public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();
        OffsetPositionSetter offsetSetter = target as OffsetPositionSetter;

        if(!Application.IsPlaying(serializedObject.targetObject) && offsetSetter.Anchor != null)
            offsetSetter.SetPositionWithOffset();
    }
}