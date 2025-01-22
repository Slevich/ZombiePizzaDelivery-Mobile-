using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;

public class AxisMover : MonoBehaviour
{
    #region Fields
    [SerializeField] private AxisPositionChanger _positionChanger;
    [SerializeField, Range(1, 20)] private float _speedModifier = 1f;

    private float _axisOffset = 0f;
    IInputInfo activeInfo;
    #endregion

    #region Methods
    private void Awake ()
    {
        InputHandler.InputInfoEvent.Where(input => DetectInputEmitter(input)).Subscribe(_ => MoveByAxis()).AddTo(this);
    }

    private void OnValidate ()
    {
        if (_speedModifier <= 0f)
            _speedModifier = 1f;
    }

    private bool DetectInputEmitter(IInputInfo[] infos)
    {
        IInputInfo inputInfo = infos.Where(info => info.ReturnInputState() == InputActionPhase.Started).FirstOrDefault();

        if (inputInfo == null)
        {
            activeInfo = null;
            return false;
        }
        else
        {
            activeInfo = inputInfo;
            return true;
        }
    }

    private void MoveByAxis ()
    {
        if (activeInfo == null)
            return;

        float inputDirection = activeInfo.ReturnInputDirection().x;

        float updateOffset = (Time.fixedDeltaTime * _speedModifier) * inputDirection;
        float offset = _axisOffset + updateOffset;

        if (offset > _positionChanger.MaxAxisValue)
            offset = _positionChanger.MaxAxisValue;
        else if (offset < _positionChanger.MinAxisValue)
            offset = _positionChanger.MinAxisValue;

        _axisOffset = offset;
        _positionChanger.SetPositionOffsetOnAxis(_axisOffset);
    }
    #endregion
}
