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

    #region Constructor
    public MultipleObjectsMover(float ObjectMoveUpdate)
    {
        if(_objectsMovementInterval == null)
            _objectsMovementInterval = new ActionInterval();

        _objectsMovementUpdateRate = ObjectMoveUpdate;
    }
    #endregion

    #region Methods
    private void ObjectsMovementTick ()
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

                movement.LerpByDirection();
            }

            if (destroyedTransforms.Count == 0)
                return;

            foreach (Transform destroyedTransform in destroyedTransforms)
            {
                _movingTransforms.Remove(destroyedTransform);
            }
        };

        _objectsMovementInterval.StartInterval(_objectsMovementUpdateRate, onInterval);
    }

    public void AddNewMovingObject(Transform MovingTransform)
    {
        _movingTransforms.Add(MovingTransform);

        if (_objectsMovementInterval.Busy)
            return;

        ObjectsMovementTick();
    }

    public void StopMovement()
    {
        if (_objectsMovementInterval.Busy)
            _objectsMovementInterval.Stop();

        _movingTransforms.Clear();
    }
    #endregion
}
