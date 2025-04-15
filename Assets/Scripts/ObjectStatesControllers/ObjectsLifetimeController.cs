using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class ObjectsLifetimeController : MonoBehaviour
{
    #region Fields
    private List<GameObject> _observableObjects = new List<GameObject>();
    private ActionInterval _objectsLifetimeInterval;
    private float _objectsLifetimeUpdateRate = 0.1f;
    #endregion

    #region Properties
    public float ObjectLifetime { get; set; } = 1f;
    public Action<IEnumerable<GameObject>> SendObjectToDestroy { get; set; }
    #endregion

    #region Constructors
    public ObjectsLifetimeController (float LifetimeUpdate, float ObjectsLifetime, List<GameObject> StartObservableObjects = null)
    {
        if (_objectsLifetimeInterval == null)
            _objectsLifetimeInterval = new ActionInterval();

        _objectsLifetimeUpdateRate = LifetimeUpdate;

        if(StartObservableObjects != null)
            _observableObjects = StartObservableObjects;

        ObjectLifetime = ObjectsLifetime;
    }
    #endregion

    #region Methods

    public void AddNewObservableObject(GameObject NewObservableObject)
    {
        _observableObjects.Add(NewObservableObject);

        if (_observableObjects.Count > 0 && !_objectsLifetimeInterval.Busy)
        {
            ObjectsLifetimeTick();
        }
    }

    public void RemoveObservableObject(GameObject RemovedObject)
    {
        _observableObjects.Remove(RemovedObject);

        if(_observableObjects.Count == 0 && _objectsLifetimeInterval.Busy)
        {
            _objectsLifetimeInterval.Stop();
        }
    }

    private void ObjectsLifetimeTick()
    {
        Action onInterval = delegate
        {
            if (_observableObjects.Count == 0)
            {
                _objectsLifetimeInterval.Stop();
                return;
            }

            ObjectLifetime lifeTime = null;
            List<GameObject> destroyedObjects = new List<GameObject>();

            foreach (GameObject observableObject in _observableObjects)
            {
                if(observableObject == null)
                {
                    destroyedObjects.Add(observableObject);
                    continue;
                }

                if (observableObject.TryGetComponent<ObjectLifetime>(out ObjectLifetime lifetimeComponent))
                    lifeTime = lifetimeComponent;
                else
                    lifeTime = observableObject.AddComponent<ObjectLifetime>();

                lifeTime.LifeTime += _objectsLifetimeUpdateRate;

                if (lifeTime.LifeTime >= ObjectLifetime)
                {
                    destroyedObjects.Add(observableObject);
                }
            }

            if (destroyedObjects.Count > 0)
            {
                SendObjectToDestroy?.Invoke(destroyedObjects);
                destroyedObjects.Clear();
            }
        };

        _objectsLifetimeInterval.StartInterval(_objectsLifetimeUpdateRate, onInterval);
    }

    public void DestroyAllObjects()
    {
        if (_observableObjects.Count == 0)
            return;

        if (_objectsLifetimeInterval.Busy)
            _objectsLifetimeInterval.Stop();

        SendObjectToDestroy?.Invoke(_observableObjects);

        _observableObjects.Clear();
    }
    #endregion
}
