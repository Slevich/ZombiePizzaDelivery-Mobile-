using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectiveMovement : MonoBehaviour
{
    public void MoveByDirection(Vector3 Direction)
    {
        Vector3 currentWorldPosition = transform.position;
        transform.position = currentWorldPosition + Direction;
    }
    public void LerpByDirection (Vector3 Direction, float AccelerationModifier)
    {
        Vector3 currentWorldPosition = transform.position;
        Vector3 targetPoint = currentWorldPosition + Direction;
        Vector3 objectInterpolatedPosition = Vector3.Lerp(currentWorldPosition, targetPoint, AccelerationModifier);
        transform.position = objectInterpolatedPosition;
    }
}
