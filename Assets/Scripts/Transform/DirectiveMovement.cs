using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectiveMovement : MonoBehaviour
{
    [field: Header("Speed modifier."), SerializeField, ReadOnly] public float Speed { get; set; } = 1f;
    public Vector3 Direction { get; set; } = Vector3.zero;


    public void MoveByDirection(Vector3 Direction)
    {
        Vector3 currentWorldPosition = transform.position;
        transform.position = currentWorldPosition + Direction;
    }

    public void LerpByDirection ()
    {
        Vector3 currentWorldPosition = transform.position;
        Vector3 targetPoint = currentWorldPosition + Direction;
        Vector3 objectInterpolatedPosition = Vector3.Lerp(currentWorldPosition, targetPoint, Speed);
        transform.position = objectInterpolatedPosition;
    }
}
