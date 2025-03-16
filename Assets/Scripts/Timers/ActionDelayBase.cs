using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;


public abstract class ActionDelayBase
{
    #region Fields
    protected float _delay = 0f;
    protected CompositeDisposable _disposable;
    protected bool _busy = false;
    protected Action<Action, float> _delayAction;
    #endregion

    #region Properties
    public bool Busy => _busy;
    #endregion

    #region Constructor
    public ActionDelayBase ()
    {
        _delay = 0f;
        _busy = false;

        if (_disposable != null && !_disposable.IsDisposed)
            Dispose();

        _disposable = new CompositeDisposable();
    }
    #endregion

    #region Methods
    protected virtual bool ReadyForAction (ref Action SomeAction, bool DisposeOnEnd = false)
    {
        if (_busy)
            return false;
        else
            SomeAction += delegate { _busy = false; };

        if(DisposeOnEnd)
            SomeAction += Dispose;

        _busy = true;
        return true;
    }

    protected void Dispose() => _disposable?.Dispose();
    #endregion
}
