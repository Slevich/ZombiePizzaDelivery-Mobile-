using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TransformInfo
{
    #region Fields
    private Transform _lookingTransform;
    private InfoUpdateType _updateType;

    private Vector3 _localPosition;
    private Vector3 _globalPosition;
    private Vector3 _localRotationEulers;
    private Vector3 _globalRotationEulers;
    private Quaternion _localRotationQuaternion;
    private Quaternion _globalRotationQuaternion;
    #endregion

    #region Constructor
    public TransformInfo (Transform LookingTransform, InfoUpdateType UpdateType = InfoUpdateType.Updatable)
    {
        _lookingTransform = LookingTransform;
        _updateType = UpdateType;

        _localPosition = _lookingTransform.localPosition;
        _globalPosition = _lookingTransform.position;
        _localRotationEulers = _lookingTransform.localRotation.eulerAngles;
        _globalRotationEulers = _lookingTransform.rotation.eulerAngles;
        _localRotationQuaternion = _lookingTransform.localRotation;
        _globalRotationQuaternion = _lookingTransform.rotation;
    }
    #endregion

    #region Properties
    public Vector3 LocalPosition
    {
        get
        {
            switch(_updateType)
            {
                case InfoUpdateType.Sealed:
                    return _localPosition;

                case InfoUpdateType.Updatable:
                    return _lookingTransform.localPosition;

                default:
                    return _lookingTransform.localPosition;
            }
        }
    }

    public Vector3 GlobalPosition
    {
        get
        {
            switch (_updateType)
            {
                case InfoUpdateType.Sealed:
                    return _globalPosition;

                case InfoUpdateType.Updatable:
                    return _lookingTransform.position;

                default:
                    return _lookingTransform.position;
            }
        }
    }
    public Vector3 LocalRotationEulers
    {
        get
        {
            switch (_updateType)
            {
                case InfoUpdateType.Sealed:
                    return _localRotationEulers;

                case InfoUpdateType.Updatable:
                    return _lookingTransform.localRotation.eulerAngles;

                default:
                    return _lookingTransform.localRotation.eulerAngles;
            }
        }
    }
    public Vector3 GlobalRotationEulers
    {
        get
        {
            switch (_updateType)
            {
                case InfoUpdateType.Sealed:
                    return _globalRotationEulers;

                case InfoUpdateType.Updatable:
                    return _lookingTransform.rotation.eulerAngles;

                default:
                    return _lookingTransform.rotation.eulerAngles;
            }
        }
    }
    public Quaternion LocalRotationQuaternion
    {
        get
        {
            switch (_updateType)
            {
                case InfoUpdateType.Sealed:
                    return _localRotationQuaternion;

                case InfoUpdateType.Updatable:
                    return _lookingTransform.localRotation;

                default:
                    return _lookingTransform.localRotation;
            }
        }
    }
    public Quaternion GlobalRotationQuaternion
    {
        get
        {
            switch (_updateType)
            {
                case InfoUpdateType.Sealed:
                    return _globalRotationQuaternion;

                case InfoUpdateType.Updatable:
                    return _lookingTransform.rotation;

                default:
                    return _lookingTransform.rotation;
            }
        }
    }
    #endregion
}

public enum InfoUpdateType
{
    Updatable,
    Sealed
}
