using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class MultipleObjectsMover
{
    #region Fields
    private List<Transform> _movingTransforms = new List<Transform>();
    private ActionInterval _objectsMovementInterval;
    private float _objectsMovementUpdateRate = 0.01f;
    #endregion

    #region Properties
    public float Speed { get; set; } = 0.1f;
    #endregion

    #region Constructor
    public MultipleObjectsMover()
    {
        if(_objectsMovementInterval == null)
            _objectsMovementInterval = new ActionInterval();
    }
    #endregion

    #region Methods
    private void ObjectsMovementTick (Vector3 movementDirection, float speedModifier)
    {
        Action onInterval = delegate
        {
            if (_movingTransforms.Count == 0)
            {
                _objectsMovementInterval.Stop();
                return;
            }

            List<Transform> destroyedTransforms = new List<Transform>();

            foreach (Transform movingTransform in _movingTransforms)
            {
                if(movingTransform == null)
                {
                    destroyedTransforms.Add(movingTransform);
                    continue;
                }

                DirectiveMovement movement = null;

                if (movingTransform.TryGetComponent<DirectiveMovement>(out DirectiveMovement directiveMovement))
                    movement = directiveMovement;
                else
                    movement = movingTransform.gameObject.AddComponent<DirectiveMovement>();

                movement.LerpByDirection(movementDirection, speedModifier);
            }

            foreach (Transform destroyedTransform in destroyedTransforms)
            {
                _movingTransforms.Remove(destroyedTransform);
            }
        };

        _objectsMovementInterval.StartInterval(_objectsMovementUpdateRate, onInterval);
    }

    public void AddNewMovingObject(Transform MovingTransform, Vector3 MovementDirection, float SpeedModifier)
    {
        _movingTransforms.Add(MovingTransform);

        if (_objectsMovementInterval.Busy)
            return;

        ObjectsMovementTick(MovementDirection, SpeedModifier);
    }

    public void StopMovement()
    {
        if (_objectsMovementInterval.Busy)
            _objectsMovementInterval.Stop();

        _movingTransforms.Clear();
    }
    #endregion
}
