using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;

public static class TweenTransformAnimationLibrary
{
    #region Methods
    public static Tween RotateWithDirectionAlongGlobalAxis (Transform AnimatedTransform, float AngleDirection, Axes RotationAxis, float Duration)
    {
        TransformInfo info = new TransformInfo(AnimatedTransform);
        Vector3 endRotation = AxesSelector.ReturnVectorWithPlusPositionByAxis(RotationAxis, info.LocalRotationEulers, AngleDirection);
        Tween rotateTween = AnimatedTransform.DORotate(endRotation, Duration);
        return rotateTween;
    }

    public static Tween RotateWithDirectionAlongLocalAxis (Transform AnimatedTransform, float AngleDirection, Axes RotationAxis, float Duration)
    {
        TransformInfo info = new TransformInfo(AnimatedTransform);
        Vector3 endRotation = AxesSelector.ReturnVectorWithPlusPositionByAxis(RotationAxis, info.LocalRotationEulers, AngleDirection);
        Tween rotateTween = AnimatedTransform.DOLocalRotate(endRotation, Duration);
        return rotateTween;
    }

    public static Tween RotateWithDirectionLocalAxis (Transform AnimatedTransform, float AngleDirection, Axes RotationAxis, float Duration)
    {
        TransformInfo info = new TransformInfo(AnimatedTransform);
        Vector3 endRotation = AxesSelector.ReturnVectorWithPlusPositionByAxis(RotationAxis, info.LocalRotationEulers, AngleDirection);
        Tween rotateTween = AnimatedTransform.DOLocalRotate(endRotation, Duration, RotateMode.FastBeyond360);
        return rotateTween;
    }

    public static Tween RotateToGlobalTarget (Transform AnimatedTransform, Vector3 GlobalEndRotation, float Duration, Ease RotationEase = Ease.Linear)
    {
        return AnimatedTransform.DORotate(GlobalEndRotation, Duration, RotateMode.Fast).SetEase(RotationEase);
    }

    public static Tween RotateToLocalTarget (Transform AnimatedTransform, Vector3 LocalEndRotation, float Duration, Ease RotationEase = Ease.Linear)
    {
        return AnimatedTransform.DOLocalRotate(LocalEndRotation, Duration, RotateMode.Fast).SetEase(RotationEase);
    }

    //public Tween RotateAroundAxis(Transform AnimatedTransform, float AngleDirection, Axes RotationAxis, float Duration)
    //{
    //    TransformInfo info = new TransformInfo(AnimatedTransform);
    //    Vector3 endRotation = AxesSelector.ReturnVectorWithPlusPositionByAxis(RotationAxis, info.LocalRotationEulers, AngleDirection);
    //    Tween rotateTween = AnimatedTransform.DORotate(endRotation, Duration);
    //    return rotateTween;
    //}
    #endregion
}
