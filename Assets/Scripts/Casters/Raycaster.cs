using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.UI.Image;

public class Raycaster : CasterBase
{
    public Raycaster(Vector3 Origin, Vector3 Direction, float Distance) : base (Origin, Direction, Distance) 
    { }

    public override RaycastHit CastAndHit ()
    {
        if (Physics.Raycast(_origin, _direction, out RaycastHit hitResult, _distance))
            return hitResult;
        else
            return new RaycastHit();
    }

    public override RaycastHit CastLayerAndHit (LayerMask Layer)
    {
        if(Physics.Raycast(_origin, _direction, out RaycastHit hitResult, _distance, Layer))
            return hitResult;
        else
            return new RaycastHit();
    }
}
