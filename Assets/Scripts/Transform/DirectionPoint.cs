using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectionPoint : MonoBehaviour
{
    [Header("Transform to set direction.")]
    [SerializeField] private Transform _directionPointTransform;
    [Header("Local direction axis.")]
    [SerializeField] private Axes _directionAxis = Axes.Z;

#if UNITY_EDITOR
    private void OnValidate ()
    {
        if(!_directionPointTransform)
            _directionPointTransform = transform;
    }
#endif

    public Vector3 ReturnWorldAxisDirection()
    {
        Vector3 localPosition = transform.localPosition;
        Vector3 localDirection = AxesSelector.ReturnVector(_directionAxis, (transform.right, transform.up, transform.forward));
        Vector3 worldDirection = transform.TransformDirection(localDirection);
        return worldDirection;
    }
}
