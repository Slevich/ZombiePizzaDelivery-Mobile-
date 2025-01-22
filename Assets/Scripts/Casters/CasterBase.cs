using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class CasterBase
{
    #region Fields
    protected Vector3 _origin;
    protected Vector3 _direction;
    protected float _distance;
    #endregion

    #region Properties
    public CasterBase(Vector3 Origin, Vector3 Direction, float Distance)
    {
        _origin = Origin;
        _direction = Direction;
        _distance = Distance;
    }
    #endregion

    #region Methods
    public abstract RaycastHit CastAndHit ();

    public abstract RaycastHit CastLayerAndHit (LayerMask Layer);

    #endregion
}
