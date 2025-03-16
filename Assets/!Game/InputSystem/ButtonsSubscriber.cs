using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Linq;

public class ButtonsSubscriber : MonoBehaviour
{
    [SerializeField] private UnityEvent _onPressed;
    [SerializeField] private UnityEvent _onReleased;

    private InputActionPhase _phase = InputActionPhase.Waiting;
    private UnityEvent _activeEventToInvoke = null;
    private InputAction _inputAction;
    private bool _currentButtonPressingToggle = false;
    private bool _invokationPerformed = false;

    private void Start ()
    {
        InputHandler.InputInfoEvent.Where(input => DetectInput(input)).Subscribe(_ => InvokePressEvent()).AddTo(this);
    }

    private bool DetectInput (IInputInfo[] infos)
    {
        bool buttonPerformed = false;

        foreach (IInputInfo info in infos)
        {
            buttonPerformed = info.FireButtonPerformed();

            if (buttonPerformed)
                break;
        }

        if(_invokationPerformed && !buttonPerformed)
            InvokeReleaseEvent();

        
        return buttonPerformed;
    }

    private void InvokePressEvent ()
    {
        if(_invokationPerformed)
            return;

        _invokationPerformed = true;
        _onPressed?.Invoke();
    }

    private void InvokeReleaseEvent()
    {
        if (!_invokationPerformed)
            return;

        _onReleased?.Invoke();
        _invokationPerformed = false;
    }
}
