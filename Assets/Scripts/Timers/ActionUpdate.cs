using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ActionUpdate : ActionDelayBase
{
    public ActionUpdate() : base()
    {
        _delayAction = (action, delay) =>
        {
            Observable
                .EveryUpdate()
                .Subscribe(_ => action())
                .AddTo(_disposable);
        };
    }

    public void StartUpdate(Action SomeAction)
    {
        if (!ReadyForAction(ref SomeAction))
            return;

        _busy = true;
        _disposable = new CompositeDisposable();
        _delayAction(SomeAction, 0);
    }

    protected override bool ReadyForAction (ref Action SomeAction, bool DisposeOnEnd = false)
    {
        return !_busy;
    }

    public void StopUpdate()
    {
        if (_busy)
        {
            _busy = false;
            Dispose();
        }
        else
        {
            Debug.LogError("Update didn't started yet!");
        }
    }
}
