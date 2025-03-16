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
    #endregion

    #region Constructors
    public ObjectsLifetimeController ()
    {
        if (_objectsLifetimeInterval == null)
            _objectsLifetimeInterval = new ActionInterval();
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

    private void ObjectsLifetimeTick ()
    {
        float _intervalTime = 0f;

        Action onInterval = delegate
        {
            _intervalTime += _objectsLifetimeUpdateRate;

            if (_observableObjects.Count == 0)
            {
                _objectsLifetimeInterval.Stop();
                return;
            }

            ObjectLifetime lifeTime = null;
            List<GameObject> destroyedObjects = new List<GameObject>();

            foreach (GameObject observableObject in _observableObjects)
            {
                if(observableObject.IsDestroyed())
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
                    Destroy(observableObject);
                    destroyedObjects.Add(observableObject);
                }
            }

            if (destroyedObjects.Count == 0)
                return;

            foreach (GameObject destroyerObject in destroyedObjects)
            {
                _observableObjects.Remove(destroyerObject);
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

        foreach (GameObject observableObject in _observableObjects)
        {
            Destroy(observableObject);
        }

        _observableObjects.Clear();
    }
    #endregion
}
