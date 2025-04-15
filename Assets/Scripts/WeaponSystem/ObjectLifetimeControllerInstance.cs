using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class ObjectLifetimeControllerInstance : MonoBehaviour, IObservableCollectionChanged
{
    #region Fields
    [Header("Lifetime checking tick in seconds."), SerializeField] private float _lifetimeUpdateTime = 0.1f;
    [Header("Objects lifetime in seconds."), SerializeField] private float _objectLifetime = 1f;

    private ObjectsLifetimeController _controller;
    #endregion

    #region Properties
    public Action<IEnumerable<GameObject>> SendObjectsToRemove { get; set; }
    #endregion

    #region Methods
    private void Awake ()
    {
        _controller = new ObjectsLifetimeController(_lifetimeUpdateTime, _objectLifetime);
        _controller.SendObjectToDestroy += (objectsToDestroy) => SendObjectsToRemove?.Invoke(objectsToDestroy); 
    }

    public void CollectionChanged (object? sender, NotifyCollectionChangedEventArgs args)
    {
        GameObject changedObject = null;

        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                changedObject = args.NewItems[0] as GameObject;
                _controller.AddNewObservableObject(changedObject);
                break;

            case NotifyCollectionChangedAction.Remove:
                changedObject = args.OldItems[0] as GameObject;
                _controller.RemoveObservableObject(changedObject);
                break;

            default:
                break;
        }
    }

    public void StopObserving() => _controller.DestroyAllObjects();
    #endregion
}
