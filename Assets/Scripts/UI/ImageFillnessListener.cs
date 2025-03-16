using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ImageFillnessListener : MonoBehaviour
{
    #region Fields
    [Header("UI Image to listen fillness."), SerializeField] private Image _listenedImage;
    [Header("Conditions and events on value changed."), SerializeField] private DependsOnFloatConditionAction[] _actions;

    private static readonly float _imageMaxFillness = 1f;
    private static readonly float _imageMinFillness = 0f;
    #endregion

    #region Properties
    private bool _hasImage => _listenedImage != null;
    private bool _correctType => _listenedImage.type == Image.Type.Filled;
    private bool _haveActions => _actions.Length > 0;
    #endregion

    #region Methods
    private void Awake ()
    {
        if (_listenedImage == null)
        {
            Debug.LogError("Image fillness listener has no image reference!");
            return;
        }
    }

    public void SetImageFillness(float NewImageFillness)
    {
        if (!_hasImage || !_correctType || !_haveActions)
        {
            Debug.LogError("ImageFillnessListener error!");
            return;
        }

        if (NewImageFillness > _imageMaxFillness)
            NewImageFillness = _imageMaxFillness;

        if(NewImageFillness < _imageMinFillness)
            NewImageFillness = _imageMinFillness;

        _listenedImage.fillAmount = NewImageFillness;
        CheckActionsConditions();
    }

    private void CheckActionsConditions()
    {
        foreach(DependsOnFloatConditionAction action in _actions)
        {
            action.CheckConditionAndInvoke(_listenedImage.fillAmount);
        }
    }
    #endregion
}

[Serializable]
public class DependsOnFloatConditionAction
{
    [Header("Condition float value."), SerializeField, Range(0f,1f)] private float _value;
    [Header("Value condition."), SerializeField] private NumberCondition _condition;
    [Header("Event on true condition state."), SerializeField] private UnityEvent _onChange;

    public void CheckConditionAndInvoke(float checkingValue)
    {
        bool conditionIsCompleted = CheckConditionDependsOnType(checkingValue);

        if (conditionIsCompleted)
            _onChange?.Invoke();
    }

    private bool CheckConditionDependsOnType(float checkingValue) => _condition switch
    {
           NumberCondition.Less => checkingValue < _value,
           NumberCondition.LessOrEqual => checkingValue <= _value,
           NumberCondition.Equal => checkingValue == _value,
           NumberCondition.Greater => checkingValue > _value,
           NumberCondition.GreaterOrEqual => checkingValue >= _value,

           var t => throw new NotSupportedException($"Condition '{_condition.ToString()}' is not supported!")
    };
}

public enum NumberCondition
{
    Less,
    LessOrEqual,
    Equal,
    Greater,
    GreaterOrEqual,
}