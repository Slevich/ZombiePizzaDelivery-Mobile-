using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RotationAroundAxisAnimation : MonoBehaviour
{
    #region Fields
    [Header("Animated transform."), SerializeField] private Transform _animatedTransform;
    [Header("Time for one spin."), SerializeField] private float _duration = 1f;
    [Header("Global or local axis of rotation."),SerializeField] private Axes _rotationAxes = Axes.X;
    [Header("Type of the rotation"), SerializeField] private TransformType _rotationType = TransformType.Local;
    [Header("Direction of the rotation"), SerializeField] private RotationDirection _rotationDirection = RotationDirection.Clockwise;

    private Tween _currentTween = null;
    private float directionValue
    {
        get
        {
            switch(_rotationDirection)
            {
                case RotationDirection.Clockwise:
                    return 360f;

                case RotationDirection.Ñounterclockwise:
                    return -360f;

                default:
                    return 0f;
            }
        }
    }
    #endregion

    #region Methods
    private void Awake ()
    {
        if(_animatedTransform == null)
            _animatedTransform = transform;
    }

    private void GetTween()
    {
        _currentTween = TweenTransformAnimationLibrary.RotateWithDirectionLocalAxis(_animatedTransform, directionValue, _rotationAxes, _duration);
        _currentTween.SetLoops(-1, LoopType.Restart).SetAutoKill(false);
        _currentTween.onComplete += delegate {_currentTween = null; };
    }

    public void Play()
    {
        if(_currentTween == null)
            GetTween();

        _currentTween.Play();
    }

    public void Pause()
    {
        if(_currentTween != null && _currentTween.IsPlaying())
        {
            _currentTween.Pause();
        }

    }

    public void Kill()
    {
        if (_currentTween != null && _currentTween.IsPlaying())
        {
            _currentTween.Kill(true);
        }

    }

    private void OnEnable ()
    {
        
    }

    private void OnDisable ()
    {
        Kill();
    }
    #endregion

    public enum RotationDirection
    {
        Clockwise,
        Ñounterclockwise
    }
}
