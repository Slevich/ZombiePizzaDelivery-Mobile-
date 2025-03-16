using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIDamageWarning : MonoBehaviour
{
    [Header("Image to change color."), SerializeField] private Image _warningMaskImage;
    [Header("Animation end color."), SerializeField] private Color _warningMaskColor;
    [Header("Time in seconds."), SerializeField] private float _warningMaskFlashDuration;

    [Header("Image to change color."), SerializeField] private Image _warningSignImage;
    [Header("Animation end color."), SerializeField] private Color _warningSignColor;
    [Header("Time in seconds."), SerializeField] private float _warningSignFlashDuration;

    private Color _startWarningMaskColor;
    private Color _startWarningSignColor;
    private Tween _warningMaskColorTween;
    private Tween _warningSignColorTween;
    private Sequence _currentSequence;

    private void Awake ()
    {
        _startWarningMaskColor = _warningMaskImage.color;
        _startWarningSignColor = _warningSignImage.color;
    }

    public void StartAnimation()
    {
        _currentSequence = DOTween.Sequence();
        _warningMaskColorTween = TweenUIAnimationsLibrary.ChangeUIImageColor(_warningMaskImage, _warningMaskColor, _warningMaskFlashDuration)
                                                  .SetLoops(-1, LoopType.Yoyo)
                                                  .SetEase(Ease.Linear);
        _warningSignColorTween = TweenUIAnimationsLibrary.ChangeUIImageColor(_warningSignImage, _warningSignColor, _warningSignFlashDuration)
                                                  .SetLoops(-1, LoopType.Yoyo)
                                                  .SetEase(Ease.Linear);
        _currentSequence.Append(_warningMaskColorTween).Join(_warningSignColorTween);
        _currentSequence.Play();
    }

    public void ReturnToBaseColor()
    {
        if (!_currentSequence.IsActive())
            return;

        DropAnimation();
        _currentSequence = DOTween.Sequence();
        _warningMaskColorTween = TweenUIAnimationsLibrary.ChangeUIImageColor(_warningMaskImage, _startWarningMaskColor, _warningMaskFlashDuration)
                                                  .SetEase(Ease.Linear);
        _warningSignColorTween = TweenUIAnimationsLibrary.ChangeUIImageColor(_warningSignImage, _startWarningSignColor, _warningSignFlashDuration)
                                          .SetEase(Ease.Linear);
        _currentSequence.Append(_warningMaskColorTween).Join(_warningSignColorTween);
        _currentSequence.Play();
    }

    public void DropAnimation()
    {
        if (_currentSequence.IsActive())
            _currentSequence.Kill();
    }

    private void OnDisable ()
    {
        DropAnimation();
    }
}
