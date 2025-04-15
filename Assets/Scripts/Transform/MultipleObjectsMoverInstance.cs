using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class MultipleObjectsMoverInstance : MonoBehaviour, IObservableCollectionChanged
{
    #region Fields
    [Header("Time in seconds to update objects movement."), SerializeField] private float _movementUpdate = 0.01f;

    private MultipleObjectsMover _mover;
    #endregion

    #region Properties
    public Action<IEnumerable<GameObject>> SendObjectsToRemove { get; set; }
    #endregion

    #region Methods
    private void Awake ()
    {
        _mover = new MultipleObjectsMover(_movementUpdate);
    }

    public void CollectionChanged (object? sender, NotifyCollectionChangedEventArgs args)
    {
        GameObject changedObject = null;

        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                changedObject = args.NewItems[0] as GameObject;
                _mover.AddNewMovingObject(changedObject.transform);
                break;

            default:
                break;
        }
    }

    public void StopObserving() => _mover.StopMovement();
    #endregion
}
