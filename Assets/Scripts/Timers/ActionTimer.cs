using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ActionTimer : ActionDelayBase
{
    public ActionTimer() : base()
    {
        _delayAction = (action,delay) =>
        {
            Observable
                .Timer(TimeSpan.FromSeconds(delay))
                .Subscribe(_ => action())
                .AddTo(_disposable);
        };
    }

    public void StartTimerAndAction(float TimerValue, Action SomeAction, bool DisposeOnEnd = false)
    {
        if (!ReadyForAction(ref SomeAction, DisposeOnEnd))
            return;

        _disposable = new CompositeDisposable();
        _delayAction(SomeAction, TimerValue);
    }

    public void StopTimer()
    {
        if(_busy)
        {
            _busy = false;
            Dispose();
        }
        else
        {
            Debug.LogError("Timer didn't started yet!");
        }
    }
}
