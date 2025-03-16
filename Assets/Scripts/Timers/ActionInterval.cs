using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.UIElements;

public class ActionInterval : ActionDelayBase
{
    Action _endAction;

    public ActionInterval() : base()
    {
        _delayAction = (action, delay) =>
        {
            Observable
                .Interval(TimeSpan.FromSeconds(delay))
                .Subscribe(_ => action())
                .AddTo(_disposable);
        };
    }

    public void StartInterval(float TimeInterval, Action SomeAction)
    {
        if(!ReadyForAction(ref SomeAction))
            return;

        _busy = true;
        _disposable = new CompositeDisposable();
        _endAction = SomeAction;
        _delayAction(_endAction, TimeInterval);
    }

    protected override bool ReadyForAction (ref Action SomeAction, bool DisposeOnEnd = false)
    {
        return !_busy;
    }

    public void Stop(bool InvokeOnEnd = false)
    {
        if (_busy)
        {
            _busy = false;
            Dispose();

            if (InvokeOnEnd)
                _endAction?.Invoke();
        }
        else
        {
            Debug.LogError("Interval didn't started yet!");
        }
    }
}
