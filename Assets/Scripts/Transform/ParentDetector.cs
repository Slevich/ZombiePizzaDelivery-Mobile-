using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class ParentDetector : MonoBehaviour
{
    #region Fields
    [Header("Transform parent of this object.")]
    [SerializeField] private Transform _parent;
    [field: Space(5)]
    [field: Header("Called than parent changes (single mode).")]
    [field: SerializeField] public UnityEvent OnParentChanged { get; set; } = new UnityEvent();
    
    private bool _actionCalled = false;
    private ActionUpdate _update = null;
    private Action _detectingAction = null;
    #endregion

    #region Methods
    public void CommitParent() => _parent = transform.parent;

    public void StartDetection()
    {
        _update = new ActionUpdate();
        CommitParent();
        _detectingAction = delegate 
        {
            if (this == null)
                EndDetection();
                return;

            if (transform.parent != _parent && !_actionCalled)
            {
                OnParentChanged?.Invoke();
                _actionCalled = true;
                EndDetection();
            }
        };

        _update.StartUpdate(_detectingAction);
    }

    public void EndDetection()
    {
        if (_update != null)
            _update.StopUpdate();

        _update = null;
        _detectingAction = null;
        _actionCalled = false;
    }

    private void OnDestroy ()
    {
        OnParentChanged.RemoveAllListeners();
        EndDetection();
    }
    #endregion


}
