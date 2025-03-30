using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class ItemPickUpAnimation : MonoBehaviour
{
    #region Fields
    [SerializeField] private ObtainableItem _obtainable;
    [SerializeField] private Transform _animatedTransform;
    [SerializeField] private float _moveUpDistance = 1f;
    [SerializeField] private float _moveUpDuration = 0.5f;
    [SerializeField] private float _moveToTargetDuration = 1f;
    [SerializeField] private GameObject _spawnedOnNotPick;

    private Sequence _animSequence;
    #endregion

    #region Properties
    private bool _transformIsNull => _animatedTransform == null;
    #endregion


    #region Methods
    private void Awake ()
    {
        if(_transformIsNull)
            _animatedTransform = GetComponent<Transform>();

        if (_obtainable != null)
            _obtainable.OnInteractWithTransform += (pickedUp, target, onEnd) => SelectAnimation(pickedUp, target, onEnd);
    }

    private void SelectAnimation(bool state, Transform target, Action OnEnd)
    {
        if (state)
            LerpToTransform(target, OnEnd);
        else
            DestroyWithAnimation();
    }

    public void LerpToTransform(Transform TargetTransform, Action OnEnd)
    {
        if (_animSequence != null && _animSequence.active)
            return;

        _animatedTransform.SetParent(TargetTransform.parent);
        float yEndPosition = _animatedTransform.position.y; 
        Tween moveUpTween = _animatedTransform.DOMoveY(yEndPosition + _moveUpDistance, _moveUpDuration);

        Tween moveToTargetTween = _animatedTransform.DOLocalMove(TargetTransform.localPosition, _moveToTargetDuration);

        Tween scaleTween = _animatedTransform.DOScale(Vector3.zero, _moveToTargetDuration);

        _animSequence = DOTween.Sequence();
        _animSequence.Join(moveUpTween).Append(moveToTargetTween).Join(scaleTween).OnComplete(delegate { OnEnd?.Invoke();  Destroy(gameObject); });
        _animSequence.Play();
    }

    public void DestroyWithAnimation()
    {
        if (_animSequence != null && _animSequence.active)
            _animSequence.Kill();

        Tween scaleTween = _animatedTransform.DOScale(Vector3.zero, _moveToTargetDuration).OnComplete(delegate 
        {
            Instantiate(_spawnedOnNotPick, null, true);
            Destroy(gameObject); 
        });

        scaleTween.Play();
    }

    private void OnDisable ()
    {
        if(_animSequence != null && _animSequence.active)
            _animSequence.Kill();
    }
    #endregion
}
