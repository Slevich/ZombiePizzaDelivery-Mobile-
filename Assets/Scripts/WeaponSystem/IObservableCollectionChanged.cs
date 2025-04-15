using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public interface IObservableCollectionChanged
{
    public void CollectionChanged (object? sender, NotifyCollectionChangedEventArgs args);
    public void StopObserving ();
    public Action<IEnumerable<GameObject>> SendObjectsToRemove { get; set; }
}
