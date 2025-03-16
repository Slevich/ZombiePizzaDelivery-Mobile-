using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidbodyManipulator : MonoBehaviour
{
    [Header("Rigidbody to manipulate."), SerializeField] private Rigidbody _rigidBody;

    private bool _hasRigidbody
    {
        get
        {
            if(!_rigidBody)
            {
                if(TryGetComponent<Rigidbody>(out Rigidbody rb))
                    _rigidBody = rb;
            }

            return _rigidBody != null;
        }
    }

    public void SetVelocity(Vector3 Velocity)
    {
        if (!_hasRigidbody)
            return;

        _rigidBody.velocity = Velocity;
    }
}
