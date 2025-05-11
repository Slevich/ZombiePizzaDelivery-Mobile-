using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class ObservableObjectsStorage : MonoBehaviour
{
    #region Fields
    [Header("Time update in seconds to check objects on remove."), SerializeField] private float _removeObjectsUpdate = 0.1f;
    [Header("Destroyed objects per frame."), Range(0, 200), SerializeField] private int _destroyedObjectsPerCall = 80;

    private IObservableCollectionChanged[] _observers = new IObservableCollectionChanged[] { };
    private ObservableCollection<GameObject> _objectsInStorage = new ObservableCollection<GameObject>();
    private ObservableCollection<GameObject> _objectsToRemove = new ObservableCollection<GameObject>();
    private ActionInterval _removeObjectsInterval;

    #endregion

    #region Properties
    private bool _hasObservers => _observers != null && _observers.Length > 0;
    public IObservableCollectionChanged[] Observers => _observers;
    #endregion

    #region Methods
    private void Awake ()
    {
        Component[] observersComponents = gameObject.GetComponents(typeof(IObservableCollectionChanged));
        _observers = observersComponents.Select(comp => comp as IObservableCollectionChanged).ToArray();
        _removeObjectsInterval = new ActionInterval();

        _objectsToRemove.CollectionChanged += RemovingObjectsCollectionChanged;

        if (!_hasObservers)
            return;

        foreach(IObservableCollectionChanged observer in _observers)
        {
            _objectsInStorage.CollectionChanged += observer.CollectionChanged;
            observer.SendObjectsToRemove += (sentObjectsToRemove) => _objectsToRemove.AddRange(sentObjectsToRemove);
        }
    }

    private void Update () => DestroyObjects();

    private void DestroyObjects()
    {
        if (_objectsToRemove.Count == 0)
            return;

        int count = 0;

        for (int i = 0; i < _objectsToRemove.Count && i < _destroyedObjectsPerCall; i++)
        {
            GameObject destroyedObject = _objectsToRemove[i];
            _objectsToRemove.RemoveAt(i);
            Destroy(destroyedObject);
            count = i;
        }

        //Debug.Log("Удалено: " + count);
    }

    private void RemovingObjectsCollectionChanged (object? sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (_objectsToRemove.Count > 0 && !_removeObjectsInterval.Busy)
                    StartRemovingDetection();
                break;

            case NotifyCollectionChangedAction.Remove:
                if (_objectsToRemove.Count == 0 && _removeObjectsInterval.Busy)
                    StopRemovingDetection();
                break;

            default:
                break;
        }
    }

    private void StartRemovingDetection()
    {
        if (_removeObjectsInterval.Busy)
            return;

        Action OnRemoveObjects = delegate
        {
            foreach (GameObject objectToRemove in _objectsToRemove)
            {
                RemoveObjectFromStorage(objectToRemove);
            }
        };

        _removeObjectsInterval.StartInterval(_removeObjectsUpdate, OnRemoveObjects);
    }

    public void SpawnObjectIntoStorage(GameObject SpawnedPrefab)
    {
        GameObject spawnedObject = ObjectsSpawner.SpawnIntoParent(SpawnedPrefab, transform);
        AddObjectToStorage(spawnedObject);
    }

    private void AddObjectToStorage(GameObject NewObservableObject, bool SetAsParent = false)
    {
        if(SetAsParent)
            NewObservableObject.transform.SetParent(transform);

        OnObjectDestroy onObjectDestroy = null;

        if (NewObservableObject.TryGetComponent<OnObjectDestroy>(out OnObjectDestroy onDestroy))
            onObjectDestroy = onDestroy;
        else
            onObjectDestroy = NewObservableObject.AddComponent<OnObjectDestroy>();

        _objectsInStorage.Add(NewObservableObject);

        onObjectDestroy.OnDestroyCallback += delegate { _objectsToRemove.Add(NewObservableObject); };

    }

    public void AddObjectToStorageWithParenting (GameObject NewObservableObject) => AddObjectToStorage(NewObservableObject, true);
    public void AddObjectToStorageWithoutParenting (GameObject NewObservableObject) => AddObjectToStorage(NewObservableObject);

    public void DestroyObjectFromStorage(GameObject DestroyableObject)
    {
        RemoveObjectFromStorage(DestroyableObject);
        Destroy(DestroyableObject);
    }

    public void RemoveObjectFromStorage(GameObject ObservableObject)
    {
        OnObjectDestroy onObjectDestroy = null;

        if (ObservableObject.TryGetComponent<OnObjectDestroy>(out OnObjectDestroy onDestroy))
            onObjectDestroy = onDestroy;

        _objectsInStorage.Remove(ObservableObject);

        if (onObjectDestroy == null)
            return;

        onObjectDestroy.OnDestroyCallback -= delegate { _objectsToRemove.Add(ObservableObject); };
    }

    private void StopRemovingDetection()
    {
        if (_removeObjectsInterval != null && _removeObjectsInterval.Busy)
            _removeObjectsInterval.Stop();
    }

    private void OnDisable ()
    {
        foreach (IObservableCollectionChanged observer in _observers)
        {
            observer.StopObserving();
        }

        StopRemovingDetection();
    }

    private void OnDestroy ()
    {
        _objectsToRemove.CollectionChanged -= RemovingObjectsCollectionChanged;

        foreach (IObservableCollectionChanged observer in _observers)
        {
            _objectsInStorage.CollectionChanged -= observer.CollectionChanged;
            observer.SendObjectsToRemove -= (sentObjectsToRemove) => _objectsToRemove.AddRange(sentObjectsToRemove);
            observer.StopObserving();
        }

        StopRemovingDetection();
    }
    #endregion
}
