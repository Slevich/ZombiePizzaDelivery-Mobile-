using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Threading.Tasks;
using System.Threading;

public class AxisMover : MonoBehaviour
{
    #region Fields
    [Header("Max movement speed."), SerializeField, Range(0, 1)] private float _maxSpeedSlider = 1f;
    [Header("Delay in seconds."), SerializeField, Range(0, 1)] private float _delayBeforeMovement = 0.2f;

    private IInputInfo _activeInfo;
    private AxisPositionChanger _positionChanger;
    private CancellationTokenSource _cancellationTokenSource;
    private ActionInterval _actionInterval;
    public Action<float> OnMovementDirection;
    private Action _onInterval;
    private float _currentOffsetChange = 0f;
    private float _inputDirection = 0f;
    private float _currentSpeed = 0f;
    private float _targetSpeedTotalTimeInput = 0.8f;
    private float _targetSpeedTotalTimeInertia = 0.7f;
    private float _targetSpeedTimeSteps = 0f;
    private float _inertiaSpeedTimeSteps = 0f;
    private float _speedChangeInterval = 0.1f;
    private float _fixedDeltaTime = 0f;
    private bool _movementInProcess = false;
    private bool _inertiaInProcess = false;
    #endregion

    #region Properties
    private float MoveDirection
    {
        set 
        {
            if (_inputDirection == value)
                return;

            if(value != 0)
            {
                if(!_movementInProcess)
                    MoveByInputAsync(value);
            }
            else if(value == 0)
            {
                if(_movementInProcess && !_inertiaInProcess)
                    CancelSubstepTokenSource();
            }

            _inputDirection = value; 
        }
    }
    #endregion

    #region Methods
    private void Awake ()
    {
        InputHandler.InputInfoEvent.Where(input => DetectInputEmitter(input)).Subscribe(_ => ChangeMovementDirection()).AddTo(this);
        _positionChanger = PlayerReferencesContainer.Instance.PositionChanger;;
        _currentOffsetChange = _positionChanger.AxisOffsetChange;
        _fixedDeltaTime = Time.fixedDeltaTime;
        _targetSpeedTimeSteps = _targetSpeedTotalTimeInput / _speedChangeInterval;
        _inertiaSpeedTimeSteps = _targetSpeedTotalTimeInertia / _speedChangeInterval;
        _actionInterval = new ActionInterval();
        _onInterval = delegate { _positionChanger.AxisOffsetChange = _currentOffsetChange; };
    }

    private bool DetectInputEmitter(IInputInfo[] infos)
    {
        _activeInfo = infos.Where(info => info.ReturnInputState() == InputActionPhase.Started).FirstOrDefault();

        if (_activeInfo == null)
        {
            MoveDirection = 0;
            return false;
        }    
        else
            return true;
    }

    private void ChangeMovementDirection()
    {
        if (_activeInfo == null)
            return;

        MoveDirection = _activeInfo.ReturnInputDirection().x;
    }

    private async void MoveByInputAsync(float inputDirection)
    {
        _movementInProcess = true;
        OnMovementDirection(inputDirection);

        Task delay = DelayAwaitAsync(_cancellationTokenSource);

        try
        {
            await delay;
        }
        catch (OperationCanceledException)
        {
            goto Exit;
        }

        Task lerpMaxSpeedTask = LerpSpeed(_cancellationTokenSource, _maxSpeedSlider, _targetSpeedTimeSteps);
        Task changeOffsetTask = ChangeOffsetByInput(_cancellationTokenSource, inputDirection);

        try
        {
            await changeOffsetTask;
        }
        catch (OperationCanceledException)
        {
            ResetSubstepTokenSource();
            OnMovementDirection(0);
            _inertiaInProcess = true;
            changeOffsetTask = ChangeOffsetByInput(_cancellationTokenSource, inputDirection);
            await LerpSpeed(_cancellationTokenSource, 0, _inertiaSpeedTimeSteps);
            _inertiaInProcess = false;
        }

        Exit:
        if(!_cancellationTokenSource.IsCancellationRequested)
        {
            CancelSubstepTokenSource();
        }

        ResetSubstepTokenSource();
        OnMovementDirection(0);
        _currentSpeed = 0;
        _currentOffsetChange = 0;
        _movementInProcess = false;
    }

    private Task DelayAwaitAsync(CancellationTokenSource source)
    {
        int miliSeconds = Mathf.RoundToInt(_delayBeforeMovement * 1000);

        Task delayTask = Task.Delay(miliSeconds, source.Token);
        return delayTask;
    }

    private Task ChangeOffsetByInput(CancellationTokenSource source, float inputDirection)
    {
        bool finish = false;
        int miliseconds = Mathf.RoundToInt(_fixedDeltaTime * 1000);

        Func<Task> offsetChange = async () =>
        {
            while (!finish)
            {
                finish = ChangeOffset(inputDirection);
                await Task.Delay(miliseconds, source.Token);
            }
        };

        Task positionChangeTask = Task.Run(offsetChange, source.Token);
        return positionChangeTask;
    }

    private Task LerpSpeed(CancellationTokenSource source, float TargetSpeed, float SpeedSteps)
    {
        bool finish = false;
        int miliseconds = Mathf.RoundToInt(_speedChangeInterval * 1000);
        float leftSpeed = TargetSpeed - _currentSpeed;
        float speedChangePerInterval = leftSpeed / SpeedSteps;

        Func<Task> speedlerp = async () =>
        {
            while (!finish)
            {
                finish = TryToLerpSpeed(TargetSpeed, speedChangePerInterval);
                await Task.Delay(miliseconds, source.Token);
            }
        };

        Task speedLerpingTask = Task.Run(speedlerp, source.Token);
        return speedLerpingTask;
    }

    private bool TryToLerpSpeed(float TargetSpeed, float SpeedStep)
    {
        float changedSpeed = MathF.Round(_currentSpeed + SpeedStep, 2);
        _currentSpeed = Mathf.Clamp(changedSpeed, 0, _maxSpeedSlider);

        if (_currentSpeed == TargetSpeed)
        {
            return true;
        }

        return false;
    }

    private bool ChangeOffset(float inputDirection)
    {
        bool borderIsReached = false;
        _currentOffsetChange = _currentSpeed * inputDirection;

        NumbersZeroEquality numberDirection = (NumbersZeroEquality)Mathf.RoundToInt(inputDirection);

        switch (numberDirection)
        {
            case NumbersZeroEquality.Negative:
                borderIsReached = _positionChanger.ReachMin;
                break;

            case NumbersZeroEquality.Positive:
                borderIsReached = _positionChanger.ReachMax;
                break;
        }

        return borderIsReached;
    }


    private void CancelSubstepTokenSource()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Cancel();
        }
    }

    private void ResetSubstepTokenSource()
    {
        if (_cancellationTokenSource != null)
        {
            _cancellationTokenSource.Dispose();
        }

        _cancellationTokenSource = new CancellationTokenSource();
    }

    private void OnDisable ()
    {
        CancelSubstepTokenSource();

        if (_actionInterval.Busy)
            _actionInterval.Stop();
    }

    private void OnEnable()
    {
        ResetSubstepTokenSource();

        if (_actionInterval != null && !_actionInterval.Busy)
            _actionInterval.StartInterval(_fixedDeltaTime, _onInterval);
    }
    #endregion
}

public enum NumbersZeroEquality
{
    Zero = 0,
    Positive = 1, 
    Negative = -1
}